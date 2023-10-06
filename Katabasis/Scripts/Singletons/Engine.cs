using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.IO;
using System.Xml.Linq;
using System.Xml;
using UnityEngine.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;
using Unity.Entities;
using Debug = UnityEngine.Debug;


public class Engine : Singleton<Engine>
{

	public Canvas loadingScreen;

    [Header("Constants")]
	public int loadDistance=15;
	public float VMax = 35;
	public float timescale = 1;
	public float baseRangedInaccuracity;
	[Range(0, 100)]
	public float bloodChanceBonus;
	[Range(0, 100)]
	public float flamabilityChance;
	[Range(0, 100)]
	public float cretatureSelfExtingusihChance;
	[Range(0, 100)]
	public float ExtinguishChance;
	[Range(0, 1f)]
	public float smokeEmissionChance;
	[Range(0, 1)]
	public float maxDarknessA;
	[Range(0, 1)]
	public float maxAirA;
	[Range(0, 1000)]
	public int frameDrawInterval;


	public uint dustEmissionWeight;

	[Space]
	[Header("Curves")]
	public AnimationCurve WeightToSpeedCurve;
	public AnimationCurve WeightToCostCurve;
	public AnimationCurve SpeedToDistance;
	public AnimationCurve lightCurve;
	public AnimationCurve fireLvlToFlamability;
	public AnimationCurve TToFlamability;
	public AnimationCurve MovementAnimation;
	public AnimationCurve ButtAnimation;

	[Space]
	[Header("Bools")]
	public bool playAnimations = false;
	public bool drawTraces = false;
	public bool showTrails;
	public bool showDamageOnCreatures;
	public bool showDamageOnThings;
	public bool debugSystems;
	[HideInInspector]
	public bool ready = false;
	public Toggle animationsToggle;

	[Space]
	[Header("Masks")]
	public LayerMask lightColliderLayerMask;
	public LayerMask solidCollider;

	[HideInInspector]
	public Slider animationSpeedSlider;
	public Slider bloodAmountSlider;


	public Texture2D Darkness;
	public Texture2D FOV;
	public Texture2D PathMask;



	//private static Engine _instance;
	//public static Engine i
	//{
	//	get
	//	{

	//		return _instance;
	//	}
	//}

    public static void ImportDefaultValues()
    {
        ItemDataBase.i.ImportDefaultValues();
        RoomDatabase.i.ImportDefaultValues();
    }
	public void Start()
    {
        
        ImportDefaultValues();
		UiManager.i.CloseAll();
		Pooler.i.RecreatePools();
        DungeonStructureGenerator.i.Generate();
		if (MapGenerator.i.location.generationPreset != GenerationPresetType.Arena)
        {
            if (SaveLoader.LoadCurrentLocation())
            {
                

                Spawner.SpawnPlayer();
				TimeSystem.SheludeFrameDraw();
                TileUpdater.Update();
                PlayerAbilitiesSystem.controlled = true;
            }
            else
            {
                DungeonStructureGenerator.i.Generate();
				MapGenerator.i.Generate(DungeonStructure.CurrentLocation);

            }
        }
        else
        {

            MapGenerator.i.Generate();
        }

    }

    public static Action OnDispose;
    public void Dispose()
    {
		OnDispose?.Invoke();
		Camera.main.transform.SetParent(null);
        var all = World.DefaultGameObjectInjectionWorld.EntityManager.GetAllEntities();
        foreach (var entity in all)
        {
            entity.Destroy();

        }
        //all.Dispose();
        //for (int j = 1; j < World.DefaultGameObjectInjectionWorld.Systems.Count; j++)
        //{
        //    var system = World.DefaultGameObjectInjectionWorld.Systems[j];
        // 	World.DefaultGameObjectInjectionWorld.DestroySystem(system);
        //}
        //TileUpdater.NeiborsOffsetsArray4.Dispose();
        //TileUpdater.NeiborsOffsetsArray8.Dispose();
        TileUpdater.Dispose();
    }

    public void Restart()
    {
        //foreach (var slot in FindObjectsOfType<InventorySlot>())
        //{
        //    slot.Clear();
        //}
        Dispose();
		Start();
		
    }
	
	//public GameObject[];
	public string SavePath;
	
	
	
    public void OnApplicationQuit()
    {
        Dispose();
        TileUpdater.NeiborsOffsetsArray4.Dispose();
        TileUpdater.NeiborsOffsetsArray8.Dispose();
    }


	public void DeleteSaves()
    {
		SaveLoader.DeleteSaves();
		
    }


}




