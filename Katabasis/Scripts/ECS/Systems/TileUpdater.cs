using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
using System.Linq;
using Unity.Burst;
using System.Threading.Tasks;

public struct IndividualTileJob : IJob
{
    
    private bool testDone;
    [ReadOnly] public NativeArray<TileData> current;
    [WriteOnly] public NativeArray<TileData> next;
    [ReadOnly] public TileData playerTile;
    [ReadOnly] public int playerViewDistance;
    [ReadOnly] public float maxDarcnessA;
    [ReadOnly] public NativeArray<int2> neiborOffsets;

    public bool FOVUpdateSchaduled;
    private int testIndex;
    
    private TileData newTileData;
    private TileData testData;

    public void Execute()
    {
        testDone = false;
        float time = 0;
        for (int index = 0; index < current.Length; index++)
        {
            
            newTileData = current[index];
            if (newTileData.isAbyss)
            {
                if (newTileData.SolidLayer != Entity.Null
                    && !newTileData.SolidLayer.HasComponent<ImpulseComponent>()
                    && !newTileData.SolidLayer.GetTags().Contains(Tag.Flying))
                {
                  //  AnimationSystem.AddAnimation()
                  // newTileData.SolidLayer.GetBuffer<>(new AnimationElement(TileData.Null, AnimationType.FallToTheAbyss));
                }
                foreach (var item in newTileData.DropLayer)
                {
                    if (!item.HasComponent<ImpulseComponent>())
                    {
                      // .. item.AddComponentData(new AnimationElement(TileData.Null, AnimationType.FallToTheAbyss));
                    }
                }
            }


            if (FOVUpdateSchaduled)
            {   
                bool wasVisible = newTileData.visible;
                if (playerTile.GetSqrDistance(newTileData) < playerViewDistance*playerViewDistance)
                {
                    float dst;
                    newTileData.visible = playerTile.ClearLineOfSight( newTileData, current,out dst);
                    newTileData.DstToPlayer = dst;
                    

                }
                else newTileData.visible = false;

                if (wasVisible != newTileData.visible) newTileData.updateScheduled = true;
                
                
                if (newTileData.visible)
                {
                    newTileData.updateScheduled = true;
                    newTileData.maped = true;
                    newTileData.L = 1 - math.clamp(newTileData.DstToPlayer/15, 0f,0.8f);
                }
                else
                {   
                    if(newTileData.maped) newTileData.L = 0.1f;
                    else newTileData.L = 0;
                    
                }
            }

            //if (newTileData.visible) Debug.Log("!");

            //int T = 0;
            //int P = 0;
            //float2 air = int2.zero;

            //for (int i = 0; i < 8; i++)
            //{
            //    testIndex = (newTileData.position + neiborOffsets[i]).ToMapIndex();
            //    if (testIndex != -1)
            //    {
            //        testData = current[testIndex];
            //        T += testData.T;
            //        P += testData.P;
            //        if (P > 0) air += testData.Airflow;
            //    }
            //}

            //newTileData.T = T / 9;
            //newTileData.P = P / 9;
            //if (P > 0) newTileData.Airflow = math.normalize(air);
            //if(newTileData.impact)
            //{
            //    newTileData.impact = false;
            //}
            next[index] = newTileData;

            
           
        }
        
    }

    private static bool HasWall(TileData tileData)
    {
        //bool b =World.DefaultGameObjectInjectionWorld.EntityManager.HasComponent<WallComponent>(tileData.entity);
        //if (b) Debug.Log(tileData.entity.Index + " " + World.DefaultGameObjectInjectionWorld.EntityManager.GetName(tileData.entity) + " " + tileData.position);
        return false;
    }
}


public static class TileUpdater
{
    public static bool FOVUpdateScheduled;
    public static bool RenderFrameScheduled;
    [ReadOnly] public static NativeArray<TileData> current;
    private static NativeArray<TileData> next;
    private static NativeArray<TileData> tilesToUpdate;
    public static NativeArray<int2> NeiborsOffsetsArray8;
    public static NativeArray<int2> NeiborsOffsetsArray4;

    public static List<Entity>[] TileDropContainers;
    public static List<Entity>[] TileGroundCoverContainers;


    public static void Dispose()
    {
        if (current.IsCreated) current.Dispose();
        if (next.IsCreated) next.Dispose();
        if (tilesToUpdate.IsCreated) tilesToUpdate.Dispose();
    }
    public static void SetTileData(TileData tileData)
    {
        current[tileData.index] = tileData;
    }
    public static void Init()
    {
        if (NeiborsOffsetsArray8.IsCreated)
        {
            NeiborsOffsetsArray8.Dispose();
        }

        NeiborsOffsetsArray8 = new NativeArray<int2>(new int2[]
        {
                new int2(0,1),
                new int2(-1,0),
                new int2(1,0),
                new int2(0,-1),
                new int2(-1,1),
                new int2(1,1),
                new int2(-1,-1),
                new int2(1,-1),

        },
            Allocator.Persistent
        );

        if (NeiborsOffsetsArray4.IsCreated)
        {
            NeiborsOffsetsArray4.Dispose();
        }

        NeiborsOffsetsArray4 = new NativeArray<int2>(new int2[]
        {
            new int2(0,1),
            new int2(-1,0),
            new int2(1,0),
            new int2(0,-1),

        },
        Allocator.Persistent
        );
        
        if (current.IsCreated) current.Dispose(); current = new NativeArray<TileData>(4096, Allocator.Persistent);
        if (next.IsCreated) next.Dispose(); next = new NativeArray<TileData>(4096, Allocator.Persistent);
        TileDropContainers = new List<Entity>[4096];
        TileGroundCoverContainers = new List<Entity>[4096];

        for (int i = 0; i < current.Length; i++)
        {
            current[i] = new TileData()
            {
                position = i.ToMapPosition(),
                index = i,
                visible = false,
                maped = false,
                L = 0,
                updateScheduled = true,
            

            };
            
            TileDropContainers[i] = new List<Entity>();
            TileGroundCoverContainers[i] = new List<Entity>();
        }
        // Engine.i.FOV.Clear();
        if(Application.isPlaying) UpdateTileVisibility();

    }

    public async static Task Update()
    {
        float startTime = UnityEngine.Time.realtimeSinceStartup;

        var job = new IndividualTileJob()
        {
            playerTile = PlayerAbilitiesSystem.playerEntity.CurrentTile(),
            current = current,
            next = next,
            FOVUpdateSchaduled = FOVUpdateScheduled,
            neiborOffsets = TileUpdater.NeiborsOffsetsArray8,
            playerViewDistance = PlayerAbilitiesSystem.playerEntity.GetComponentData<CreatureComponent>().viewDistance,
            maxDarcnessA = Engine.i.maxDarknessA,
        };
        
        

        var handle = job.Schedule();
        
        handle.Complete();

        current.CopyFrom(next);


        //Debug.Log((UnityEngine.Time.realtimeSinceStartup - startTime) * 1000f);
        if (FOVUpdateScheduled)
        {
            UpdateTileVisibility();
        }
        FOVUpdateScheduled = false;

        //if (RenderFrameScheduled)
        //{
            
        //    await Task.Delay(Engine.i.frameDrawInterval);
        //    RenderFrameScheduled = false;
        //}
        
        
    }
    public static void UpdateTileVisibility()
    {
        
        //startTime = Time.realtimeSinceStartup;
        for (int i = 0; i < current.Length; i++)
        {
            var item = current[i];
            
            if(item.updateScheduled) 
            {
                 Engine.i.FOV.SetPixel(item.position.x, item.position.y, new Color(1, 1, 1, (1-item.L)));
                if(item.visible)
                {
                    item.ShowUnmapable();
                }
                else
                {
                    Engine.i.FOV.SetPixel(item.position.x, item.position.y, new Color(1, 1, 1, (1-item.L)));
                    item.HideUnmapable();
                }
                item.updateScheduled = false;
                item.Save();


            }
                
        }
        Engine.i.FOV.Apply();
        //Player.i.cam.Render();
            
        //Debug.Log((UnityEngine.Time.realtimeSinceStartup - startTime) * 1000f);
        
    }

}

