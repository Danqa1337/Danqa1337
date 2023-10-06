using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Linq;
using System.Diagnostics;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public enum BodyPartTag
{
    None,
    Head,
    RightArm,
    LeftArm,
    RightFrontLeg,
    RightRearLeg,
    LeftFrontLeg,
    LeftRearLeg,
    RightFrontPaw,
    RightRearPaw,
    LeftFrontPaw,
    LeftRearPaw,
    Body,
    Tail,
    Tentacle,
    FirstTentacle,
    SecondTentacle,
    Fin,
    FirstFin,
    SecondFin,
    RightClaw,
    LeftClaw,
    Teeth,
    Fists,
    LowerBody,
}
public enum Direction
{
    U,
    D,

    R,
    L,

    UL,
    UR,

    DR,
    DL,

    Null

    
}
public enum Scaling 
{
    A, 
    B,
    C,
    D,
    _,
}
public enum EquipPice
{
    Weapon,
    Shield,
    Headwear,
    Chestplate,
    Boots,
    None,
}
public enum ReadinessLevel
{
    Sleeping,
    Loitering,
    OnAllert,
    FullyConcentrated,
}
public enum Team
{
    Friends,
    Enemies,
}
public enum Stance
{
    Free,
    Closed,
}
public enum HitType
{
    Normal,
    Crit,
}
public enum WeaponGrip
{
    High,
    Medium,
    Low,
}

public enum Gender
{
    Male,
    Female,
    It,
}
public enum DamageType
{
    Blunt,
    Piercing,
    Cutting,
    Shattering,
    Minor,
    Fire,
}

public enum Tag
{
    None,
    Player,

    Weapon,
    Chestplate,
    Headwear,
    Boots,
    Underwear,
    Food,
    Projectile,


    Cat,
    Rat,
    Dog,
    Human,

    Corpse,
    ImmuneToDismemberment,
    Immortal,
    Amoral,

    Rock,
    PrimitiveWeapon,
    Mushroom,
    Weed,
    Bloodless,
    Undead,
    Crafted,
    Bones,
    Head,
    Arm,
    Leg,
    Paw,
    Body,
    Tail,
    Tentacle,
    Container,
    Wood,
    Statue,
    Jumper,
    ResistFire,
    ResistCold,
    ResistPoison,
    ResistBleeding,
    ResistElectricity,
    Polearm,
    Solid,
    NonSolid,
    Drop,
    Explosive,
    Gold,
    Team1, Team2, Team3, Team4,

    Amphibious, Fish,
    Potion,
    Dishonored,
    RangedWeapon,
    Shield,
    Any,
    Wall,
    Liquid,
    Abyss,
    Merm,
    Humanoid,
    Floor,
    Mapable,
    Creature,
    Unmovable,
    Dummy,
    Flying,
    LOSblock,
    ExtraFragile,
    Digger,
    Immaterial
}
public enum TileState
{
    Floor,
    Wall,
    Door,
    ShallowWater,
    Darkness,
    Abyss,
    Passage,
    Any,

}
public enum Size : int
{
    Tiny = 0,
    Small = 1,
    Average = 2,
    Large = 3,
    Huge = 4,
}
public enum ObjectType
{
    Solid,
    Drop,
    Floor,
    Liquid,
    GroundCover,
    Hovering,
}



public class BaseMethodsClass : MonoBehaviour
{

    [HideInInspector]
    public static Vector3[] miroring = new Vector3[]
    {
        new Vector3(1,1,1),
        new Vector3(-1,1,1),
        new Vector3(1,-1,1),
        new Vector3(-1,-1,1),
    };
    public static Vector3 GetRandomMiroring(bool vetical = true)
    {
        if (vetical) return miroring[UnityEngine.Random.Range(0, 4)];
        else return miroring[UnityEngine.Random.Range(0, 2)];
    }
    private static Stopwatch stopwatch;

    public static void StartTest(string str = "")
    {
        if(str != "") Debug.Log(str);
        stopwatch = new Stopwatch();
        stopwatch.Start();

    }
    public static void EndTest(string str)
    {
        stopwatch.Stop();
        CheatConsole.print(str + stopwatch.ElapsedTicks + " ticks");
    }
    public static Quaternion GetRandomRotation(float r1 = 0, float r2 = 360)
    {
        return Quaternion.Euler(new Vector3(0, 0, UnityEngine.Random.Range(r1, r2)));
    }
    public static ISpawnDataByChance GetRandomSpawnData(List<ISpawnDataByChance> spawnDatas, int lvl)
    {
        float sum = 0;
        float iterator = 0;
        HashSet<ISpawnDataByChance> dataOfThatLevel = new HashSet<ISpawnDataByChance>();
        
        for (var index = 0; index < spawnDatas.Count; index++)
        {
            var spawnData = spawnDatas[index];
            float start = spawnData.normalDepthStart;
            float end = spawnData.normalDepthEnd;
            start = math.min(BaseMethodsClass.GenerateNormalRandom(start, 0.5f), start);
            end = math.max(BaseMethodsClass.GenerateNormalRandom(end, 0.5f), end);
            if (lvl >= start && lvl <= end)
            {
                sum += spawnData.baseSpawnChance;
                dataOfThatLevel.Add(spawnData);
            }

        }



        float rnd = UnityEngine.Random.Range(0.001f, sum);
        foreach (var item in dataOfThatLevel)
        {
            iterator += item.baseSpawnChance;
            if (rnd <= iterator) return item;
        }
        return null;

    }
    public static float GenerateNormalRandom(float mu, float sigma)
    {
        float rand1 = UnityEngine.Random.Range(0.0f, 1.0f);
        float rand2 = UnityEngine.Random.Range(0.0f, 1.0f);

        float n = Mathf.Sqrt(-2.0f * Mathf.Log(rand1)) * Mathf.Cos((2.0f * Mathf.PI) * rand2);

        return (mu + sigma * n);
    }

 

    //public Vector2 applyForce(RealTile origin, Thing thing, RealTile target )
    //{
    //    RaycastHit2D hit = Physics2D.Linecast(GetVector3FromVector2(origin.position), GetVector3FromVector2(target.position));


    //    if(hit.collider == null)
    //    {
    //        throwThing();
    //    }
    //    else
    //    {

    //        //for (int i = 0; i < power; i++)
    //        //{
    //        //    hit
    //        //}
    //    }

    //    void throwThing()
    //    {
    //        //if(thing.gameObject != 0)
    //        //{
    //        //    thing.gameObject.transform.position = GetVector3FromVector2(target.position);
    //        //}
    //        //else
    //        //{
    //        //    target.
    //        //}
    //    }

    //}

    public static bool Chance(float percent)
    {
        if (percent == 0) return false;
        if (percent == 100) return true;
        if (percent >= UnityEngine.Random.Range(0, 10000) * 0.01) return true;
        return false;

    }
    public float getPercent(float i, float percent)
    {

        return i * 0.01f * percent;
    }
    public float getPercentInt(int i, int percent)
    {
        return (i / 100) * percent;
    }


    //public PathNode GetNodeFromTile(Tile tile)
    //{
    //    bool walkable = false;

    //    if (tile.state == TileState.Floor)// && tile.SolidLayer == null)
    //    {
    //        walkable = true;
    //    }



    //    return new PathNode(walkable, tile.position.x, tile.position.y);
    //}
    static public Direction GetRandomDir(bool IncludeDiagonal, bool includeNull = false)
    {
        if (includeNull)
        {
            return (Direction)UnityEngine.Random.Range(0, 9);
        }
        else
        if (IncludeDiagonal)
        {
            return (Direction)UnityEngine.Random.Range(0, 8);
        }
        else
        {
            return (Direction)UnityEngine.Random.Range(0, 4);
        }
    }
    public List<TileData> GetMapBorderFromDirection(Direction direction)
    {
        switch (direction)
        {
            case Direction.U: 
                return GetAllMapTiles().Where(t => t.y == 63).ToList();
                break;
            case Direction.D: 
                return GetAllMapTiles().Where(t => t.y == 0).ToList();
                break;
            case Direction.R:
                return GetAllMapTiles().Where(t => t.x == 63).ToList();
                break;
            case Direction.L:
                return GetAllMapTiles().Where(t => t.x == 0).ToList();
                break;
            case Direction.UL:
                break;
            case Direction.UR:
                break;
            case Direction.DR:
                break;
            case Direction.DL:
                break;
            case Direction.Null:
                break;
            default:
                break;
        }
        return null;
    }

    public Direction GetDarknessDirection(TileData tile)
    {
        if (tile.checkDirectionState( Direction.D, TileState.Darkness)) return Direction.D;
        if (tile.checkDirectionState( Direction.U, TileState.Darkness)) return Direction.U;
        if (tile.checkDirectionState( Direction.R, TileState.Darkness)) return Direction.R;
        if (tile.checkDirectionState( Direction.L, TileState.Darkness)) return Direction.L;
        if (tile.checkDirectionState( Direction.D, TileState.Floor)) return GetOpositeDirection(Direction.D);
        if (tile.checkDirectionState( Direction.U, TileState.Floor)) return GetOpositeDirection(Direction.U);
        if (tile.checkDirectionState( Direction.R, TileState.Floor)) return GetOpositeDirection(Direction.R);
        if (tile.checkDirectionState( Direction.L, TileState.Floor)) return GetOpositeDirection(Direction.L);
        return Direction.Null;
    }
    //public RealTile GetTileToFlee(RealTile start, RealTile danger, int distance)
    //{

    //    RealTile tile = new RealTile();

    //    if(danger.X - start.X >= 0 && danger.X - start.X >=0) 
    //    if(danger.X - start.X >= 0 && danger.Y - start.Y <= 0)
    //    if(danger.X - start.X <= 0 && danger.Y - start.Y <= 0)
    //    if(danger.X - start.X >= 0 && danger.Y - start.Y >= 0)

    //                    do
    //    {
    //        tile = GetTileDataFromVector(new Vector2Int(UnityEngine.Random.Range(danger.X), )
    //    }
    //    while
    //}


    public ImpactTile GetStraightTraectory(TileData startTile, TileData targetTile, float force)
    {
        //ImpactTile impactTile;
        //if (startTile == null || targetTile == null) return null;
        //if (force < 1 && force >= 0)
        //{
        //    impactTile = new ImpactTile(startTile);
        //    return impactTile;
        //}



        //force = Mathf.Clamp(force, 0, (targetTile.position - startTile.position).SqrMagnitude());
        //print("force: " + force);
        //float2 poltorashka = new float2(0.5f, 0.5f);
        //Vector2 start = startTile.position + poltorashka;
        //Vector2 step = math.normalize((float2)targetTile.position - startTile.position);
        //Vector2 current = start;
        //Vector2 next = start;




        //for (int i = 0; i < force; i++)
        //{  
        //    next = current + step;
        //    if(GetTileDataFromVector(GetVector2IntFromVector2(next)).SolidLayer != null && next!= start)
        //    {
        //       // UnityDebug.DrawLine(start, next, Color.red);
        //        TileData tileToDropItemOn = null;
        //        if(GetTileDataFromVector(GetVector2IntFromVector2(next)).SolidLayer.GetComponent<Creature>())
        //        {
        //            tileToDropItemOn = GetTileDataFromVector(GetVector2IntFromVector2(next));
        //        }
        //        else
        //        {
        //            tileToDropItemOn = GetTileDataFromVector(GetVector2IntFromVector2(current));
        //        }
        //        return new ImpactTile(tileToDropItemOn, GetTileDataFromVector(GetVector2IntFromVector2(next)).SolidLayer);
        //    }
        //    current = next;
        //}
        ////Debug.DrawLine(start, current, Color.red);
        return null;//new ImpactTile(GetTileDataFromVector(GetVector2IntFromVector2(current)));
     }
    public ImpactTile GetPorabolicTraectory(TileData startTile, TileData targetTile, float force)
    {
        //    ImpactTile impactTile;
        //    if (startTile == null || targetTile == null) return null;
        //    if (force < 1 && force >= 0)
        //    {
        //        impactTile = new ImpactTile(startTile);
        //        return impactTile;
        //    }



        //    force = Mathf.Clamp(force, 0, (targetTile.position - startTile.position).Magnitude());
        //    // Debug.Log("force: " + force);
        //    float2 poltorashka = new float2(0.5f, 0.5f);
        //    float2 start = startTile.position + poltorashka;
        //    float2 step = math.normalize((float2)targetTile.position - startTile.position);
        //    float2 current = start;
        //    float2 next = start;




        //    for (int i = 0; i < force; i++)
        //    {
        //        next = current + step;
        //        if (GetTileDataFromVector(GetVector2IntFromVector2(next)).SolidLayer != null && next.Equals(start))
        //        {
        //          //  Debug.DrawLine(start, next, Color.red);
        //            return new ImpactTile(GetTileDataFromVector(GetVector2IntFromVector2(current)), GetTileDataFromVector(GetVector2IntFromVector2(next)).SolidLayer);
        //        }
        //        current = next;
        //    }
        ////    Debug.DrawLine(start, current, Color.red);
        return null;//new ImpactTile(GetTileDataFromVector(GetVector2IntFromVector2(current)));
    }


    public static TileData GetRadomTile()
    {
        return TileUpdater.current.RandomItem();
    }
    public static TileData GetFreeTile()
    {
        TileData tile = GetRadomTile();

        while (tile.SolidLayer != Entity.Null)
        {
            tile = GetRadomTile();
        }

        return tile;
    }
    public static TileData GetRadomTile(TileState state)
    {
        return GetAllMapTiles().Where(t => t.template.tileState == state).ToList().RandomItem();
    }
    public static TileData GetRadomTileData(TileState state)
    {
        return TileUpdater.current.Where(t => t.template.tileState == state).ToList().RandomItem();
    }

    //public static TileData GetTileDataFromVector(float x, float y)
    //{
    //    int i = GetIndexFromVector(new float2(x, y));
    //    if (i == -1) return TileData.Null;
    //    return TileUpdater.current[i];
    //}


    
    public static TileData GetNearestTileFromList(List<TileData> tilesList, TileData toTile)
    {
        float dst = 0;
        TileData tile = new TileData();
        for (int i = 0; i < tilesList.Count; i++)
        {
            float newDst = tilesList[i].GetSqrDistance(toTile);
            if (i == 0 || newDst < dst)
            {
                dst = newDst;
                tile = tilesList[i];
            }
        }
        return tile;
    }

    public List<Direction> getListOfDirections(int howMany)
    {
        List<Direction> directions = new List<Direction>();
        for (int i = 0; i < howMany; i++)
        {
            directions.Add((Direction)i);
        }
        return directions;
    }
    public static bool TagListcontainsTag(Tag _tag, List<Tag> _tags)
    {
        foreach (Tag tag in _tags)
        {
            if (tag == _tag) return true;

        }
        return false;
    }
    public static  bool TaglistsInterpolates(List<Tag> _tags1, List<Tag> _tags2)
    {
        if(_tags1 == null || _tags2 == null) return false;
        if (_tags1.Count == 0 || _tags2.Count == 0) return false;
        List<Tag> tags = _tags1.Intersect(_tags2).ToList();
        if (tags.Count > 0) return true;
        return false;
    }
    public static bool TaglistIncludesTagList(List<Tag> main, List<Tag> child)
    {
        if (main == null ||main.Count == 0) return false;
        if (child == null || child.Count == 0) return true;
        foreach (Tag childtag in child)
        {

           if(!main.Contains(childtag) && childtag != Tag.Any) return false;
        }
        return true;
    }
    //public List<RealTile> GetTilesInArea(Vector2Int center()) 
    public static List<ISpawnDataByBiome> GetSpawnDatasOfBiome(List<ISpawnDataByBiome> spawnDatas, Biome biome)
    {
        return spawnDatas.Where(c => biome == Biome.Any || c.biomes.Contains(Biome.Any) || c.biomes.Contains(biome)).ToList();
    }

    //public void SwapSolidObjects(Thing object1, Thing object2)
    //{
    //    TileData tile1 = object1.CurrentTile;
    //    TileData tile2 = object2.CurrentTile;

    //    object2.MovePosition(new int2(1, 1).ToTileData());
    //    object1.MovePosition(tile2);
    //    object2.MovePosition(tile1);
    //}
    
    public static HashSet<TileData> GetTilesInRadius(TileData center, int radius)
    {
        radius++;
        HashSet<TileData> tiles = new HashSet<TileData>();
        for (int x = center.position.x -radius; x < center.position.x + radius; x++)
        {
            for (int y = center.position.y -radius; y < center.position.y + radius; y++)
            {
                if ((new int2(x,y) - center.position ).SqrMagnitude() < Math.Pow(radius, 2) )
                {
                    TileData newTile = new int2(x, y).ToTileData();
                    if(newTile != TileData.Null) tiles.Add(newTile);
                   
                }
                
            }
        }
        
       
       
        return tiles;
    }
    public static NativeList<TileData> GetTilesInRadiusNative(TileData center, int radius)
    {
        radius++;
        float sqrRadius = radius * radius;
        NativeList<TileData> tiles = new NativeList<TileData>((int)sqrRadius, Allocator.Temp);
        int X = center.position.x + radius;
        int Y = center.position.y + radius;


        for (int x = center.position.x -radius; x < X; x++)
        {
            for (int y = center.position.y -radius; y < Y; y++)
            {
                if ((new int2(x,y) - center.position ).SqrMagnitude() < sqrRadius)
                {
                    TileData newTile = new int2(x, y).ToTileData();
                    if(newTile != TileData.Null) tiles.Add(newTile);
                   
                }
                
            }
        }
        
       
       
        return tiles;
    }
    public static TileData[] GetAllMapTiles()
    {
        return TileUpdater.current.ToArray();
    }
    public HashSet<TileData> GetTilesInRectangle(TileData center, Vector2Int size)
    {
        HashSet<TileData> tiles = new HashSet<TileData>();
        for (int x = center.position.x - size.x/2; x < center.position.x + size.x/2; x++)
        {
            for (int y = center.position.y - size.y/2; y < center.position.y + size.y/2; y++)
            {
                TileData tile = new int2(x, y).ToTileData(); 
                tiles.Add(tile);
            }
        }
        return tiles;
    }
    public HashSet<TileData> GetTilesInRectangle(int2 leftCorner, int2 rightCorner)
    {
        HashSet<TileData> tiles = new HashSet<TileData>();
        for (int x = leftCorner.x; x < rightCorner.x; x++)
        {
            for (int y = leftCorner.y; y < rightCorner.y; y++)
            {
                tiles.Add(new int2(x, y).ToTileData());
            }
        }



        return tiles;
    } 
    public HashSet<TileData> GetTilesInRectangle(TileData leftCorner, TileData rightCorner)
    {
        HashSet<TileData> tiles = new HashSet<TileData>();
        for (int x = leftCorner.x; x < rightCorner.x; x++)
        {
            for (int y = leftCorner.y; y < rightCorner.y; y++)
            {
                tiles.Add(new int2(x, y).ToTileData());
            }
        }



        return tiles;
    }
    public List<TileData> GetTilesInRectangleList(int2 leftCorner, int2 rightCorner)
    {
        List<TileData> tiles = new List<TileData>();
        for (int x = leftCorner.x; x < rightCorner.x; x++)
        {
            for (int y = leftCorner.y; y < rightCorner.y; y++)
            {
                tiles.Add(new int2(x, y).ToTileData());
            }
        }



        return tiles;
    }

    public List<TileData> GetTilesInRectangle(TileData center, int sizex, int sizey)
    {
        List<TileData> tiles = new List<TileData>();
        for (int x = center.position.x - sizex / 2; x < center.position.x + sizex / 2; x++)
        {
            for (int y = center.position.y - sizey / 2; y < center.position.y + sizey / 2; y++)
            {

                TileData tile = new int2(x, y).ToTileData();
                if (tile != null) tiles.Add(tile);



            }
        }



        return tiles;
    }
   
    public Direction GetOpositeDirection(Direction direction)
    {
        switch (direction)
        {
            case Direction.U:
                return Direction.D;
            case Direction.R:
                return Direction.L;
            case Direction.L:
                return Direction.R;
            case Direction.D:
                return Direction.U;
        }
        return Direction.U;
    }
    public static EquipPice BodypartToEquipPice(BodyPartTag bodyPartTag) => (bodyPartTag) switch
    {
        BodyPartTag.None => EquipPice.None,
        BodyPartTag.Head => EquipPice.Headwear,
        BodyPartTag.RightArm => EquipPice.Weapon,
        BodyPartTag.LeftArm => EquipPice.Shield,
        BodyPartTag.LeftFrontLeg => EquipPice.Boots,
        BodyPartTag.RightFrontLeg => EquipPice.None,
        BodyPartTag.Body => EquipPice.Chestplate,
        BodyPartTag.Tail => EquipPice.None,
        BodyPartTag.Tentacle => EquipPice.None,
        BodyPartTag.Fin => EquipPice.None,
    };





    
    //public bool Collide(Thing thing1, Thing thing2, Thing atacker = null)
    //{
    //    if (thing1 != null && thing2 != null)
    //    {
    //        if (thing1.isThereACollision(thing2, atacker) && thing2.isThereACollision(thing1, atacker))
    //        {
    //            print(thing1.name + " collided with " + thing2.name + " with reletive speed of " +(thing1.V + thing2.V)) ;
    //            if (thing1.V > thing2.V)
    //            {
    //                thing1.TakeHit(thing2);
    //                thing2.TakeHit(thing1);
    //            }
    //            else
    //            {
    //                thing2.TakeHit(thing1);
    //                thing1.TakeHit(thing2);
    //            }

    //            return true;
    //        }
    //    }
    //    //if(thing1.OnReciveingABlow((int)(F * thing2.sharpness * generateNormalRandom(1, 0.05f)) , atacker, thing2.damageType) & thing2.OnReciveingABlow((int)(F * thing1.sharpness * generateNormalRandom(1, 0.05f)), atacker, thing2.damageType) )
    //    //{
    //    //    return true;
    //    //}
    //    return false;

    //}
    public void SheludeDelayedAction(Action action, int delay)
    {
        
    }
    public void AddEvent(GameObject obj, EventTriggerType type, UnityAction<BaseEventData> action)
    {
    EventTrigger trigger = obj.GetComponent<EventTrigger>();
    var eventTrigger = new EventTrigger.Entry();
    eventTrigger.eventID = type;
    eventTrigger.callback.AddListener(action);
    trigger.triggers.Add(eventTrigger);
}
    public void SwapSlots(InventorySlot slot1, InventorySlot slot2)
    {
        if (slot1 == slot2) return;
        if (slot1 == null || slot2 == null) return;
        if (slot1.CanPlaceInSlot(slot2.currentItem) && slot2.CanPlaceInSlot(slot1.currentItem))
        {
            var itemFromSlot2 = slot2.currentItem;
            var itemFromSlot1 = slot1.currentItem;
            slot1.Clear();
            slot2.Clear();

            
            slot1.PlaceItemFromSlot(itemFromSlot2);
            slot2.PlaceItemFromSlot(itemFromSlot1);
            


        }

    }
    public static List<T> DecodeCharSeparatedEnums<T>(string charSeparatedEnum, char separator = ',')
    {
        List<T> enums = new List<T>();
        if (charSeparatedEnum.Length > 0)
        {
            foreach (string str in charSeparatedEnum.Split(separator))
            {
                bool enumFound = false;
                foreach (T e in Enum.GetValues(typeof(T)))
                {

                    if (e.ToString() == str)
                    {
                        //Debug.Log(e.ToString());
                        enums.Add(e);
                        enumFound = true;
                        break;
                    }
                }

                if (!enumFound) throw new NullReferenceException("There is no such name " + str + " inside " + typeof(T).ToString() + " enum");

            }
        }
        return enums;

    }
}


public class ImpactTile
{
    public ImpactTile(TileData _tile)
    {
        tile = _tile;
    }
    public TileData tile;
    public Entity ColidedObject;
}
