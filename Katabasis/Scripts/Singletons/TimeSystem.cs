using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;

public static class TimeSystem
{
    public static int Ticks;

    


    private static bool drawNextFarme;
    public static void SheludeFrameDraw()
    {
        drawNextFarme = true;
    }
    private static bool PlayerIsAlive => Controller.controllerState != Controller.ControllerState.dead;

    private static bool debugSystems => Engine.i.debugSystems;
    public static async Task SpendTime(int ticks)
    {

        int t = (int)(ticks * Engine.i.timescale);
        for (int i = 0; i < t; i++)
        {
            await NewTick();
        }
        await World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<AnimationSystem>().UpdateAsync();
        World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<AnimationSystem>().ClearBuffers();
        Selector.i.UpdateTileSelection();

    }
    public static async Task NewTick()
    {
        
        Ticks++;

        


        drawNextFarme = false;
        if (PlayerIsAlive) World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EffectSystem>().Update();
        if (debugSystems) Debug.Log("effects");

        if (PlayerIsAlive) World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<AiSystem>().Update();

        

        if (debugSystems) Debug.Log("ai");



        if (PlayerIsAlive) World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<AbilitySystem>().Update();
        if (debugSystems) Debug.Log("abilities");

        if (PlayerIsAlive) World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<SwapSystem>().Update();
        if (debugSystems) Debug.Log("swap");

        if (PlayerIsAlive) World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<CreatureEvasionSystem>().Update();
        if (debugSystems) Debug.Log("evasion");


        if (PlayerIsAlive) World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<MovementSystem>().Update();
        if (debugSystems) Debug.Log("movement");
        if (PlayerIsAlive) await World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<AnimationSystem>().MinorUpdate();

        if (PlayerIsAlive) World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<PhysicsSystem>().Update();
        if (debugSystems) Debug.Log("physics");

        if (PlayerIsAlive) World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<CollisionSystem>().Update();
        if (debugSystems) Debug.Log("collisions");


        if (PlayerIsAlive) World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<MoraleSystem>().Update();
        if (debugSystems) Debug.Log("morale");

        if (PlayerIsAlive) World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<HealthSystem>().Update();
        if (debugSystems) Debug.Log("health");

        if (PlayerIsAlive) World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<DurabilitySystem>().Update();

        if (debugSystems) Debug.Log("durability");
        if (PlayerIsAlive) Spawner.Update();

        if (debugSystems) Debug.Log("spawner");


        //if (PlayerIsAlive) await World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<AnimationSystem>().UpdateAsync();
        //if (debugSystems) Debug.Log("animation1");


        if (PlayerIsAlive) await TileUpdater.Update();
        if (debugSystems) Debug.Log("tiles");


        if (PlayerIsAlive) ExplosionSystem.Update();
        if (debugSystems) Debug.Log("explosions");
        if (PlayerIsAlive) TempObjectSystem.Update();
        if (debugSystems) Debug.Log("temps");

        //if (PlayerIsAlive) await World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<AnimationSystem>().UpdateAsync();
        //if (debugSystems) Debug.Log("animation2");


        if (PlayerIsAlive) TickCounter.i.SetValue(Ticks);
        
        
    }
}
