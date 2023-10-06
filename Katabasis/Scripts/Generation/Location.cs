using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Location 
{
    public string name;
    public int id;
    public int level = 0;
    public GenerationPresetType generationPreset;
    public List<int> transitionsIDs = new List<int>();

    public Location(string name, int id, int level, GenerationPresetType generationPreset)
    {
        this.name = name;
        this.id = id;
        this.level = level;
        this.generationPreset = generationPreset;
    }
    public Location(int id, int level, GenerationPresetType generationPreset)
    {
        
        this.id = id;
        this.level = level;
        this.generationPreset = generationPreset;

        this.name = generationPreset + " " + level;
    }
}
