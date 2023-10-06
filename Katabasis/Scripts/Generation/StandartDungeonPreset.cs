using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Burst;
using Unity.Jobs;
public class StandartDungeonPreset : GenerationPreset
{

    protected override void GenerateAsync()
    {
        base.GenerateAsync();
        GenerateMap();
    }


    public void GenerateMap()
    {
        DrawAutomata();
        DefineRegions();
        RemoveBadRegions();

        DefineBiomes();

        ConnectRegions();
        CreateRiver();

        GenerateRooms();
        GenerateCoridors();
        
        DefineRegions();
        ConnectRegions();
        //GenerateSecretRooms();
        //SpawnGeisers();
        PlaceDoors();
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

}
