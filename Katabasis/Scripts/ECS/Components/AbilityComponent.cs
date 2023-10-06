using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct AbilityComponent : IComponentData
{
    public Ability ability;
    public TileData targetTile;
    public Entity targetEntity;
    public int power;

    public AbilityComponent(Ability ability, TileData targetTile, int Power = 0)
    {
        this.ability = ability;
        this.targetTile = targetTile;
        this.targetEntity = Entity.Null;
        this.power = Power;
    }
    public AbilityComponent(Ability ability, int Power = 0)
    {
        this.ability = ability;
        this.targetTile = TileData.Null;
        this.targetEntity = Entity.Null;
        this.power = Power;
    }
    public AbilityComponent(Ability ability, Entity targetEntity, int Power = 0)
    {
        this.ability = ability;
        this.targetTile = TileData.Null;
        this.targetEntity = targetEntity;
        this.power = Power;
    }

    public AbilityComponent(Ability ability, TileData targetTile, Entity targetEntity, int Power = 0)
    {
        this.ability = ability;
        this.targetTile = targetTile;
        this.targetEntity = targetEntity;
        this.power = Power;
    }
}








