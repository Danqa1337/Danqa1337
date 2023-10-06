using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Collections;
[System.Serializable]
public struct TileData : IComponentData
{
    public static TileData Null { get; }
    public int index;
    public int2 position;
    public int x => position.x;
    public int y => position.y;


    public bool visible;
    public bool maped;
    public int T;
    public int P;
    public float L;
    public float DstToPlayer;
    public float2 Airflow;
    public bool isAbyss => FloorLayer == Entity.Null;
    public bool HasLOSblock => SolidLayer.HasComponent<LOSBlockTag>();

    [NonSerialized]public Entity SolidLayer;
    [NonSerialized] public Entity LiquidLayer;
    [NonSerialized] public Entity FloorLayer;
    [NonSerialized] public Entity HoveringLayer;
    
    public bool updateScheduled;
    public bool impact;
    public Biome biome;
    public TileTemplate template
    {
        get => MapGenerator.templateMap[index];

        set
        {
            MapGenerator.templateMap[index] = value;
        }
    }
    public readonly List<Entity> DropLayer
    {
       get => TileUpdater.TileDropContainers [index];
    }
    public readonly List<Entity> GroundCoverLayer
    {
        get => TileUpdater.TileGroundCoverContainers[index];
        set => TileUpdater.TileGroundCoverContainers[index] = value;
    }
    public void SetBiome(Biome biome)
    {
        this.biome = biome;
        Save();
    }
    public void Drop(Entity item)
    {  
        
        var tr = item.GetComponentObject<Transform>();
        tr.rotation = Quaternion.Euler(0,0, UnityEngine.Random.Range(90f, 270f));

        if (!DropLayer.Contains(item))
        {
            TileUpdater.TileDropContainers[index].Add(item);
            var cr = item.GetComponentData<CurrentTileComponent>();

            foreach (var part in item.GetComponentData<AnatomyComponent>().GetLoverHierarchy())
            {
                part.GetComponentObject<RendererComponent>().spritesSortingLayerName = "Drop";
            }
            var renderer = item.GetComponentObject<RendererComponent>();

            tr.SetParent(null);
            //tr.rotation = quaternion.Euler(0, 0, UnityEngine.Random.Range(90, 270));
            tr.position = position.ToRealPosition() + (Vector3) renderer.spriteCenterOffset;



            cr.currentTileId = index;

            cr.objectType = ObjectType.Drop;
            item.SetComponentData(cr);
            Debug.Log(item.GetName() + " dropped on " + position);
            Save();
        }
        //else
        //{
        //    throw new System.ArgumentOutOfRangeException("Drop allready contains " + item.GetName());
        //}
    }

    public void ApplyTemplate()
    {

        if (template.SolidLayer != -1) Spawner.Spawn(template.SolidLayer, position);
        if (template.DropLayer != -1) Spawner.Spawn(template.DropLayer, position);
        if (template.FloorLayer != -1) Spawner.Spawn(template.FloorLayer, position);
        if (template.LiquidLayer != -1) Spawner.Spawn(template.LiquidLayer, position);
        if (template.GroundCover != -1) Spawner.Spawn(template.GroundCover, position);

    }
    
    public HashSet<RendererComponent> GetAllRenderers()
    {
        
        var renderers = new HashSet<RendererComponent>();
        var objects = new HashSet<Entity>();
        
        if (SolidLayer != Entity.Null)
        {
            foreach (var item in SolidLayer.GetComponentData<AnatomyComponent>().GetLoverHierarchy())
            {
                objects.Add(item);
            }
        }
        if (LiquidLayer != Entity.Null)
        {
            objects.Add(LiquidLayer);
        }
        if (HoveringLayer != Entity.Null)
        {
            objects.Add(HoveringLayer);
        }
        if (FloorLayer != Entity.Null)
        {

            objects.Add(FloorLayer);

        }
        foreach (var item in DropLayer)
        {
            foreach (var part in item.GetComponentData<AnatomyComponent>().GetLoverHierarchy())
            {
                objects.Add(part);
            }
        }
        foreach (var item in GroundCoverLayer)
        {
            objects.Add(item);
        }
        foreach (var item in objects)
        {
            renderers.Add(item.GetComponentObject<RendererComponent>());
        }

        return renderers;
    }
    public void ShowUnmapable()
    {
        if (SolidLayer.HasComponent<AIComponent>())
        {
            SolidLayer.ShowRenderer();
           
        }
        if(HoveringLayer.HasComponent<AIComponent>())
        {
            HoveringLayer.ShowRenderer();
        }
        foreach (var item in DropLayer)
        {
            item.ShowRenderer();
        }
    }
    public void HideUnmapable()
    {
        if (SolidLayer.HasComponent<AIComponent>())
        {
            SolidLayer.HideRenderer();

        }
        if (HoveringLayer.HasComponent<AIComponent>())
        {
            HoveringLayer.HideRenderer();
        }
        foreach (var item in DropLayer)
        {
            item.HideRenderer();
        }
    }
    public void Save()
    {
        TileUpdater.current[index] = this;
    }
    public void Save(NativeArray<TileData> array)
    {
        array[index] = this;
    }
    public bool isInsideCameraFrustrum()
    {
        var frustrum = MainCameraHandler.i.cam.WorldToViewportPoint(position.ToRealPosition());
        if(frustrum.x > 1  || frustrum.x < 0 || frustrum.y > 1 || frustrum.y < 0)
        {
            return false;
        }
        return true;
    }
    public bool isWalkable(Entity creature)
    {
        if (creature != PlayerAbilitiesSystem.playerEntity || maped || PlayerAbilitiesSystem.playerEntity == Entity.Null) //is not player or tile maped
        {

            if (creature == Entity.Null) //query from generator
            {
                if (SolidLayer == Entity.Null) //is free
                {
                    return true;
                }
            }
            else // creature is not null
            {
                if (creature.GetObjectType() == ObjectType.Hovering) //is hovering object
                {
                    return true;
                }
                else
                {
                    if (SolidLayer == Entity.Null) //is free
                    {
                        
                        if (isAbyss) //tile has no floor
                        {
                            if (creature.HasTag(Tag.Flying))//but creature can fly
                            {
                                return true;
                            }
                        }
                        else // tile has floor
                        {
                            return true;
                        }
                    }
                    else //is not free
                    {

                        if (SolidLayer.HasComponent<AIComponent>()) // solid is creature
                        {
                            if (SolidLayer.CanBeSwapedPotentialy(creature)) // creatures can swap 
                            {
                                return true;
                            }

                        }
                        else
                        {
                            if (creature.HasTag(Tag.Digger)) // creature can dig
                            {
                                return true;
                            }
                        }
                        
                    }
                }
            }
        }
        return false;
    }
    public TileData[] GetNeibors(bool diagonals)
    {
        TileData[] neibors;
        if (diagonals)
        {
            neibors = new TileData[8];
            for (int i = 0; i < 8; i++)
            {
                neibors[i] = (position + TileUpdater.NeiborsOffsetsArray8[i]).ToTileData();
            }
        }
        else
        {
            neibors = new TileData[4];
            for (int i = 0; i < 4; i++)
            {
                neibors[i] = (position + TileUpdater.NeiborsOffsetsArray8[i]).ToTileData();
            }
        }

        return neibors;
    }

    public void Remove(Entity entity)
    {
        if (SolidLayer == entity) SolidLayer = Entity.Null;
        if (DropLayer.Contains(entity)) TileUpdater.TileDropContainers[index].Remove(entity);
        if (GroundCoverLayer.Contains(entity)) TileUpdater.TileGroundCoverContainers[index].Remove(entity);
        Save();
    }
    public bool CheckStateInNeibors(TileState _state, bool includeDiagonals, int numberOfNaiborsWithState = 1)
    {
        int n = 0;
        foreach (TileData neibor in GetNeibors(includeDiagonals))
        {
            if (neibor.template.tileState == _state) n++;
            if (n == numberOfNaiborsWithState) return true;
        }
        return false;
    }
    public bool checkDirectionState(Direction direction, TileState state)
    {

        if (direction == Direction.Null)
        {
            if (template.tileState == state) return true;
            return false;
        }

        if ((this + direction) != TileData.Null && (this + direction).template.tileState == state)
        {
            //Debug.Log("tile on " + (tile.position + getVectorFromDirection(direction)) + " is " + state);
            return true;
        }
        else
        {
            // Debug.Log("tile on " + (tile.position + getVectorFromDirection(direction)) + " is not " + state);
            return false;
        }

    }
    public bool isBorderTile()
    {
        if (position.x == 0 || position.y == 0 || position.x == 64 - 1 || position.y == 64 - 1)
        {
            return true;
        }
        return false;
    }

    public bool isPlaceForDoor()
    {
        if (template.tileState == TileState.Floor || template.tileState == TileState.Door)
        {

            if ((this + Direction.L).template.tileState == TileState.Wall && (this + Direction.R).template.tileState == TileState.Wall)
            {
                return true;
            }
            else if ((this + Direction.U).template.tileState == TileState.Wall && (this + Direction.D).template.tileState == TileState.Wall)
            {
                return true;
            }
        }
        return false;
    }
    public int GetDistanceFromEdge()
    {
        return Mathf.Min(math.abs(position.x - 32), math.abs(position.y - 32));
    }
    public static TileData operator +(TileData tileData, int2 vector)
    {
        return (tileData.position + vector).ToTileData();
    }
    public static TileData operator -(TileData tileData, int2 vector)
    {
        return (tileData.position - vector).ToTileData();
    }

    public static int2 operator +(TileData A, TileData B)
    {
        return A.position + B.position;
    }
    public static TileData operator +(TileData A, Direction direction)
    {
        switch (direction)
        {
            case Direction.U:
                return A + new int2(0, 1);
            case Direction.R:
                return A + new int2(1, 0);
            case Direction.L:
                return A + new int2(-1, 0);
            case Direction.D:
                return A + new int2(0, -1);

            case Direction.DL:
                return A + new int2(-1, -1);
            case Direction.DR:
                return A + new int2(1, -1);
            case Direction.UL:
                return A + new int2(-1, 1);
            case Direction.UR:
                return A + new int2(1, 1);

            case Direction.Null:
                return A + new int2(0, 0);

        }
        return TileData.Null;
    }
    public static int2 operator -(TileData A, TileData B)
    {
        return A.position - B.position;
    }
    public static bool operator ==(TileData A, TileData B)
    {
        return A.position.x == B.position.x && A.position.y == B.position.y;
    }
    public static bool operator !=(TileData A, TileData B)
    {
        return A.position.x != B.position.x || A.position.y != B.position.y ;
    }
    public bool ClearLineOfSight(TileData tile2, out float distance)
    {

        float2 raycastFloatPos = position;
        TileData currentRaycastTile;
        float2 raycastDirection = math.normalize(tile2.position - position);

        distance = 0f;
        int testIndex = 0;
        while (true)
        {
            distance++;
            raycastFloatPos += raycastDirection;
            currentRaycastTile = new int2((int)(raycastFloatPos.x + 0.5f), (int)(raycastFloatPos.y + 0.5f)).ToTileData();

            if (testIndex == -1 || currentRaycastTile == tile2) return true;
            if (currentRaycastTile.HasLOSblock)
            {
                return false;
            }
        }
    } 
    public bool ClearTraectory(TileData tile2)
    {

        float2 raycastFloatPos = position;
        TileData currentRaycastTile;
        float2 raycastDirection = math.normalize(tile2.position - position);

        int testIndex = 0;
        while (true)
        {
            raycastFloatPos += raycastDirection;
            currentRaycastTile = new int2((int)(raycastFloatPos.x + 0.5f), (int)(raycastFloatPos.y + 0.5f)).ToTileData();

            if (testIndex == -1 || currentRaycastTile == tile2) return true;
            if (currentRaycastTile.SolidLayer != Entity.Null)
            {
                //if(currentRaycastTile != this && currentRaycastTile != tile2)
                {
                    return false;
                }
               
            }
        }
    }

    public bool ClearLineOfSight(TileData tile2, NativeArray<TileData> array, out float distance)
    {

        float2 raycastFloatPos = position;
        TileData currentRaycastTile;
        float2 raycastDirection = math.normalize(tile2.position - position);

        distance = 0f;
        int testIndex = 0;
        while (true)
        {
            distance++;
            raycastFloatPos += raycastDirection;
            testIndex = new int2((int)(raycastFloatPos.x + 0.5f), (int)(raycastFloatPos.y + 0.5f)).ToMapIndex();

            if (testIndex == -1) return true;

            currentRaycastTile = array[testIndex];

            if (currentRaycastTile == tile2) return true;

            if (currentRaycastTile.HasLOSblock)
            {
                return false;
            }
        }
    }
    public bool ClearLineOfSight(TileData tile2)
    {
        float f;
        return ClearLineOfSight(tile2, out f);
    }
}