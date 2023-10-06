using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using System.Linq;
using Unity.Jobs;
using Random = UnityEngine.Random;

public abstract class IAIBehaviour
{
    public const int moraleFleeTreshold = 10;
    public abstract float Evaluate();
    public abstract void Execute();

    public virtual void DoPathfinderJob()
    {

    }
    public Entity self;
    public Entity target => aIComponent.target;
    public float morale => aIComponent.morale;
    public AIComponent aIComponent;
    public CreatureComponent creatureComponent;
    public EquipmentComponent equipment;
    public Inventory inventory;
    public TileData currentTile;
    public PathFinderQuery query;
    public void Init(Entity self)
    {
        this.self = self;
        aIComponent = self.GetComponentData<AIComponent>();
        creatureComponent = self.GetComponentData<CreatureComponent>();
        equipment = self.GetComponentData<EquipmentComponent>();
        inventory = self.GetComponentData<Inventory>();

        currentTile = self.CurrentTile();

    }
}

public class ChaseBehaviour : IAIBehaviour
{


    public override float Evaluate()
    {
        if (target != Entity.Null)
        {
            return 0.5f;
        }
        return -1;
    }

    public override void DoPathfinderJob()
    {

        base.DoPathfinderJob();

        Debug.Log("!");
        query = new PathFinderQuery(currentTile, target.CurrentTile(), self, true, 100);
        
        Debug.Log("!");
        Pathfinder.FindPath(query);
        
    }

    public override void Execute()
    {

        if (target != Entity.Null)
        {

            var path = query.Path;
            if (path != PathFinderPath.NUll)
            {
                self.AddComponentData(new AbilityComponent() { ability = Ability.MakeStep, targetTile = path.tiles[0] });
                if (self.HasTag(Tag.Digger))
                {
                    if (path.tiles[0].SolidLayer != Entity.Null && !path.tiles[0].SolidLayer.HasComponent<AIComponent>())
                    {
                        self.SetComponentData(new AbilityComponent() { ability = Ability.Dig, targetTile = path.tiles[0] });

                    }
                }
            }
            else
            {

                self.AddComponentData(new AbilityComponent(Ability.SpendTime10));

            }
        }

    }
}

public class AtackBehaviour : IAIBehaviour
{

    public override float Evaluate()
    {
        if (target != Entity.Null)
        {
            if (self.CurrentTile().GetNeibors(true).ToList().Contains(target.CurrentTile()))
            {
                return 1;
            }

        }
        return -1;
    }

    public override void Execute()
    {
        self.AddComponentData(new AbilityComponent() { ability = Ability.Atack, targetTile = target.CurrentTile() });
    }
}
public class StepBackBehaviour : IAIBehaviour
{
    private TileData stepTile;
    public override float Evaluate()
    {
        if (self.GetComponentData<AIComponent>().target != Entity.Null)
        {
            Entity target = self.GetComponentData<AIComponent>().target;
            if (self.CurrentTile().GetNeibors(true).Contains(target.CurrentTile()))
            {
                var tiles = self.CurrentTile().GetNeibors(true).Where(t =>
                    t.isWalkable(self) &&
                    !target.CurrentTile().GetNeibors(true).Contains(t)).ToList();

                if (tiles.Count > 0)
                {
                    stepTile = tiles.RandomItem();
                    if (self.GetComponentData<CreatureComponent>().GetHealthPercent < 60 && BaseMethodsClass.Chance(10)) return 1.1f;
                    return 0.5f;
                }
            }

        }
        return -1;
    }

    public override void Execute()
    {

        self.AddComponentData(new AbilityComponent(Ability.MakeStep, stepTile));
    }
}
public class StepAsideBehaviour : IAIBehaviour
{
    private TileData stepTile;
    public override float Evaluate()
    {
        if (self.GetComponentData<AIComponent>().target != Entity.Null && BaseMethodsClass.Chance(1000f / self.GetComponentData<CreatureComponent>().baseMovementCost))
        {
            Entity target = self.GetComponentData<AIComponent>().target;
            if (self.CurrentTile().GetNeibors(true).Contains(target.CurrentTile()))
            {
                var tiles = self.CurrentTile().GetNeibors(true).Where(t =>
                    t.isWalkable(self) &&
                    target.CurrentTile().GetNeibors(true).Contains(t)).ToList();

                if (tiles.Count > 0)
                {
                    stepTile = tiles.RandomItem();
                    if (BaseMethodsClass.Chance(10)) return 1.1f;
                    return 0.5f;
                }
            }

        }
        return -1;
    }

    public override void Execute()
    {

        self.AddComponentData(new AbilityComponent(Ability.MakeStep, stepTile));
    }
}
public class KickBehaviour : IAIBehaviour
{
    public override float Evaluate()
    {
        throw new System.NotImplementedException();
    }

    public override void Execute()
    {
        throw new System.NotImplementedException();
    }
}
public class PicupWeaponBehaviour : IAIBehaviour
{
    private Entity NewWeapon;
    private Entity OldWeapon;
    
    public override float Evaluate() 
    {
        if (BaseMethodsClass.Chance(10) && equipment.CanUseEquipPice(EquipPice.Weapon))
        { 
            
            if(equipment.itemInMainHand != Entity.Null)
            {
                OldWeapon = equipment.itemInMainHand;
            }
            else
            {
                OldWeapon = creatureComponent.fists;
            }
            var bestDPT = DamageCalculator.CalculateDPT(self, OldWeapon); ;
            foreach (var tile in currentTile.GetNeibors(true))
            {
                foreach (var item in tile.DropLayer)
                {
                    if (item.HasTag(Tag.Crafted))
                    {
                        var dpt = DamageCalculator.CalculateDPT(self, item);
                        if(dpt > bestDPT)
                        {
                            NewWeapon = item;
                        }
                    }
                }
            }
            if(NewWeapon != Entity.Null)
            {
                return 3;
            }
        }
        return -1;
    }

    public override void Execute()
    {
        if(OldWeapon != Entity.Null)
        {
            equipment.UnequipItem(OldWeapon);
        }
        self.AddComponentData(new AbilityComponent(Ability.PicUp, NewWeapon));
    }
}
public class StepForwardBehaviour : IAIBehaviour
{
    private TileData stepTile;
    public override float Evaluate()
    {
        if (morale > moraleFleeTreshold && target != Entity.Null)
        {
            if (!self.CurrentTile().GetNeibors(true).Contains(target.CurrentTile()))
            {
                var tiles = self.CurrentTile().GetNeibors(true).Where(t =>
                    t.SolidLayer == Entity.Null
                    && t.isWalkable(self)
                    && t.GetNeibors(true).Contains(target.CurrentTile())).ToList();


                if (tiles.Count > 0)
                {
                    stepTile = tiles.RandomItem();
                    return 2f;
                }
            }

        }
        return -1;
    }

    public override void Execute()
    {

        self.AddComponentData(new AbilityComponent(Ability.MakeStep, stepTile));
    }
}

public class WaitBehaviour : IAIBehaviour
{

    public override float Evaluate()
    {
        return 0;
    }

    public override void Execute()
    {
        self.AddComponentData(new AbilityComponent() { ability = Ability.SpendTime10 });
    }
}

public class RoamBehaviour : IAIBehaviour
{
    public override float Evaluate()
    {
        if (target == Entity.Null && BaseMethodsClass.Chance(300f / creatureComponent.baseMovementCost) && self.CurrentTile().GetNeibors(true).Where(t => t.isWalkable(self)).ToList().Count > 0)
        {
            return 0.1f;
        }

        return -1;
    }

    public override void Execute()
    {
        var tile = self.CurrentTile().GetNeibors(true).Where(t => t.isWalkable(self)).ToList().RandomItem();
        self.AddComponentData(new AbilityComponent(Ability.MakeStep, tile));

    }
}

public class ShootBehaviour : IAIBehaviour
{
    public override float Evaluate()
    {
        if (target != Entity.Null)
        {
            if (self.GetComponentData<EquipmentComponent>().itemInMainHand.HasComponent<RangedWeaponComponent>())
            {
                if (self.GetComponentData<EquipmentComponent>().itemInMainHand.GetComponentData<RangedWeaponComponent>()
                    .Ready)
                {
                    if (inventory.HasItem("Arrow"))
                    {
                        if (self.CurrentTile().ClearTraectory(target.CurrentTile()))
                        {
                            return 2;
                        }
                    }
                }
            }
        }

        return -1;
    }

    public override void Execute()
    {
        self.AddComponentData(new AbilityComponent(Ability.Shoot, target.CurrentTile()));

    }
}
public class ThrowBehaviour : IAIBehaviour
{
    public override float Evaluate()
    {
        if (target != Entity.Null)
        {
            if (equipment.itemInMainHand != Entity.Null)
            {
                if(inventory.items.Count > 0)
                {
                    if (!currentTile.GetNeibors(true).Contains(target.CurrentTile()) && currentTile.ClearTraectory(target.CurrentTile()))
                    {
                        return 1;
                    }
                }
            }
        }

        return -1;
    }

    public override void Execute()
    {
        self.AddComponentData(new AbilityComponent(Ability.Throw,  target.CurrentTile(), equipment.itemInMainHand , Power: math.max(15, creatureComponent.viewDistance)));

    }
}
public class ReloadBehaviour : IAIBehaviour
{
    public override float Evaluate()
    {
        if (self.GetComponentData<EquipmentComponent>().itemInMainHand.HasComponent<RangedWeaponComponent>())
        {
            if (!self.GetComponentData<EquipmentComponent>().itemInMainHand.GetComponentData<RangedWeaponComponent>()
                .Ready)
            {
                return 1.1f;
            }
        }

        return -1;
    }

    public override void Execute()
    {
        self.AddComponentData(new AbilityComponent(Ability.Shoot, TileData.Null));


    }
}
public class FleeBehaviour : IAIBehaviour
{
    TileData[] freeTiles;
    public override float Evaluate()
    {
        if (target != Entity.Null && morale < 50)
        {
            freeTiles = currentTile.GetNeibors(true).Where(t => t.isWalkable(self)).ToArray();
            if(freeTiles.Any(t => !t.GetNeibors(true).Contains(target.CurrentTile())))
            {
                return 2;
            }



        }
        return -1;
    }

    public override void Execute()
    {
        var tileToFlee = freeTiles.OrderBy(t => 1 / t.GetSqrDistance(target.CurrentTile())).ToList()[0];
        self.AddComponentData(new AbilityComponent(Ability.MakeStep, tileToFlee));

    }
}




