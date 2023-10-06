using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitGenerationPreset : GenerationPreset
{
    //protected override void GenerateAsync()
    //{
    //    base.GenerateAsync();
    //    GenerateRooms();
    //    generatePit();
    //    //generatePilars();
    //    generateDebris();

    //    GeneratePassages();
    //    SpawnCreatures();
    //    SpawnPlayer();
    //    SpawnHobos();
    //}

    //public AnimationCurve falloffCurve;
    //void generatePit()
    //{
    //    float[,] noise = NoiseGen.GenerateNoiseMap(mapSize, mapSize, Random.Range(-10000, 10000), scale, 3, automatonPersistance, automatonLacunarity, new Vector2(0, 0));

    //    for (int x = 0; x < mapSize; x++)
    //    {
    //        for (int y = 0; y < mapSize; y++)
    //        {
    //            Vector2Int center = new Vector2Int(mapSize / 2 - 1, mapSize / 2 - 1);
    //            float dst = (new Vector2(x, y) - center).magnitude;
    //            noise[x, y] += Mathf.Clamp(falloffCurve.Evaluate(Mathf.Abs(dst / mapSize)), -1f, 1f);

    //        }
    //    }

    //    foreach (var tile in TileUpdater.current)
    //    {
    //        if(tile.FloorLayer.Length == 0 && tile.SolidLayer == null)
    //        {
    //            tile.GenerateObject("SludgeFloor");
    //            if (Chance(0.1f)) tile.GenerateObject("SludgePillar");

    //            if(tile.isBorderTile()) tile.GenerateObject("SludgeWall");
    //            if (noise[tile.position.x, tile.position.y] < 0.5f)
    //            {
    //                //tile.generateObject("BlackSand");
    //                tile.GenerateObject("ShallowWater");
    //            }
    //        }
    //    }
    //}

    //void generateDebris()
    //{
    //    foreach (var tile in GetAllMapTiles())
    //    {
    //            string[] debris01 = new string[] { "SludgePillar", "SludgeWall", "RockWall","BrickWall", "Stalagmite", "Fire", "BigRock"};

    //            if (tile.SolidLayer == null)
    //            {
    //                if (Chance(1f)) tile.GenerateObject(debris01.RandomItem());
    //            }
    //    }
    //}
    //void SpawnHobos()
    //{
    //    for (int i = 0; i < 10; i++)
    //    {
    //        Creature hobo = GetFreeTile().GenerateObject(ItemDataBase.i.GetRandomCreature(Biome.Any, Random.Range(0,10)).creature) as Creature;
    //        if(hobo.CanEquipItems)
    //        {
    //            hobo.nameWithoutUpgrades = "Dihonored " + hobo.name;
    //            hobo.EquipItem(hobo.inventory.recieveItemFromGenerator(ItemDataBase.i.GetRandomThing(null, Random.Range(0, 10), Biome.Any)), EquipPice.Weapon);
    //            hobo.teamTag = Tag.None;
    //            hobo.tags.Remove(Tag.Human);
    //            hobo.enemyTags.Remove(Tag.Player); 
                
    //        }
    //        //hobo.stats.STR *= Random.Range(0.5f, 2);
    //        //hobo.AGL *= (int)Random.Range(0, 20);
    //        //hobo.maxHp *= (int)Random.insideUnitCircle.x;
    //        hobo.DefaultMovementCost *= Random.Range(0.5f, 1.5f);

    //    }

    //}


}
