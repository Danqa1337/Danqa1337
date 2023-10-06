using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Entities;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public abstract class GenerationPreset : BaseMethodsClass
{


    public int CavesChance;
    public float floodCaveChance;
    public int minCaveSizeToFlood;
    public float riverChance;
    public int minCahsmSize;
    public int minRegionSize;
    public float ChasmChance;
    public float secretRoomsNum;
    public int fitRoomTryLimit;
   



    [Range(0, 0.02f)]
    public float enemyOccurence;
    [Range(0, 0.01f)]
    public float itemOccurence;
    public int roomNum;
    public int trapNum;
    public bool turnRooms = true;
    public int pathTurnChance;
    public Location location;
    protected List<TileData> arcs;
    
    [Range(0, 10)] public float automatonLacunarity;
    [Range(0, 10)] public float automatonPersistance;
    public float automatonBorderFactor;
    public int scale;
    public float automatonWallPercent;
    protected List<Region> Regions;

    protected virtual void GenerateAsync()
    {

        
    }
    public void Generate()
    {
        
        
        Stopwatch globalStopWatch = new Stopwatch();
        globalStopWatch.Start();
        location = MapGenerator.i.location;



            InitializeSystems();
        
        GenerateAsync();


        globalStopWatch.Stop();
        
        print("Generation process completed and took " + globalStopWatch.ElapsedMilliseconds + " ms");
        if (Application.isPlaying)
        {
           // if (LoadingScreen.i != null) LoadingScreen.i.EndLoading();
            DungeonStructureVisualizer.i.UpdateCurrentLocationInfo();
            PlayerAbilitiesSystem.controlled = true;
            TileUpdater.FOVUpdateScheduled = true;
            World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<SeamlessTextureSystem>().Update();
            Controller.controllerState = Controller.ControllerState.movement;
            TimeSystem.NewTick();
        }

    }
    protected void InitializeSystems()
    {
        TileUpdater.Init();
    }
    public void PopulateRegions()
    {
        foreach (var tile in GetAllMapTiles())
        {
            if (tile.template.tileState == TileState.Floor)
            {
                if (tile.template.Biome == Biome.Cave)
                {

                    if (Chance(50))
                    {
                        tile.template.GenerateObject("Moss");
                        continue;
                    }

                    if (Chance(30) && tile.CheckStateInNeibors(TileState.Darkness, true))
                    {


                        int f = 0;
                        foreach (var item in tile.GetNeibors(true))
                        {
                            if (item.template.tileState == TileState.Floor) f++;
                        }
                        if (f > 4) tile.template.GenerateObject("Stalagmite");
                        continue;
                    }

                    if (Chance(5) && tile.CheckStateInNeibors(TileState.Floor, true, 4))
                    {
                        tile.template.GenerateObject("Mushroom");
                    }
                    if (Chance(10) && tile.CheckStateInNeibors(TileState.Floor, true, 4))
                    {
                        tile.template.GenerateObject("QuarryBush");
                    }
                }
                else if (tile.biome == Biome.Dungeon && !tile.CheckStateInNeibors(TileState.Wall, false))
                {
                    if (Chance(2))
                    {
                        tile.template.GenerateObject("Vase");
                    }
                }
            }
        }
    }
    public void GenerateSecretRooms()
    {
        StartTest();
        List<RoomSpawnData> roomDatas = new List<RoomSpawnData>();
        roomDatas.Add(RoomDatabase.GetRoom("SecretRoom1"));
        roomDatas.Add(RoomDatabase.GetRoom("SecretRoom2"));
        roomDatas.Add(RoomDatabase.GetRoom("SecretRoom3"));

        RoomSpawnData roomData = roomDatas.RandomItem();
        int roomSize = roomData.GetSize();
        
       
        for (int j = 0; j < 1000; j++)
        {
            TileData center = GetAllMapTiles().RandomItem();
            if (canfitRoom(center, roomSize))
            {
               
                Room room = PlaceRoom(center, roomData);
                PathFinderPath way = ConnectRegionsWithPath(Regions[0],room);
                TurnWayIntoSecretCoridor(way);
                print("secretRoomGenerated On "+ center.position);
                break;
            }
        }
        EndTest("Secret rooms generation took ");
    }

    public bool debugStairs;
    public virtual void GenerateStairs()
    {
        
        foreach (var id in location.transitionsIDs)
        {
            var transition = DungeonStructure.GetTransition(id);




            if (debugStairs)
            {
                if (transition.exposed)
                {
                    var tile = GetRadomTile(TileState.Floor);

                    //TileData tile = GetAllMapTiles().Where(t => t.template.tileState == TileState.Floor).ToList()
                    //    .RandomItem();

                    var newTile = tile;
                
                    Entity stairsEntity;
                    if (transition.LocationFromID == location.id)
                    { 
                        newTile = tile + new int2(1, 0);

                   
                        stairsEntity = Spawner.Spawn("StairsDown", newTile.position);
                        transition.positionFrom = newTile.position;

                    }
                    else
                    { 
                    
                        newTile = tile+ new int2(-1, 0);
                        stairsEntity = Spawner.Spawn("StairsUp", newTile.position);
                        transition.positionTo = newTile.position;
                    
                    }
                    stairsEntity.AddComponentData(new StairsComponent(){transitionId = transition.id});

                    var template = newTile.template;
                    template.SolidLayer = -1;
                    template.FloorLayer = -1;
                    template.tileState = TileState.Passage;
                    newTile.template = template;
                    Debug.Log("Generated stairs on " + tile.position );
                }
            }
            else
            {
                if (transition.exposed)
                {

                    TileData tile = GetAllMapTiles().Where(t => t.template.tileState == TileState.Floor).ToList().RandomItem();



                    Entity stairsEntity;
                    if (transition.LocationFromID == location.id)
                    {
                        stairsEntity = Spawner.Spawn("StairsDown", tile.position);
                        transition.positionFrom = tile.position;

                    }
                    else
                    {
                        stairsEntity = Spawner.Spawn("StairsUp", tile.position);
                        transition.positionTo = tile.position;

                    }

                    var template = tile.template;
                    template.SolidLayer = -1;
                    template.FloorLayer = -1;
                    template.LiquidLayer = -1;
                    tile.template = template;
                    Debug.Log("Generated stairs on " + tile.position);
                }
            }
            
        }

    }

    public virtual void ApplyTiles()
    {
        foreach (var item in TileUpdater.current)
        {
            item.ApplyTemplate();
        }
    }
    public virtual void SpawnPlayer()
    {       

        TileData tile = GetRadomTileData(TileState.Floor);
        if(debugStairs)
        {
            foreach (var id in location.transitionsIDs)
            {
                if (DungeonStructure.GetTransition(id).LocationFromID == location.id)
                {
                    tile = DungeonStructure.GetTransition(id).positionFrom.ToTileData();
                    break;
                }
            }
        }
        foreach (var id in location.transitionsIDs)
        {
            if (DungeonStructure.GetTransition(id).LocationToID == location.id)
            {
                tile = DungeonStructure.GetTransition(id).positionTo.ToTileData();
                break;
            }
        }


        Spawner.SpawnPlayer(tile.position);
        print("spawned Player on " + tile.position);

        
    }

    protected int GetFloorTilesNum()
    {
        return GetAllMapTiles().Where(t => t.template.SolidLayer == -1).ToArray().Length;
    }
    public virtual void GenerateAbyssSides()
    {
        foreach (var item in GetAllMapTiles().Where(t=>t.template.tileState == TileState.Abyss))
        {
            item.template.MakeAbyss();
            
        }
        var tiles = GetAllMapTiles().Where(t => t.template.FloorLayer == -1 && (t + Direction.U).template.FloorLayer != -1);
        foreach (var tile in tiles)
        {
            tile.template.GenerateObject("AbyssSide");
        }
            
    }
    public virtual void SpawnCreatures()
    {
        StartTest();
        int e = 0;
        var names = new List<string>();

        for (int i = 0; i< enemyOccurence * GetFloorTilesNum(); i++)
        {
            e++;
            TileData tile = GetAllMapTiles().Where(t => t.template.SolidLayer == -1 && t.SolidLayer == Entity.Null && t.template.isFloor()).ToList().RandomItem();
            
            var creatureData = ObjectDataFactory.GetRandomCreature(location.level, tile.biome, withRandomEquip: true);
            names.Add(creatureData.staticData.name);
            Spawner.Spawn(creatureData,tile.position);
            continue;
            ;
            List<TileData> neibors = GetTilesInRadius(tile, 3).ToList();

            //neibors.Shuffle();

            //for (int q = 0; q < creatureData.flockSize; q++)
            //{
            //    for (int n = 0; n < neibors.Count; n++)
            //    {
            //        if (neibors[n].SolidLayer != null || neibors[n].template.tileState != TileState.Floor)
            //        {
            //            neibors.Remove(neibors[n]);
            //        }
            //        else
            //        {
            //            i++;

            //            e++;
                        
            //            break;
            //        }
            //    }
            //}
        }
        var message = e + " mobs spawned: ";
        foreach (var item in names)
        {
            message += ", " + item;
        }
        CheatConsole.print(message);
        EndTest("Spawning Creatures took ");
    }
    public virtual void spawnItems()
    {
        
        for (int i = 0; i <  itemOccurence * GetFloorTilesNum(); i++)
        { 
            TileData tile = GetAllMapTiles().Where(t=> t.template.SolidLayer == -1 && t.template.isFloor()).ToList().RandomItem();

           tile.template.GenerateObject(ItemDataBase.GetRandomItem(location.level, tile.biome, new List<Tag>(){Tag.Drop}).name);
        }
    }
    
    public virtual void FillDarkness()
    {
        
        StartTest();
        foreach (var item in GetAllMapTiles().Where(t => t.template.tileState == TileState.Darkness))
        {
            if (depthMap[item.position.x, item.position.y] > 0.7)
            {
                item.template.GenerateObject("MarbleWall");
            }
            else
            {
                item.template.GenerateObject("RockWall");
            }
            item.SetBiome(Biome.Wall);
        }
  
        EndTest("Filling darkness took ");
    }
    public virtual void GenerateCoridors()
    {
        StartTest();
        placeArcs();

        foreach (var Arc in arcs)
        {
            PathFinderPath path = GeneratePath(Arc);
            if (path != PathFinderPath.NUll)
            {
                if (Chance(20) && path.waySide.Any(t =>
                t.template.tileState != TileState.Darkness
                && !(t.GetNeibors(false).Contains(path.startTile) || t.GetNeibors(false).Contains(path.targetTile))))
                {
                    ExpandPassage(path, 100);
                }
                if (Chance(30))
                {
                   // TurnWayIntoMarbleCoridor(path);
                }
                else
                {
                    TurnPathIntoBrickCoridor(path);
                }
            }
        }

        PathFinderPath GeneratePath(TileData arc)
        {

            List<TileData> pathTiles = new List<TileData>();
            Direction currentDirection = GetDarknessDirection(arc);
               

            Direction lastDirection = currentDirection;
            TileData currentTile = arc + currentDirection;
            TileData nextTile = currentTile + currentDirection;
            pathTiles.Add(currentTile);

            while (true)
            {
                if(pathTiles.Count != 1)
                {
                    foreach (var item in currentTile.GetNeibors(false))
                    {
                        if (item.template.isFloor())
                        {
                            return new PathFinderPath(pathTiles);
                        }
                            
                    }

                }


                    

                if (itsTimeForForcedTurn() || itsTimeForSpantanTurn())
                {

                    List<Direction> posibleDirections = new List<Direction>() { Direction.U, Direction.D, Direction.R, Direction.L };

                    posibleDirections.Remove(lastDirection);
                    posibleDirections.Remove(GetOpositeDirection(lastDirection));
                    posibleDirections.Remove(GetOpositeDirection(currentDirection));
                    posibleDirections.Remove(currentDirection);


                    currentDirection = posibleDirections.RandomItem();
                    lastDirection = currentDirection;
                }


                currentTile = currentTile + currentDirection;
                nextTile = currentTile + currentDirection;
                pathTiles.Add(currentTile);

                if (nextTile.isBorderTile())
                {
                    cancelPath();
                    return PathFinderPath.NUll;
                }
            }
            
            bool itsTimeForSpantanTurn()
            {
                if(Chance(pathTurnChance) && pathTiles.Count > 3 && pathTiles.Count % 2 == 0)
                {
                    return true;
                }
                return false;
            }
            bool itsTimeForForcedTurn()
            {
                if (nextTile.isBorderTile() || nextTile.template.tileState == TileState.Abyss) return true;
                return false;
            }
            void cancelPath()
            {
                arc.template.GenerateObject("BrickWall");

                //foreach (var item in arc.GetNeibors(false))
                //{
                //    if (item.template.tileState == TileState.Wall)
                //    {
                //        arc.template.GenerateObject(item.template.SolidLayer); 
                //        break;
                //    }
                //}
                arc.template.SetState(TileState.Wall);

            }
            
        }
        EndTest("Coridors placement took ");
    }
    protected void TurnPathIntoBrickCoridor(PathFinderPath path)
    {
        if (path != PathFinderPath.NUll)
        {
            foreach (var item in path.waySide.Where(t => t.template.tileState == TileState.Darkness))
            {
                item.template.GenerateObject("BrickWall");
            }

            foreach (var item in path.tiles)
            {
                item.template.Clear();
                item.template.GenerateObject("BrickFloor");
                item.SetBiome(Biome.Dungeon);
            }
        }
    }
    protected void TurnWayIntoMarbleCoridor(PathFinderPath way)
    {
        foreach (var item in way.waySide.Where(t => t.template.tileState == TileState.Darkness))
        {
            item.template.Clear();
            item.template.GenerateObject("MarbleWall");
        }
        foreach (var item in way.tiles)
        {
            item.template.Clear();
            item.template.GenerateObject("MarbleFloor");
            item.SetBiome(Biome.Dungeon);

        }
    }
    protected void TurnWayIntoSecretCoridor(PathFinderPath way)
    {
       // HashSet<TileData> tiles = new HashSet<TileData>(path.tiles.Concat(path.tiles));
        foreach (var item in way.tiles)
        {
            //item.GenerateObject("BrickFloor");
            //item.state = TileState.NaturalFloor;
        }

        foreach (var tile in way.tiles)
        {
            foreach (var neibor1 in tile.GetNeibors(true))
            {
                if (neibor1.isWalkable(PlayerAbilitiesSystem.playerEntity) && !way.tiles.Contains(neibor1) && tile != way.targetTile && Regions[0].tiles.Contains(neibor1))
                {
                   
                    if (tile.CheckStateInNeibors(TileState.Wall, true))
                    {
                       // wall = //tile.GetNeibors(true).First(t => t.SolidLayer != null).SolidLayer;
                    }

                    tile.template.GenerateObject("RockWall");
                   // tile.SolidLayer.currentHp = 1;
                    //tile.SolidLayer.maxHp = 1;
                    break;
                }
            }
        }

        //foreach (var item in path.waySide.Where(t => t.state == TileState.Darkness))
        //{
        //    item.GenerateObject("MarbleWall");
        //}
        //foreach (var item in path.tiles.Where(t => t.state == TileState.Darkness))
        //{
        //    item.GenerateObject("MarbleFloor");
        //    if (item.SolidLayer != null) item.Clear();
        //}

    }
    protected void TurnPathIntoCavePassage(PathFinderPath way)
    {
        foreach (var item in way.tiles)
        {
            //if (item != way.targetTile)
            {
                item.template.Clear();
                item.template.GenerateObject("RockFloor");
            }
        }

    }
    protected void ExpandPassage(PathFinderPath path, float expandingChance)
    {


        foreach (var item in path.waySide)
        
        {
            if(Chance(expandingChance))
            {
                //if (item != TileData.Null && !item.GetNeibors(true).Contains(path.startTile)
               //     && !item.GetNeibors(true).Contains(path.targetTile))
                {
                    //if (item.template.tileState == TileState.Darkness)
                    {
                        path.tiles.Add(item);
                    }
                }
            }
            
        }

    }
    
    public virtual void GenerateRooms()
    {
        StartTest();

        


        List<RoomSpawnData> roomSpawnDatas = new List<RoomSpawnData>();
        for (int i = 0; i < roomNum; i++)
        {
            roomSpawnDatas.Add(RoomDatabase.GetRandomRoom(location.generationPreset, location.level, 64));





        }
        roomSpawnDatas.Shuffle() ; //OrderBy(r => 64-r.GetSize()).ToList();
        

        for (int i = 0; i < roomNum; i++)
        {      
            var freeTiles = GetAllMapTiles().Where(t => t.template.tileState == TileState.Darkness).ToList();
            var leftCorner = freeTiles.RandomItem();


            foreach (var data in roomSpawnDatas)
        {       

                if (canfitRoom(leftCorner, math.max(data.width, data.height) ))
                {
                    PlaceRoom(leftCorner, data);
                    break;
                }
                
            }
            roomSpawnDatas.Shuffle();
        }


        //foreach (var room in roomSpawnDatas)
        //{
        //    int roomSize = room.GetSize();
        //    var freeTiles = GetAllMapTiles().Where(t => t.template.tileState == TileState.Darkness && t.GetDistanceFromEdge() > roomSize * 0.5f + 2).ToList();
        //    if (freeTiles.Count > 0)
        //    {
        //        for (int j = 0; j < fitRoomTryLimit; j++)
        //        {
        //            TileData center = freeTiles.RandomItem();

        //            if (canfitRoom(center, roomSize))
        //            {
        //                PlaceRoom(center, room);
        //                break;
        //            }
        //        }
        //    }
        //}

        EndTest("Room placing took ");
    }
    
    public void DefineRegions()
    {
        StartTest();
        Regions = new List<Region>();
        var closedSet = new NativeList<TileData>( 1024, Allocator.Temp);
        var OpenList = new NativeList<TileData>(1024, Allocator.Temp);
        HashSet<TileData> freeTiles = new HashSet<TileData>();
        foreach (var VARIABLE in  GetAllMapTiles().Where(t => t.template.isFloor()))
        {
            freeTiles.Add(VARIABLE);
        }
        
        while (freeTiles.Count > 0)
        {
            closedSet.Clear();
            OpenList.Clear();
            Region newRegion = new Region();
            TileData newTile = freeTiles.First();

            OpenList.Add(newTile);

            while (OpenList.Length > 0)
            {
                newTile = OpenList[0];
                freeTiles.Remove(newTile);
                OpenList.Remove(newTile);
                newRegion.tiles.Add(newTile);

                closedSet.Add(newTile);

                foreach (var item in newTile.GetNeibors(false))
                {
                    if (freeTiles.Contains(item)) OpenList.Add(item);
                }

            }
            //  print("A Region defined with size of " + newRegion.tiles.Count + " tiles");
            Regions.Add(newRegion);


        }

        closedSet.Dispose();
        OpenList.Dispose();
        EndTest("Defining regions took ");

        print(string.Format("Defined {0} regions", Regions.Count));
    }
    public virtual void RemoveBadRegions()
    {
        int r = 0;
        var badRegions = new List<Region>();
        for (int i = 0; i < Regions.Count; i++)
        {
            if(Regions[i].size < minRegionSize)
            {
                badRegions.Add(Regions[i]);
               
                
            }
        }

        foreach (var region in badRegions)
        {
            r++;
            Regions.Remove(region);
            foreach (var item in region.tiles)
            {
                item.template.Clear();
            }
        }

        

        Debug.Log(r + " regions removed");
    }

    public void CreateRiver()
    {
        if (Chance(riverChance))
        {
            var direction1 = GetRandomDir(false);
            var direction2 = GetOpositeDirection(direction1);

            var start = GetMapBorderFromDirection(direction1).RandomItem();
            var end = GetMapBorderFromDirection(direction2).RandomItem();


            var path = Pathfinder.FindPath(start, end, true, 10000);

            path.tiles.Add(end);
            
            ExpandPassage(path, 100);
            ExpandPassage(path, 80);


            foreach (var item in path.tiles)
            {
                item.template.GenerateObject("BlackSandFloor");
            }

            foreach (var tile in path.tiles)
            {

                
                tile.template.GenerateObject("ShallowWater");
                
                tile.SetBiome(Biome.FloodedCave);
            }
        }
    }
    public virtual void TurnRegionIntoMushroomForest(Region region)
    {
        foreach (var tile in region.tiles)
        {
            if(Chance(25))
            {
                Spawner.Spawn("Mushroom", tile.position);
            }
        }
    }

    public virtual void DefineBiomes()
    {
        foreach (var region in Regions)
        {
            if(Chance(floodCaveChance) && region.size > minCaveSizeToFlood)
            {
                
                foreach (var item in region.tiles)
                {
                    item.template.GenerateObject("BlackSandFloor");
                    if (item.template.tileState == TileState.Floor
                        && !item.CheckStateInNeibors(TileState.Darkness, true)
                        && depthMap[item.position.x, item.position.y] < 0.3f)
                    {
                        item.template.GenerateObject("ShallowWater");


                    }
                    item.SetBiome(Biome.FloodedCave);
                    
                }
                foreach (var item in region.tiles)
                {

                    if (!item.CheckStateInNeibors(TileState.Floor, true) && depthMap[item.position.x, item.position.y] < 0.2f)
                    {
                        item.template.SetState(TileState.Abyss);
                    }
                }

                
                
            }
            else if (region.size > minCahsmSize && Chance(ChasmChance))
            {

                foreach (var item in region.tiles)
                {
                    item.SetBiome(Biome.Chasm);
                    if (item.template.Biome == Biome.Cave && !item.CheckStateInNeibors(TileState.Darkness, true))
                    {
                        item.template.SetState(TileState.Abyss);
                    }
                }
            }
           
        }
    }

    protected void SpawnGeisers()
    {
        StartTest();
        List<TileData> tiles = GetAllMapTiles().Where(t => t.template.tileState == TileState.Floor && t.GetDistanceFromEdge() > 5).ToList();
     
        foreach (var tile in tiles)
        {
            HashSet<TileData> tilesInRadius = GetTilesInRadius(tile, 3);
            bool radiusIsFree = true;
            foreach (var neibor in tilesInRadius)
            {
                if (neibor.SolidLayer != null)
                {
                    radiusIsFree = false;
                    break;
                }
            }
            if(radiusIsFree)
            {
                tile.template.GenerateObject("Geiser");
                
                foreach (var neibor in tilesInRadius)
                {
                   if(!tile.GetNeibors(true).Contains(neibor) && neibor.template.tileState == TileState.Floor && neibor != tile && Chance(80)) neibor.template.GenerateObject("ShallowWater");                }
                break;
            }

        }
        EndTest("Geiser spawning took ");
        

    }
    public virtual void ConnectRegions()
    {
     
        StartTest();
        
        while(Regions.Count>1)
        {

            Region currentRegion = Regions[0];
            float minDistance = float.MaxValue;
            Region nearestRegion = null;
            foreach (var region in Regions)
            {                 
                foreach (var tile in region.tiles)
                {


                    var distance = (tile - currentRegion.center).SqrMagnitude();
                    if (region != currentRegion && distance < minDistance)
                    {
      
                            minDistance = distance;
                            nearestRegion = region;
                    }

                }
            }


            PathFinderPath path = ConnectRegionsWithPath(currentRegion, nearestRegion);

            ExpandPassage(path,60);
            TurnPathIntoCavePassage(path);
            Regions.Remove(currentRegion);
            Regions.Remove(nearestRegion);
            Regions.Add(new Region(currentRegion, nearestRegion));
            
        }

        EndTest("Regions connection took ");
    }
    
    public virtual PathFinderPath ConnectRegionsWithPath(Region region1, Region region2)
    {
        if (region1 == region2) throw new NullReferenceException("regions are same");
        float minDistance = float.MaxValue;
        TileData nearestTileToCenter2 = TileData.Null;
        TileData nearestTileToCenter1 = TileData.Null;

        foreach (var tile in region1.tiles)
        {
            float distance = (tile - region2.center).SqrMagnitude();
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestTileToCenter2 = tile;
            }
        }

        minDistance = float.MaxValue;
        foreach (var tile in region2.tiles)
        {
            float distance = (tile - nearestTileToCenter2).SqrMagnitude();
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestTileToCenter1 = tile;
            }
        }


        PathFinderPath path = Pathfinder.FindPath(nearestTileToCenter2, nearestTileToCenter1, false, tryLimit: 10000);
        if (path == PathFinderPath.NUll) throw new NullReferenceException("Path is null");
        var tiles = path.tiles;
        bool remove = false;
        for (int i = 0; i < tiles.Count; i++)
        {
            
            if (remove)
            { 
                tiles.Remove(tiles[i]);
            }
            else
            {
                if (region2.tiles.Contains(tiles[i]))
                {
                    remove = true;
                } 
            }

        }
       
        path = new PathFinderPath(tiles);
        region1.Add(path.tiles);

        return path;
    }


    public class Region
    {
        [HideInInspector] public HashSet<TileData> tiles;
        private TileData _center;
        public int size { get => tiles.Count; }

        public Region(HashSet<TileData> _tiles)
        {
            tiles = _tiles;
        }
        public Region()
        {
            tiles = new HashSet<TileData>();
        }
        public Region(Region r1, Region r2)
        {

            tiles = new HashSet<TileData>();
            foreach (var item in r1.tiles.Concat(r2.tiles))
            {
                tiles.Add(item);
            }
            
        }

        public void Add(List<TileData> tiles)
        {
           
            foreach (var VARIABLE in tiles)
            {
                 this.tiles.Add(VARIABLE);
            }
        }
        public TileData center
        {
            get 
            {
                if (_center == TileData.Null)
                {
                    List<int2> positions = tiles.Select(t => t.position).ToList();

                    int2 sum = int2.zero;
                    foreach (var item in positions)
                    {
                        sum += item;
                    }
                   _center = (sum / positions.Count).ToTileData();
                }
                return _center;
            }
        }

    }
    
    public class Room : Region
    {
        public Room(HashSet<TileData> _tiles)
        {
            tiles = _tiles;
        }
    }
    protected static float[,] depthMap;
    protected void DrawAutomata()
    {           
        depthMap = NoiseGen.GenerateNoiseMap(64, 64, Random.seed, scale, 3, automatonPersistance, automatonLacunarity, Vector2.zero);

        if (Chance( CavesChance))
        {
            var borderFactor = automatonBorderFactor;
            if (Chance(5)) borderFactor = 0.8f;

            var wallPercent = automatonWallPercent;// Random.Range(0, 80);
            int[,] tileStates = CelularAutomaton.ProcessMap(wallPercent, 5, borderFactor, depthMap);

            for (int x = 0; x < tileStates.GetLength(0); x++)
            {
                for (int y = 0; y < tileStates.GetLength(1); y++)
                {
                    TileData newData = new int2(x, y).ToTileData();
                    var tileState = TileState.Any;
                    var biome = Biome.Any;
                    switch (tileStates[x, y])
                    {
                        case 0:
                            tileState = TileState.Floor;
                            biome = Biome.Cave;
                            break;
                        case 1:
                            tileState = TileState.Darkness;
                            biome = Biome.Cave;
                            break;
                    }
                    newData.template.SetState(tileState);
                    newData.SetBiome(biome);
                }
            }
        }
    }
    protected virtual bool canfitRoom(TileData leftCorner, int roomSize, TileState overlappingState = TileState.Darkness)
    {

        if (leftCorner.position.x > 64 - roomSize -2) return false;
        if (leftCorner.position.y > 64 - roomSize -2) return false;
        var rightCorner = leftCorner + new int2(roomSize, roomSize);
        foreach (TileData tile in GetTilesInRectangle(leftCorner - new int2(1,1), rightCorner + new int2(1, 1)))
        {
            if (tile == TileData.Null
                || tile.index == -1
                || (tile.template.tileState != overlappingState)
                || tile.isBorderTile()) //!tile.checkDirectionState( Direction.Null, TileState.Darkness) &&
            {
                return false;
            }
        }
        return true;
    }

    public virtual void placeArcs()
    {
        arcs = new List<TileData>();

        for (int i = 0; i < TileUpdater.current.Length; i++)
        {
            TileData tile = TileUpdater.current[i];
            if (tile.template.tileState == TileState.Floor)
            {
                if (tile.CheckStateInNeibors(TileState.Darkness, false) && tile.CheckStateInNeibors(TileState.Wall, false, 2))
                {
                    arcs.Add(tile);
                    tile.template.SetState(TileState.Door);

                }
            }
        }
    }
    public virtual void PlaceDoors()
    {
        foreach (var item in GetAllMapTiles().Where(t => t.template.tileState == TileState.Door))
        {
            item.template.GenerateObject("WoodenDoor");
        }
    }
    public virtual Room PlaceRoom(TileData leftCorner, RoomSpawnData roomData)
    {
        

        HashSet<TileData> roomTiles = new HashSet<TileData>();
        Direction multipler = BaseMethodsClass.GetRandomDir(true);

        Vector2 mirror = Vector2.zero;//MapGenUtills.GetRandomMiroring();
        if (turnRooms && Chance(50))
        {
            var turnedMap = new TileTemplate[roomData.height,roomData.width];

            for (int x = 0; x < roomData.width; x++)
            {
                for (int y = 0; y < roomData.height; y++)
                {
                    turnedMap[y, x] = roomData.map[x, y];
                }
            }
            roomData.map = turnedMap;
        }


        for (int x = 0; x < roomData.width; x++)
        {
            for (int y = 0; y < roomData.height; y++)
            {
                int X = leftCorner.x + x;
                int Y = leftCorner.y + y;

                var template = roomData.map[x, y];
                template.index = new int2(X, Y).ToMapIndex();
                

                MapGenerator.templateMap[template.index] = template;
                template.index.ToTileData().SetBiome(roomData.defaultBiome);
            }
        }
        

        return new Room(new HashSet<TileData>(roomTiles.Where(t => t.isWalkable(PlayerAbilitiesSystem.playerEntity))));
    }

    protected virtual void GenerateTraps()
    {
        for (int i = 0; i < trapNum; i++)
        {
            TileData tile = GetFreeTile();

            if (Chance(50)) tile.template.GenerateObject("WeaponTrap");
            else tile.template.GenerateObject("ExplosiveTrap");
               


        }
    }


   
}

