using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using System.Linq;

public class ArenaDungeonPreset : GenerationPreset
{
    public Texture2D arenaMap;
    protected override void GenerateAsync()
    {
        base.GenerateAsync();
        GenerateMap();
    }




    
    public void GenerateMap()
    {

        placeArenaFromObject();
        GenerateAbyssSides();
        SpawnPlayer();
        SpawnTestItems();
        SpawnDummy();
        SpawnTestCreatures();
        GenerateArmory();

        ApplyTiles();

        
        //spawnDummy();
        //spawnEnemies();

        //mapObject.Save();
        //GiveEquipmentToThePlayer();
    }

    private void SpawnTestCreatures()
    {
        foreach (var item in new int2(12,9).ToTileData().GetNeibors(true))
        {
            Spawner.Spawn("Rat", item.position);
        }
        foreach (var item in new int2(12,12).ToTileData().GetNeibors(true))
        {
            Spawner.Spawn("Cat", item.position);
        } 




        //var knives = new List<string>();
        //for (int i = 0; i < 100; i++)
        //{
        //    knives.Add("Knife");
        //}

        //Spawner.Spawn(ObjectDataFactory.GetCreature("Human",
        //     itemInMainHand: "Knife",
        //     itemOnHead: "LeatherHelm",
        //     itemOnChest: "LeatherChestplate",
        //     itemsInInventory: knives,
        //     enemyTags: new List<Tag>() { Tag.Player }), new float2(20, 20))
        //    ;
            


        //Spawner.Spawn(ObjectDataFactory.GetCreature("Human",
        //   itemInMainHand: "Axe",
        //   enemyTags: new List<Tag>() { Tag.Player }), new float2(7, 16));


        //Spawner.Spawn("Cat", new float2(19, 19));


       


        //foreach (var tile in GetTilesInRadius(new int2(17, 17).ToTileData(), 5))
        //{
        //    Spawner.Spawn("LeatherChestplate", tile.position);
        //}

    }
    public void SpawnDummy()
    {
        Spawner.Spawn("Dummy", new float2(18, 18));
    }

    private void SpawnTestItems()
    {
        for (int i = 0; i < 10; i++)
        {
            Spawner.Spawn("Grenade", new float2(17, 17));

        }
    }

    private void GiveEquipmentToThePlayer()
    {
        //for (int i = 0; i < 10; i++)
        //{
        //     Player.i.currentTile.GenerateObject("Arrow",false,false);
        //}
       
    }
    private void spawnDummy()
    {
        //for (int i = 0; i < 50; i++)
        //{
        //    GetFreeTile().GenerateObject("OldCat");
        //} 
    }

    public void GenerateArmory()
    {
        var tiles = GetTilesInRectangleList(new int2(10, 2), new int2(20, 8));
        var items = new List<SpawnData>();
        foreach (var item in ItemDataBase.i.spawnList)
        {
            if(item.objectData.dynamicData.objectType == ObjectType.Drop && item.objectData.staticData.bodyPartTag == BodyPartTag.Body)
            {
                items.Add(item);
            }
        }
        for (int i = 0; i < items.Count; i++)
        {
            Spawner.Spawn(items[i].objectData, tiles[i].position);
        }
    }

    public override void SpawnPlayer()
    {
        Spawner.Spawn("Ciclops", new float2(16, 16));


    }


    public void placeArenaFromObject()
    {
        TileData center = new int2(1,1).ToTileData();   
        
        PlaceRoom(center, RoomDatabase.GetRoom("TestArena"));
    }
}

