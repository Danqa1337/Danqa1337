using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LakeGenerationPreset : GenerationPreset
{
    protected override void GenerateAsync()
    {
        base.GenerateAsync();
        DrawAutomata();
        
        DefineRegions();
        RemoveBadRegions();
              
        DefineBiomes();
        PlaceShrine();
        ConnectRegions();  

        //GenerateSecretRooms();
        //SpawnGeisers();
        PopulateRegions();
        GenerateAbyssSides();
        
        FillDarkness();
        //GeneratePassages();
        

        spawnItems();
        SpawnCreatures();



        // GenerateTraps();

        GenerateStairs();
        SpawnPlayer();
        ApplyTiles();

    }
    public override void FillDarkness()
    {
        foreach (var item in GetAllMapTiles().Where(t=>t.template.tileState == TileState.Darkness))
        {
            item.template.GenerateObject("MarbleWall");
        }
    }
    public override void DefineBiomes()
    {
        Regions = Regions.OrderBy(r => 1f / r.size).ToList();
        foreach (var tile in Regions[0].tiles)
        {
            if (true)
            {
                tile.template.GenerateObject("SandFloor");
                if (!tile.CheckStateInNeibors(TileState.Darkness, true))
                {
                    tile.template.GenerateObject("ShallowWater");
                    if (depthMap[tile.x, tile.y] < 0.1) tile.template.SetState(TileState.Abyss);
                }
                tile.SetBiome(Biome.Lake);
                
            }
        }
    }
    
    public void PlaceShrine()
    {
        var shrine = RoomDatabase.GetRoom("LakeShrine1");
        var tiles = Regions[0].tiles.ToList();
        tiles.Shuffle();

        foreach (var tile in tiles)
        {
            if (canfitRoom(tile, shrine.GetSize(), TileState.ShallowWater))
            {
                var room = PlaceRoom(tile, shrine);
                Spawner.Spawn("Ea", tile.position);
                foreach (var roomTile in room.tiles)
                {
                    roomTile.SetBiome(Biome.LakeArena);
                }
                return;
            }
        }

    }
}
