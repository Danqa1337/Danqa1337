using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
[CreateAssetMenu(fileName = "New Room Database", menuName = "Generation/Rooms/Database")]
public class RoomDatabase : SingletonScriptableObject<RoomDatabase>
{
    public RoomsList roomXLS;

    public static List<RoomSpawnData> rooms = new List<RoomSpawnData>();
    protected void OnVaidate()
    {
        ImportDefaultValues();
    }

    public static  RoomSpawnData GetRandomRoom(GenerationPresetType biome, int lvl, int maxSize = 999 )
    {
        List<ISpawnDataByChance> roomsOfThatSize = rooms.Where
            (r => r.GetSize() <= maxSize && r.active)
            .Cast<ISpawnDataByChance>().ToList();
        if (roomsOfThatSize.Count != 0)
        {
            
            RoomSpawnData room = BaseMethodsClass.GetRandomSpawnData(roomsOfThatSize, lvl) as RoomSpawnData;
            return room;
        }
        else
        {
            throw new KeyNotFoundException("No room of size less then: " + maxSize);
        }
        
    }
    public static RoomSpawnData GetRoom(string Name)
    {
        foreach (var item in rooms)
        {
            if (item.Name == Name) return item;
        }
        throw new KeyNotFoundException("No room with such name: " + Name);
    }
    private static RoomDatabase _instance;
    public static RoomDatabase Instance 
    { 
        get
        {
            
            _instance = Resources.Load<RoomDatabase>("Room Database");
            return _instance;
        } 
    }
    [ContextMenu("Import default values")]
    public void ImportDefaultValues()
    {
        rooms = new List<RoomSpawnData>();
        foreach (var param in roomXLS.sheets[0].list)
        {
            if (param.Name != "")
            {
                

                RoomSpawnData room = new RoomSpawnData();

                room.baseSpawnChance = param.baseSpawnChance;
                room.normalDepthStart = param.normalDepthStart;
                room.normalDepthEnd = param.normalDepthEnd;
                room.Name = param.Name;
                room.layers = new List<Texture2D>();
                room.active = param.Enabled;
                room.AllowedBiomes = BaseMethodsClass.DecodeCharSeparatedEnums<Biome>(param.AllowedBiomes);
                room.defaultBiome = param.DefaultBiome.DecodeCharSeparatedEnumsAndGetFirst<Biome>();

                for (int i = 0; i < 10; i++)
                {
                    Texture2D layer = Resources.Load<Texture2D>("Rooms/" + room.Name + "_" + i);
                    if (layer != null) room.layers.Add(layer);
                    else break;
                }
                rooms.Add(room);
            }
        }

        foreach (var VARIABLE in rooms)
        {
            VARIABLE.DecodePNGs();
        }
    }
    //public Dictionary<int, RoomObject> GetRandomRoom = new Dictionary<int, RoomObject>();
}

public class RoomSpawnData : ISpawnDataByChance
{
    public string Name;
    public string Defaultfloor;
    public List<Biome> AllowedBiomes;
    public Biome defaultBiome;
    public List<Texture2D> layers;
    public bool active = true;
    public TileTemplate[,] map;
    public int width => map.GetLength(0);
    public int height => map.GetLength(1);

    public int GetSize()
    {
       return Mathf.Max(layers[0].height, layers[0].width);
    }

    public void DecodePNGs()
    {
        var width = layers[0].width;
        var height = layers[0].height;
        map = new TileTemplate[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                map[x, y] = new TileTemplate(0);
            }
        }



        foreach (var texture in layers)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {

                    var color = texture.GetPixel(x, y);
                    if (color.a == 0) continue;
                    else
                    {
                        var data = ItemDataBase.GetObject(color);

                        var template =  map[x,y];
                        template.Biome = defaultBiome;
                        template.GenerateObject(data, false);
                        map[x, y] = template;
                        
                    }

                }
            }
        }
    }

    public int normalDepthStart { get; set; }
    public int normalDepthEnd { get; set; }
    public float baseSpawnChance { get; set; }
}

