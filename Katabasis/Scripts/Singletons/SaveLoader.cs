using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Unity.Entities;
using UnityEngine;
using Application = UnityEngine.Application;

public static class SaveLoader
{
    public static string savesFolderPath = Application.persistentDataPath + "/Saves/";
    public static void DeleteSaves()
    {
        var names = Directory.GetFiles(savesFolderPath);
        foreach (var name in names)
        {
           File.Delete( name);
        }
    }

    public static bool SavesExist()
    {
        var path = savesFolderPath + "DungeonStructure.dat";
        return File.Exists(path);
    }
    public static bool LoadCurrentLocation()
    {
        
        BinaryFormatter formatter = new BinaryFormatter();
        
        Engine.i.Dispose();
        
        MapGenerator.i.Clear();
        TileUpdater.Init();
        var path = savesFolderPath + "DungeonStructure.dat";
        if(SavesExist())
        {
            FileStream fs = new FileStream(path, FileMode.Open);
            DungeonStructure.dungeonStructureData = formatter.Deserialize(fs) as DungeonStructureData;

            var currentLocation = DungeonStructure.CurrentLocation;

            BaseMethodsClass.StartTest("Loading location " + currentLocation.id);
            path = savesFolderPath + "location" + currentLocation.id + ".dat";
            if (File.Exists(path))
            {
                fs = new FileStream(path, FileMode.Open);
            
                var locationSave = new LocationSaveData();
                try
                {
                    locationSave = formatter.Deserialize(fs) as LocationSaveData;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
                    throw;
                }
                finally
                {
                    fs.Close();

                    foreach (var serializableTile in locationSave.tiles)
                    {
                        TileUpdater.current[serializableTile.tile.index] = serializableTile.tile;
                        Spawner.Spawn(serializableTile.solidlayer);
                        Spawner.Spawn(serializableTile.floorLayer);
                        Spawner.Spawn(serializableTile.liquidLayer);
                        foreach (var data in serializableTile.dropLayer)
                        {
                            Spawner.Spawn(data);
                        } 
                        foreach (var data in serializableTile.groundCover)
                        {
                            Spawner.Spawn(data);
                        }
                    }
                    World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<SeamlessTextureSystem>().Update();
                    PhysicsSystem.Init();
                    TileUpdater.FOVUpdateScheduled = true;
                    TileUpdater.Update();

                    Announcer.Announce("Wellcome to " + currentLocation.name);

                    BaseMethodsClass.EndTest("Loading complete: ");
                
                }
                return true;
            }
            else
            {
                Debug.Log("Location save file not found");
                return false;
            }
        }
        else
        {
            Debug.Log("Dungeon structure save file not found");

            return false;
        }

    }

    public static DynamicObjectData LoadPlayerData()
    {
        var path = savesFolderPath + "PlayerData.dat";
        if(File.Exists(path))
        {
       
            var fs = new FileStream(path, FileMode.Open);
            var formatter = new BinaryFormatter();
            var playerData = formatter.Deserialize(fs) as PlayerSaveData;
            fs.Close();
            XPCounter.CurrentXP = playerData.currentXp;
            XPCounter.CurrentLevel = playerData.currentLVL;
            XPCounter.XPpoints = playerData.XPPoints;
            TimeSystem.Ticks = playerData.CurrentTime;
            TickCounter.i.SetValue(playerData.CurrentTime);


            return playerData.DynamicData;
            
        }
        Debug.Log("Playerdata not found");
        return null;
    }
    public static async Task Save(Location location)
    {
        BaseMethodsClass.StartTest("Saving...");

        var levelSaveData = new LocationSaveData();
        for (int i = 0; i < 4096; i++)
        {
            levelSaveData.tiles[i] = EntitySerializer.SerizlizeTile(TileUpdater.current[i]);
        }

        if (!Directory.Exists(savesFolderPath))
        {
            Directory.CreateDirectory(savesFolderPath);
        }



        var path = savesFolderPath + "location"+ location.id +".dat";
        FileStream fs = new FileStream(path, FileMode.Create);
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(fs, levelSaveData);
        fs.Close();
        

        
        
        
        path = savesFolderPath + "DungeonStructure.dat"; 
        fs = new FileStream(path, FileMode.Create);
        formatter.Serialize(fs, DungeonStructure.dungeonStructureData);
        fs.Close();

        var playerData = new PlayerSaveData()
        {
            DynamicData = EntitySerializer.DeconstructEntity(PlayerAbilitiesSystem.playerEntity),
            XPPoints = XPCounter.XPpoints,
            currentLVL = XPCounter.CurrentLevel,
            currentXp = XPCounter.CurrentXP,
            CurrentTime = TimeSystem.Ticks,

        };
        path = savesFolderPath + "PlayerData.dat";
        fs = new FileStream(path, FileMode.Create);
        formatter.Serialize(fs, playerData);
        fs.Close();



        BaseMethodsClass.EndTest("Saving complete: ");
        
    }

    
}
[System.Serializable]

public class LocationSaveData
{
   public SerializableTileData[] tiles = new SerializableTileData[4096];
   
}

[System.Serializable]
public class PlayerSaveData
{
    public int currentXp;
    public int currentLVL;
    public int XPPoints;
    public int CurrentTime;
    public DynamicObjectData DynamicData;
}





