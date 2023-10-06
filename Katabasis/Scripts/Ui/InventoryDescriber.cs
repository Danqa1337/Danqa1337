using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Entities;

public class InventoryDescriber : Describer
{
    public Entity item;
    public Counter damageCounter;
    public Counter baseAtackDelayCounter;
    public Counter scalingSTRCounter;
    public Counter sclingAGLcounter;
    public Counter resistCounter;
    public Label Name;
    public Label description;

    public static InventoryDescriber i { get; private set; }
    private void Awake()
    {
        i = this;
        Clear();
    }
    public void Describe()
    {
        if(item!= Entity.Null)
        {
            Describe(item);
        }
    }
    public void Describe(Entity entity)
    {
        item = entity;
        var physics = entity.GetComponentData<PhysicsComponent>();
        damageCounter.SetText(physics.damage + " + " + DamageCalculator.CalculateBonusDamage(PlayerAbilitiesSystem.playerEntity, entity));
        baseAtackDelayCounter.SetValue(physics.mass);
        scalingSTRCounter.SetText(physics.ScalingSTR.ToString());
        sclingAGLcounter.SetText(physics.ScalingAGL.ToString());
        resistCounter.SetValue(physics.resistance);
        var descr = entity.GetComponentData<DescriptionComponent>();
        description.SetText(descr.Description);
        Name.SetText(descr.Name);
    }
    public void Clear()
    {
        Name.Clear();
        damageCounter.Clear();
        baseAtackDelayCounter.Clear();
        description.Clear();
        scalingSTRCounter.Clear();
        sclingAGLcounter.Clear();
        resistCounter.Clear();
    }

}
