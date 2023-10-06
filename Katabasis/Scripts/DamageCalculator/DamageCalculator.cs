using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEngine.SceneManagement;
using Unity.Entities;

[ExecuteInEditMode]
public class DamageCalculator : MonoBehaviour
{
    public enum InputCalculatorParams
    {
        STR,
        AGL,
        WGH,
        ACR,
        SHR,
        scalingSTR,
        scalingAGL,

    }
    public enum OutputCalculatorParam
    {

        TOH,
        V,
        DPT,
        DPA,
        ATC,
    }

    [Header("Test")]
   // public Creature testCreature;
    
    [Range(1, 50)] public int STR;
    
    [Range(1, 50)] public int AGL;
    
    [Range(0.5f, 100)] public float WGH;
    
    [Range(0.5f, 100)] public int accuracity;

    public Scaling ScalingSTR;
    public Scaling ScalingAGL;

    [Range(0, 3)] public float sharpness;
    

    public float THC;
    public float ACost;
    public float V;
    public float DPT;
    public float DPA;

    [Header("Params")]
    
    public Vector2 minATC;
    public Vector2 maxATC;

    public Vector2 minTOH;
    public Vector2 maxTOH;

    public static Vector2 _minATC;
    public static Vector2 _maxATC;

    public static Vector2 _minTOH;
    public static Vector2 _maxTOH;




    public AnimationCurve weaponWeight_STR_toCost;
    public AnimationCurve Agl_weaponElegancy_toTOH;

    private static AnimationCurve _weaponWeight_STR_toCost;
    private static AnimationCurve _Agl_weaponElegancy_toTOH;

    public AnimationCurve weight_toBaseCost;
    private static AnimationCurve _weight_toBaseCost;


    public AnimationCurve STRscaling;
    public AnimationCurve AGLscaling;

    private static AnimationCurve _STRscaling;
    private static AnimationCurve _AGLscaling;
    public InputCalculatorParams input;
    public OutputCalculatorParam output;

    public CurveVisualizer visualizer;
    public bool active;


    //private void OnValidate()
    //{
    //    if (!Application.isPlaying)
    //    {

    //        ApplyDefaults();

    //        testWeapon.Inaccuracity = accuracity;
    //        testWeapon.Sharpness = sharpness;

    //        testWeapon.ScalingSTR = ScalingSTR;
    //        testWeapon.ScalingAGL = ScalingAGL;

    //        //testCreature.stats.STR = STR;
    //        //testCreature.stats.AGL = AGL;

    //        List<Vector2> keys = new List<Vector2>();

    //        testWeapon.Weight = WGH;
    //        testWeapon.Inaccuracity = accuracity;
    //        testWeapon.Sharpness = sharpness;

    //        //testCreature.stats.STR = STR;
    //        //testCreature.stats.AGL = AGL;
    //        THC = CalculateTOH(testCreature, testWeapon);
    //        ACost = CalculateAtackCost(testCreature, testWeapon);
    //        V = CalculateV(testCreature, testWeapon);
    //        DPT = CalculateDPT(testCreature, testWeapon);
    //        DPA = CalculateDPA(testCreature, testWeapon);

    //        visualizer.Clear();
    //        for (float i = 1f; i < 100; i += 0.1f)
    //        {
    //            modifyField(input, testCreature, testWeapon, i);
    //            keys.Add(new Vector2(i, CalculateParam(output, testCreature, testWeapon)));
    //        }
    //        visualizer.visualize(keys, visualizer.weightColor);




    //    }
    //}
    //private void ApplyDefaults()
    //{
    //    _weaponWeight_STR_toCost = weaponWeight_STR_toCost;
    //    _Agl_weaponElegancy_toTOH = Agl_weaponElegancy_toTOH;
    //    _weight_toBaseCost = weight_toBaseCost;

    //    _minATC = minATC;
    //    _maxATC = maxATC;
    //    _minTOH = minTOH;
    //    _maxTOH = maxTOH;

    //    _AGLscaling = AGLscaling;
    //    _STRscaling = STRscaling;
    //}
    //private void Awake()
    //{
    //    ApplyDefaults();
    //}
    //private static float CalculateParam(OutputCalculatorParam calculatorParams, Creature creature, Thing weapon) => (calculatorParams) switch
    //{

    //    OutputCalculatorParam.V => CalculateV(creature,weapon),
    //    OutputCalculatorParam.DPT => CalculateDPT(creature, weapon),
    //    OutputCalculatorParam.DPA => CalculateDPA(creature, weapon),
    //    OutputCalculatorParam.ATC => CalculateAtackCost(creature, weapon),
    //    OutputCalculatorParam.TOH => CalculateTOH(creature, weapon),
    //};

    private static float ScalingToFloat(Scaling scaling) => (scaling) switch
    {

        Scaling.A => 0.3f,
        Scaling.B => 0.2f,
        Scaling.C => 0.1f,
        Scaling.D => 0.05f,
        Scaling._ => 0,
    };
    private static Scaling FloatToScaling(float scaling)
    {
        if (scaling > 4) return Scaling.A;
        if (scaling > 3) return Scaling.B;
        if (scaling > 2) return Scaling.C;
        if (scaling > 1) return Scaling.D;
        return Scaling._;

    }
    //private static void modifyField(InputCalculatorParams calculatorParams, Creature creature, Thing weapon, float value)
    //{

    //    //switch (calculatorParams)
    //    //{
    //    //    case InputCalculatorParams.STR: 
    //    //        typeof(CreaturesStats).GetField("STR").SetValue(creature.stats, (int)value);
    //    //        break;
    //    //    case InputCalculatorParams.AGL: 
    //    //        typeof(CreaturesStats).GetField("AGL").SetValue(creature.stats, (int)value);
    //    //        break;
    //    //    case InputCalculatorParams.WGH:
    //    //        typeof(Thing).GetField("weight").SetValue(weapon, value);
    //    //        break;
    //    //    case InputCalculatorParams.ACR:
    //    //        typeof(Thing).GetField("accuracity").SetValue(weapon, value);

    //    //        break;
    //    //    case InputCalculatorParams.SHR:
    //    //        typeof(Thing).GetField("sharpness").SetValue(weapon, value);

    //    //        break;
    //    //    case InputCalculatorParams.scalingSTR:

    //    //        typeof(Thing).GetField("ScalingSTR").SetValue(weapon, FloatToScaling(value));

    //    //        break;
    //    //    case InputCalculatorParams.scalingAGL:
    //    //        typeof(Thing).GetField("ScalingAGL").SetValue(weapon, FloatToScaling(value));

    //    //        break;
    //    //}


    //}




    // [ContextMenu("Reset Curves")] public void ResetCurves()
    //{
    //    weaponWeight_STR_toCost.Clear();
    //    Agl_weaponElegancy_toTOH.Clear();

    //    weaponWeight_STR_toCost.AddKey(0, 1);
    //    weaponWeight_STR_toCost.AddKey(1, 1);
    //    Agl_weaponElegancy_toTOH.AddKey(0, 0);
    //    Agl_weaponElegancy_toTOH.AddKey(1, 1);

    //    ApplyCurves();
    //}

    //[ContextMenu("Apply Curves")] public void ApplyCurves()
    //{
    //    weaponWeight_STR_toCost.MoveKey(0, minATC);
    //    weaponWeight_STR_toCost.MoveKey(weaponWeight_STR_toCost.keys.Length - 1, maxATC);

    //    Agl_weaponElegancy_toTOH.MoveKey(0, minTOH);
    //    Agl_weaponElegancy_toTOH.MoveKey(Agl_weaponElegancy_toTOH.keys.Length - 1, maxTOH);
    //}
    public static float CalculateDPT(Entity creature, Entity weapon)
    {
        var ph = weapon.GetComponentData<PhysicsComponent>();

        float DPA =  ph.damage + CalculateBonusDamage(creature, weapon);
        
        float THC = CalculateTOH(creature, weapon) / 100;
        float ATC = CalculateAttackCost(creature, weapon);

        
        return (DPA * THC) / ATC;

        //return (dmg / Mathf.Max(Engine.i.WeightToCostCurve.Evaluate( Mathf.Clamp01(v/ Engine.i.VMax)), 1) );

    }
    //public static float CalculateDPA(Creature creature, Thing weapon)
    //{
    //    return CalculateV(creature, weapon) * weapon.Weight * weapon.Sharpness;
    //}
    public static float CalculateScalingBonus(Entity creature, Entity weapon)
    {
        var ph = weapon.GetComponentData<PhysicsComponent>();
        var stats = creature.GetComponentData<CreatureComponent>();
        return  stats.str * ScalingToFloat(ph.ScalingAGL) + stats.agl * ScalingToFloat(ph.ScalingAGL);
    }
    public static int CalculateBonusDamage(Entity creature, Entity weapon)
    {  
        if (weapon == Entity.Null) throw new NullReferenceException("Weapon is null");
        var stats = creature.GetComponentData<CreatureComponent>();
        var ph = weapon.GetComponentData<PhysicsComponent>();

        int bonusDamage = Mathf.FloorToInt( (stats.str * ScalingToFloat(ph.ScalingSTR) + stats.agl * ScalingToFloat(ph.ScalingAGL)) / 10 * ph.damage);
        return bonusDamage;
    }
    public static float CalculateTOH(Entity creature, Entity weapon)
    {
        var ph = weapon.GetComponentData<PhysicsComponent>();
        var stats = creature.GetComponentData<CreatureComponent>();
        return ph.accuracy;
    }
    public static int CalculateAttackCost(Entity creature, Entity weapon)
    {
        var ph = weapon.GetComponentData<PhysicsComponent>();
        var stats = creature.GetComponentData<CreatureComponent>();
        var cost = (ph.mass / ((stats.str + stats.agl) * 0.5f) * 100);
        

        return (int)Mathf.Clamp(cost, 1, 50);
    }

    //public static float CalculateMaxLiftWeight(Creature creature)
    //{

    //    return creature.Weight * 0.5f + creature.stats.STR * 2; 
    //}
    //public static bool CanHandleThing(Creature creature, Thing weapon)
    //{
    //    if (weapon == null) return false;
    //    if (CalculateMaxLiftWeight(creature) >= weapon.Weight) return true;
    //    return false;
    //}
    //public static bool CanPushThing(Creature creature, Thing weapon)
    //{
    //    if (weapon == null) return false;
    //    if (CalculateMaxLiftWeight(creature) * 1.5 >= weapon.Weight) return true;
    //    return false;
    //}







}
