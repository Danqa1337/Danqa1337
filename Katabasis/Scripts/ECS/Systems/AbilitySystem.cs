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
using System.Linq;

public enum Ability
{
    Atack,
    Kick,
    Throw,
    MakeStep,
    SpendTime1,
    SpendTime10,
    PicUp,
    Drop,
    Shoot,
    Eat,
    Dig,
}

[DisableAutoCreation]
public class AbilitySystem : ComponentSystem
{
    
    Entity self;
    protected AbilityComponent ability;
    protected AnatomyComponent anatomy;
    protected TileData selfCurrentTile;
    protected EquipmentComponent equipment;
    protected Inventory inventory;
    protected PhysicsComponent physics;
    protected override void OnCreate()
    {

    }
    protected void UpdateValues(Entity entity)
    {
        self = entity;
        if(self.HasComponent<AbilityComponent>()) ability = self.GetComponentData<AbilityComponent>();
        anatomy = self.GetComponentData<AnatomyComponent>();
        selfCurrentTile = self.CurrentTile();
        equipment = self.GetComponentData<EquipmentComponent>();
        inventory = self.GetComponentData<Inventory>();
        physics = self.GetComponentData<PhysicsComponent>();
    }
    public static NativeArray<TileData> current;
    protected async override void OnUpdate()
    {
        float startTime = UnityEngine.Time.realtimeSinceStartup;

        Entities.With(GetEntityQuery
        (
            ComponentType.ReadWrite<AbilityComponent>()
        )).ForEach((Entity entity) =>
        {
            
            
            UpdateValues(entity);
            entity.RemoveComponent<AbilityComponent>();
            if(!entity.HasComponent<IsGoingToSwapTag>() && !entity.HasComponent<ImpulseComponent>())
            {
                
            
                switch (ability.ability)
                {
                    case Ability.Atack:
                        Atack();
                        break;
                    case Ability.Kick:
                        Kick();
                        break;
                    case Ability.Throw:
                        Throw();
                        break;
                    case Ability.MakeStep:
                        MakeStep();
                        break;
                    case Ability.PicUp:
                        PicUp();
                        break;
                    case Ability.Drop:
                        break;
                    case Ability.Shoot:
                        Shoot();
                        break;
                    case Ability.Eat:
                        Eat();
                        break;
                    case Ability.SpendTime1:
                        SpendTime(1);
                        break;
                    case Ability.SpendTime10:
                        SpendTime(10);
                        break; 
                    case Ability.Dig:
                        Dig();
                        break;

                    default:
                        break;
                }
            }
            

           // if(entity.HasComponent<AbilityComponent>())PostUpdateCommands.RemoveComponent<AbilityComponent>(self);
            
            ///////////

            
        });


    }
    private void Dig()
    {
        
        if (ability.targetTile.SolidLayer != Entity.Null)
        {
            ability.targetTile.SolidLayer.AddComponentData(new DestroyTag());
            ability.targetTile.SolidLayer = Entity.Null;
            ability.targetTile.Save();
            Spawner.Spawn("RockDebris", selfCurrentTile.position);
            TempObjectSystem.SpawnTempObject(TempObjectType.Dust, selfCurrentTile);
            TempObjectSystem.SpawnTempObject(TempObjectType.Dust, ability.targetTile);

        }


        MakeStep();
    }
    private void DrawAtackAnimation(float2 direction, float2 position, Entity weapon)
    {
        //var tempEntity = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntity();
        //switch (@enum)
        //{
            
        //}
        //tempEntity.AddComponentData();
    }
    public virtual void Atack()
    {
        Debug.Log(self.GetName() + " atacks"!);

        var equip = self.GetComponentData<EquipmentComponent>();



        Entity weapon = self.GetComponentData<CreatureComponent>().fists;


        if (equip.itemInMainHand != Entity.Null)
        {
            weapon = equip.itemInMainHand;
        }
        if (self.CurrentTile().visible)
        {
            DrawAtackAnimation(ability.targetTile - self.CurrentTile(), self.CurrentTile().position, weapon);
        }


        if (BaseMethodsClass.Chance( DamageCalculator.CalculateTOH(self, weapon)))
        {
            var bonusDamage = (int)DamageCalculator.CalculateBonusDamage(self, weapon);
            var H = 1;
            var axelerationVector = ability.targetTile - self.CurrentTile();

            weapon.AddComponentData(new ImpulseComponent(axelerationVector, bonusDamage, H, self));
        }

        AnimationSystem.AddAnimation(self, new AnimationElement(selfCurrentTile, ability.targetTile, AnimationType.Butt));


        var cost = DamageCalculator.CalculateAttackCost(self, weapon);
        EffectSystem.AddEffect(self, EffectType.EngagedInBattle, 25);
        SpendTime(cost);

    }
    public virtual void Throw()
    {
        
        EffectSystem.AddEffect(self, EffectType.EngagedInBattle, 25);
        if(equipment.itemInMainHand == ability.targetEntity)
        {
            equipment.UnequipItem(ability.targetEntity);
            self.CurrentTile().Drop(ability.targetEntity);
        }
        


        var bonusDamage  = (int)(DamageCalculator.CalculateBonusDamage(self, ability.targetEntity) * ability.targetEntity.GetComponentData<PhysicsComponent>().aerodynamicsDamageMultiplier);
        var H = Mathf.RoundToInt(selfCurrentTile.GetDistance(ability.targetTile) - 0.4f);
        var axelerationVector = math.normalize(ability.targetTile - self.CurrentTile()) + UnityEngine.Random.insideUnitCircle.ToFloat2() * Engine.i.baseRangedInaccuracity;

        ability.targetEntity.AddComponentData(new ImpulseComponent(axelerationVector, bonusDamage, H, self));
        AnimationSystem.AddAnimation(self, new AnimationElement(selfCurrentTile, ability.targetTile, AnimationType.Butt));

        var cost = DamageCalculator.CalculateAttackCost(self, ability.targetEntity)*1.5f;
        if (inventory.items.Count > 0)
        {
            equipment.EquipItem(inventory.items.First(), EquipPice.Weapon);
        }
        Debug.Log(self.GetName() + " throws " + ability.targetEntity.GetName());
        SpendTime((int)cost);
    }
    
    public virtual void Kick()
    {
        EffectSystem.AddEffect(self, EffectType.EngagedInBattle, 25);

        Entity target = ability.targetEntity;

        if (target != Entity.Null)
        {

            var V = DamageCalculator.CalculateBonusDamage(self, ability.targetEntity);
            if (V > 0)
            {
                var H = 1;
                var axelerationVector = ability.targetTile - self.CurrentTile();


                ability.targetEntity.AddComponentData(new ImpulseComponent(axelerationVector, V, H, self));
                AnimationSystem.AddAnimation(self, new AnimationElement(selfCurrentTile, ability.targetEntity.CurrentTile(), AnimationType.Butt));
            }
        }
        
        SpendTime(15);
    } 
    public virtual void PicUp()
    {
        ability.targetEntity.CurrentTile().Remove(ability.targetEntity);
        var equip = self.GetComponentData<EquipmentComponent>();
        if (equip.itemInMainHand == Entity.Null)
        {
            equip.EquipItem(ability.targetEntity, EquipPice.Weapon, true);
        }
        else
        {
            var inventory = self.GetComponentData<Inventory>();
            inventory.Add(ability.targetEntity);
        }
        

    }
    public virtual void Shoot()
    {
        var weapon = equipment.itemInMainHand.GetComponentData<RangedWeaponComponent>();
        var projectile = Entity.Null;
        if (inventory.HasItem("Arrow", out projectile))
        {
            if (weapon.Ready)
            {
               

                EffectSystem.AddEffect(self, EffectType.EngagedInBattle, 25);

                weapon.CurrentReloadPhase = 0;
                equipment.itemInMainHand.SetComponentData(weapon);
                inventory.DropOut(projectile);
                var projectilePhysics = projectile.GetComponentData<PhysicsComponent>();
                var vector = math.normalize(ability.targetTile - selfCurrentTile) + UnityEngine.Random.insideUnitCircle.ToFloat2() * Engine.i.baseRangedInaccuracity;
                var H = Mathf.RoundToInt(selfCurrentTile.GetDistance(ability.targetTile));
                var Impulse = new ImpulseComponent(vector, (int)(DamageCalculator.CalculateBonusDamage(self, projectile)
                                                                * projectilePhysics.aerodynamicsDamageMultiplier) + weapon.Power,
                    H, self);
                projectile.AddComponentData(Impulse);
                projectile.GetComponentObject<EntityAuthoring>().trail.emitting = true;
                var transform = projectile.GetComponentObject<Transform>();
                transform.rotation = quaternion.LookRotationSafe(Vector3.forward, new Vector3(vector.x, vector.y, 0).normalized);
                SpendTime(10);


               



            }
            else
            {

                ReloadWeapon();

            }
        }
        else
        {
            PopUpCreator.i.CreatePopUp(self.GetComponentObject<Transform>(), "I have no arrows");
        }
    }

    public virtual void ReloadWeapon()
    {

        var weapon = equipment.itemInMainHand.GetComponentData<RangedWeaponComponent>();
        if(!weapon.Ready)
        {
            weapon.CurrentReloadPhase = math.min(weapon.CurrentReloadPhase + 1, weapon.MaxReloadingPhases);
            equipment.itemInMainHand.SetComponentData(weapon);
            
            if(weapon.Ready)PopUpCreator.i.CreatePopUp(self.GetComponentObject<Transform>(), "Loaded!",Color.yellow);
            else
            {
                PopUpCreator.i.CreatePopUp(self.GetComponentObject<Transform>(), PopupType.Interaction);
            }
            SpendTime(5);
        }
    }
    public async virtual Task MakeStep()
    {
        if (ability.targetTile != self.CurrentTile())
        {
            if (ability.targetTile.isWalkable(self))
            {
                var solid = ability.targetTile.SolidLayer;
                if (solid.CanBeSwapedRightNow(self))
                {
                    //swap creatures

                    if (solid.HasComponent<AbilityComponent>())
                    {
                        solid.SetComponentData(new AbilityComponent(Ability.SpendTime10));
                    }
                    if(solid.HasComponent<IsGoingToSwapTag>())
                    {
                        solid.RemoveComponent<IsGoingToSwapTag>();
                    }

                    self.AddComponentData(new IsGoingToSwapTag(solid));
                    await SpendTime(self.GetComponentData<CreatureComponent>().baseMovementCost);
                }
                else
                {
                    if (solid != Entity.Null)
                    {
                        if (self.HasTag(Tag.Digger))
                        {
                            Dig();
                        }
                    }
                    else
                    {
                        self.AddComponentData(new MoveComponent(selfCurrentTile, ability.targetTile,
                        MovemetType.SelfPropeled));

                        Debug.Log(self.GetName() + " makes Step");

                        await SpendTime(self.GetComponentData<CreatureComponent>().baseMovementCost);
                    }


                }  
            }
            
        }
    }

    public virtual void Eat()
    {

        var drop = new List<Entity>();
        foreach (var obj in selfCurrentTile.DropLayer)
        {
            foreach (var part in obj.GetComponentData<AnatomyComponent>().GetLoverHierarchy())
            {
                drop.Add(part);
            }
        }


        drop = drop.Where(d => d.HasComponent<EatableComponent>()).ToList();

        if(drop.Count > 0)
        {
            drop = drop.OrderBy(d => d.GetComponentData<PhysicsComponent>().mass).ToList();
            var item = drop[0];
            if (item.HasBuffer<EffectOnConsumptionElement>())
            {
                var effectsOnCons = item.GetBuffer<EffectOnConsumptionElement>();
                if (true)
                {
                    foreach (var effect in effectsOnCons)
                    {
                        EffectSystem.AddEffect(self, effect.effect);
                    }
                }
            }

            item.GetComponentObject<EntityAuthoring>().Desolve();
            {
                var nutrition = item.GetComponentData<EatableComponent>().nutrition;
                self.SetComponentData(new DurabilityChangedOnThisTick(nutrition));
                item.AddComponentData(new DestroyTag());
                item.CurrentTile().Remove(item);
                //var joint = item.GetComponentData<AnatomyComponent>().GetParentJoint();
                //if (joint != Joint.Null)
                //{
                //    joint.Destroy();
                //    selfCurrentTile.Drop(item);
                //}


                if (item.HasComponent<InternalLiquidComponent>())
                {
                    for (int i = 0; i < 3; i++)
                    {
                        if (BaseMethodsClass.Chance(15))
                        {
                            Spawner.Spawn(item.GetComponentData<InternalLiquidComponent>().liquidSpaltterId,
                                self.CurrentTile().GetNeibors(true).RandomItem().position);
                        }
                    }
                }
                Debug.Log(self.GetName() + " eats " + item.GetName());
                SpendTime(20);
               
            }
        }
        else
        {
            PopUpCreator.i.CreatePopUp(self.GetComponentObject<Transform>(), "there is nothing to eat");
            Debug.Log("there is nothing to eat");
        }
            
        
    }
    public virtual async Task SpendTime(int ticks)
    {
        {
            var ai = self.GetComponentData<AIComponent>();
            ai.abilityCooldown += ticks;
            self.SetComponentData(ai);
        }


    }

}
