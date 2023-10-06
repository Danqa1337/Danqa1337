using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class DungeonStructure
{
    [SerializeField]
    public static DungeonStructureData dungeonStructureData;

    public static Location CurrentLocation
    {
        get => dungeonStructureData.currentLocation;
        set => dungeonStructureData.currentLocation = value;
    }

    public static int depth => dungeonStructureData.depth;
    public static LocationTransition GetTransition(int id)
    {
        foreach (var transition in dungeonStructureData.transitions)
        {
            if (transition.id == id) return transition;
        }
        throw new NullReferenceException("no such transition");

    }
    public static Location GetLocation(int id)
    {
        foreach (var location in dungeonStructureData.locations)
        {
            if (location.id == id) return location;
        }

        throw new NullReferenceException("no such location");
    }

}
