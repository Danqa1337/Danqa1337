using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonStructureGenerator : Singleton<DungeonStructureGenerator>
{

    [System.NonSerialized]
    private List<LocationTransition> openTransitions;
    [System.NonSerialized]
    public List<LocationTransition> closedTransitions;

    public int LakeDepth;
    public int PitDepth;
    public float branchingChance;
    public int NumberOfLayers;


    [ContextMenu("Generate")]
    public void Generate()
    {
        DungeonStructureData dungeonStructureData = new DungeonStructureData();

        openTransitions = new List<LocationTransition>();
        closedTransitions = new List<LocationTransition>();
        dungeonStructureData.pit = null;
        dungeonStructureData.depth = NumberOfLayers;
        


        for (int i = 0; i < NumberOfLayers; i++)
        {

            GenerationPresetType preset = GenerationPresetType.Dungeon;
            if (i == LakeDepth) preset = GenerationPresetType.Lake;




            var newLocation = CreateNewLocation(preset);
            newLocation.level = i;
            //createTransition(newLocation);

            
            dungeonStructureData.locations.Add(newLocation);
        }


        if (dungeonStructureData.locations.Count > 4)
        {
            GeneratePit();
        }

        for (int j = 0; j < NumberOfLayers-1; j++)
        {
            var locationA = dungeonStructureData.locations[j];
            var locationB = dungeonStructureData.locations[j+1];

            var transition = new LocationTransition(locationA.id, locationB.id);
            transition.id = j;
            locationA.transitionsIDs.Add(transition.id);
            locationB.transitionsIDs.Add(transition.id);
            dungeonStructureData.transitions.Add(transition);
        }


        Debug.Log(dungeonStructureData.locations.Count);
        dungeonStructureData.currentLocation = dungeonStructureData.locations[0];
        
        DungeonStructure.dungeonStructureData = dungeonStructureData;
        DungeonStructureVisualizer.i.Visualize();


        void connectEnds()
        {
        //List<Location> deadEnds = new List<Location>();
        //foreach (var location in locations)
        //{
        //    if (location.isDeadEnd) deadEnds.Add(location);
        //}

        //hell = NewLevel(Biome.HotCave);
        //hell.level = 20;
        //foreach (var end in deadEnds)
        //{
        //    createTransition(end, hell);
        //}
        }
        void GeneratePit()
        {
            dungeonStructureData.pit = CreateNewLocation(GenerationPresetType.Pit);
            dungeonStructureData.pit.level = 5;

            //CreateNewTransition(dungeonStructureData.pit, dungeonStructureData.locations[4], true ,true,true);
           
        }



        Location CreateNewLocation(GenerationPresetType preset)
        {

            var newLocation = new Location(dungeonStructureData.locations.Count, dungeonStructureData.locations.Count, preset);


            //LocationTransition ParentLocationTransition = openTransitions.RandomItem();
            //ParentLocationTransition.to = newLocation;
            //openTransitions.Remove(ParentLocationTransition);
            //closedTransitions.Add(ParentLocationTransition);
            //newLocation.transitions.Add(ParentLocationTransition);


            return newLocation;
        }
        
    }
   
    //public LocationTransition GetTransition(int id)
    //{
    //    foreach (var item in closedTransitions)
    //    {
    //        if (item.id == id) return item;
    //    }
    //    return null;
    //}
    //public Location GetLocation(int id)
    //{
    //    foreach (var item in locations)
    //    {
    //        if (item.id == id) return item;
    //    }
    //    return null;
    //}
   
    
    

}
[System.Serializable]
public class DungeonStructureData
{
    public List<Location> locations = new List<Location>();
    public List<LocationTransition> transitions = new List<LocationTransition>();

    public Location pit;

    public Location hell;

    public int depth;

    public Location currentLocation;
}
