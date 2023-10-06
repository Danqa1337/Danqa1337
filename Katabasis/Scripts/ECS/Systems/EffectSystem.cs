using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Burst;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using System.Threading.Tasks;
using System.Reflection;


public enum EffectType
{
    Invisibility,
    Regeneration,
    Bleeding,
    Haste,
    Slowness,
    Damage10,
    Damage20,
    Damage50,
    ArmourIgnoring,
    EngagedInBattle,
    
}
[DisableAutoCreation]
public class EffectSystem : ComponentSystem
{
    protected override void OnCreate()
    {

    }
    public static NativeArray<TileData> current;
    protected override void OnUpdate()
    {
        float startTime = UnityEngine.Time.realtimeSinceStartup;

        Entities.With(GetEntityQuery
        (
            ComponentType.ReadWrite<HasEffectTag> ()
            
        )).ForEach((Entity entity, DynamicBuffer<EffectElement> buffer) =>
        {
            
            
            for (int i = 0; i < buffer.Length; i++)
            {
                var effect = buffer[i];

                
                if (effect.Duration != 0) // effect realisation
                {
                    if (effect.Duration > 0) effect.Duration--;
                    if(effect.CoolDown > 0) effect.CoolDown--;
                    if (effect.ready)
                    {
                        switch (effect.type)
                        {
                            case EffectType.Invisibility:

                                entity.GetComponentObject<RendererComponent>().BecomeInvisible();
                                if(entity.CurrentTile().visible && BaseMethodsClass.Chance(1))
                                {
                                    TempObjectSystem.SpawnTempObject(TempObjectType.InvisParticels, entity.CurrentTile());
                                }



                                break;
                            case EffectType.Regeneration:
                                if (entity.HasComponent<DurabilityComponent>())
                                {
                                    if (entity.HasComponent<DurabilityChangedOnThisTick>())
                                    {
                                        var change = entity.GetComponentData<DurabilityChangedOnThisTick>();
                                        change.value += 15;
                                        PostUpdateCommands.SetComponent(entity, change);
                                    }
                                    else
                                    {
                                        var change = new DurabilityChangedOnThisTick(15);

                                        PostUpdateCommands.AddComponent(entity, change);

                                    }
                                    
                                    effect.CoolDown = 5;
                                    
                                }


                                break;
                            case EffectType.Bleeding:
                                if (entity.HasComponent<DurabilityComponent>())
                                {
                                    if (entity.HasComponent<DurabilityChangedOnThisTick>())
                                    {
                                        var change = entity.GetComponentData<DurabilityChangedOnThisTick>();
                                        change.value -= 5;
                                        PostUpdateCommands.SetComponent(entity, change);
                                    }
                                    else
                                    {
                                        var change = new DurabilityChangedOnThisTick(-5);

                                        PostUpdateCommands.AddComponent(entity, change);

                                    }
                                    effect.CoolDown = 5;
                                    if(BaseMethodsClass.Chance(5))
                                    {
                                        entity.GetComponentData<AnatomyComponent>().SpillLiquid(entity.CurrentTile());
                                    }

                                }
                                break;
                            case EffectType.Haste:
                                break;
                            case EffectType.Slowness:
                                
                                break;

                            case EffectType.Damage10:
                                if (entity.HasComponent<DurabilityComponent>())
                                {
                                    if (entity.HasComponent<DurabilityChangedOnThisTick>())
                                    {
                                        var change = entity.GetComponentData<DurabilityChangedOnThisTick>();
                                        change.value -= 10;
                                        PostUpdateCommands.SetComponent(entity, change);
                                    }
                                    effect.CoolDown = 5;

                                }
                                break;
                            default:
                                break;
                        }
                    }
                    buffer[i] = effect;
                }
                else // end effect
                {
                    switch (effect.type)
                    {
                        case EffectType.Invisibility: entity.GetComponentObject<RendererComponent>().Becomevisible();
                            break;
                        case EffectType.Regeneration:
                            break;
                        case EffectType.Bleeding:
                            break;
                        case EffectType.Haste:
                            break;
                        case EffectType.Slowness:
                            break;
                    }
                    buffer.Remove(effect);

                    if (buffer.Length == 0)
                    {
                        PostUpdateCommands.RemoveComponent<HasEffectTag>(entity);
                        

                    }
                }
            }
            if(entity.IsPlayer())
            {
                PlayerHud.i.UpdateEffects();

            }
        });
    }


    public static void AddEffect(Entity entity, EffectType type)
    {
        int duration = 100;
        switch (type)
        {
            case EffectType.Invisibility: duration = 500;
                break;
            case EffectType.Regeneration:
                break;
            case EffectType.Bleeding:
                break;
            case EffectType.Haste:
                break;
            case EffectType.Slowness:
                break;
            case EffectType.Damage10: duration = 1;
                break;
        }

        var element = new EffectElement(type, entity, duration);
        AddEffect(entity, element);


    }
    public static void AddEffect(Entity entity, EffectType type, int Duration)
    {
        

        var element = new EffectElement(type, entity, Duration);
        AddEffect(entity, element);


    }
    public static void RemoveEffect(Entity entity, EffectType type)
    {
        if(entity.HasEffect(type))
        {
            var buffer = entity.GetBuffer<EffectElement>();
            for (int i = 0; i < buffer.Length; i++)
            {
                if(buffer[i].type == type)
                {
                    var e = buffer[i];
                    e.Duration = 0;
                    buffer[i] = e;
                    break;
                }
            }
        }
    }
    public static void AddEffect(Entity entity, EffectElement effect)
    {
        switch (effect.type)
        {
            case EffectType.Invisibility:
                foreach (var item in entity.GetComponentData<AnatomyComponent>().GetBodypartsWithoutItself())
                {
                    EffectSystem.AddEffect(item, effect);
                }
                break;
            case EffectType.Regeneration:
                break;
            case EffectType.Bleeding:
                break;
            case EffectType.Haste:
                break;
            case EffectType.Slowness:
                break;
        }
        
        if (entity.HasBuffer<EffectElement>())
        {
            if(entity.HasEffect(effect.type))
            {
                var buffer = entity.GetBuffer<EffectElement>();
                for (int i = 0; i < buffer.Length; i++)
                {
                    if (buffer[i].type == effect.type)
                    {
                        var e = buffer[i];
                        e.Duration = math.max(e.Duration, effect.Duration);
                        e.CoolDown = 0;
                        buffer[i] = e;



                        break;
                    }
                }
            }
            else
            {
                entity.GetBuffer<EffectElement>().Add(effect);
            }
            if (!entity.HasComponent<HasEffectTag>())
            {
                entity.AddComponentData(new HasEffectTag());
            }
            if (entity.IsPlayer())
            {
                PlayerHud.i.UpdateEffects();
            }

        }
        else throw new Exception("no efect buffer on entity " + entity.GetName());


    }
}
[System.Serializable]
public struct EffectElement : IBufferElementData
{
    public EffectType type;
    public bool ready => CoolDown == 0;
    public int CoolDown;
    public int Duration;
    public int Level;
    public EffectElement(EffectType type, Entity affectedEntity, int Duration, int Level = 0)
    {
        this.type = type;
        this.Duration = Duration;
        this.Level = Level;
        this.CoolDown = 0;
    }
    public EffectElement(EffectType type,int Duration, int Level = 0)
    {
        this.type = type;
        this.Duration = Duration;
        this.Level = Level;
        this.CoolDown = 0;

    }

}

public struct EffectOnConsumptionElement : IBufferElementData
{
    public EffectType effect;
    public EffectOnConsumptionElement(EffectType effect)
    {
        this.effect = effect;
    }
}
public struct HasEffectTag : IComponentData
{

}
