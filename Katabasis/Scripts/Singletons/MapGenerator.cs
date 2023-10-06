using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Unity.Mathematics;
using Random = UnityEngine.Random;

public enum Biome
{
    Dungeon,
    Cave,
    SmallCave,
    FloodedCave,
    Chasm,
    Lake,
    HotCave,
    Arena,
    TestPreset,
    Pit,
    Any,
    Entrance,
    LakeArena,
    Wall,
    LakeShrine
}
public enum GenerationPresetType
{
    Dungeon,
    Lake,
    Pit,
    Arena,
}
public class MapGenerator : Singleton<MapGenerator>
{
    public int seed;
    public bool alwaysRandomSeed;
    public Location location;

    public static int mapSize = 64;
    public static TileTemplate[] templateMap;
    
    public void Generate(Location location)
    {
        this.location = location;
        Generate();
    }
    public void Generate()
    {
        Clear();
        if (Application.isPlaying)
        {
            LoadingScreen.i.Show();
        }
        else
        {
            Engine.ImportDefaultValues();

        }

        if (alwaysRandomSeed)
        {
            seed = (int)DateTime.Now.Ticks;
            Random.seed = seed;
        }
        else
        {
             Random.seed = seed;
        }

        templateMap = new TileTemplate[4096];

        for (int i = 0; i < 4096; i++)
        {
            templateMap[i] = new TileTemplate(i);
            
        }

        switch (location.generationPreset)
        {
            case (GenerationPresetType.Dungeon):
                GetComponent<StandartDungeonPreset>().Generate();
                break;
            case (GenerationPresetType.Arena):
                GetComponent<ArenaDungeonPreset>().Generate();
                break;
            case (GenerationPresetType.Pit):
                GetComponent<PitGenerationPreset>().Generate();
                break;
            case (GenerationPresetType.Lake):
                GetComponent<LakeGenerationPreset>().Generate();
                break;
        }
        if(Application.isPlaying)
        {
            LoadingScreen.i.Hide();
            PhysicsSystem.Init();
            Announcer.Announce("Wellcome to location " + location.name);

        }
        //templateMap.Dispose();

    }

    public void Clear()
    {
        foreach (var VARIABLE in FindObjectsOfType<EntityAuthoring>())
        {
            if (Application.isPlaying)
            {
                Pooler.PutObjectBackToPool(VARIABLE.gameObject);
            }
            else
            {
                VARIABLE.gameObject.SetActive(false);

            }
        }
    }

    public void RandomSeed()
    {
        seed = Random.Range(-100000, 100000);
    }

}


