using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = System.Random;

[DisableAutoCreation]
public class CollisionSystem : ComponentSystem
{
    protected override void OnUpdate()
    {

        float startTime = UnityEngine.Time.realtimeSinceStartup;
        var collisionQuery = GetEntityQuery
        (
            ComponentType.ReadOnly<Collision>()
        );
        Entities.With(collisionQuery

        ).ForEach((Entity e, ref Collision collision) =>
        {





            var entity1 = definePart(collision.entity1);
            var entity2 = definePart(collision.entity2);

            var dmgToFirstDR = 0f;
            var dmgToSecondDR = 0f;
            var bonus1 = 0f;
            var bonus2 = 0f;

            ProcessCollision(entity1,entity2, collision.responsibleEntity, out dmgToSecondDR, out bonus1);
            ProcessCollision(entity2,entity1, collision.responsibleEntity, out dmgToFirstDR, out bonus2);


            spawnDust(entity1);
            spawnDust(entity2);

            Debug.Log(entity1.GetName() + " ( " + bonus2 + ") collided with "
                      + entity2.GetName() + " ( " + bonus1 + ")." +
                      " Each took " + dmgToSecondDR + " and " + dmgToFirstDR + " damage");

            if (collision.responsibleEntity == PlayerAbilitiesSystem.playerEntity)
            {
                if (!collision.entity1.HasComponent<DamagedByPlayerTag>())
                {
                    collision.entity1.AddComponentData(new DamagedByPlayerTag());
                }

                if (!collision.entity2.HasComponent<DamagedByPlayerTag>())
                {
                    collision.entity2.AddComponentData(new DamagedByPlayerTag());
                }
            }







            Entity definePart(Entity entity)
            {
                float bestScore = 0;

                var bestPart = entity;
                if (entity != PhysicsSystem.Ground)
                {
                    var partList = entity.GetComponentData<AnatomyComponent>().GetLoverHierarchy();


                    foreach (var part in partList)
                    {
                        float currentScore = 1;
                        var anatomy = part.GetComponentData<AnatomyComponent>();
                        var ph = part.GetComponentData<PhysicsComponent>();

                        currentScore += GetBodyPartTypeBonus(anatomy.bodyPartTag);
                        currentScore += (int) ph.size;
                        currentScore *= BaseMethodsClass.GenerateNormalRandom(1f, 1f);

                        if (currentScore > bestScore)
                        {
                            bestScore = currentScore;
                            bestPart = part;
                        }
                    }
                }

                Debug.Log("the best part inside " + entity.GetName() + " hierarchy is " + bestPart.GetName() +
                          ", it wins with score of " + bestScore);
                return bestPart;

                float GetBodyPartTypeBonus(BodyPartTag bodyPartTag) => bodyPartTag switch
                {
                    BodyPartTag.None => 0,
                    BodyPartTag.Head => 1,
                    BodyPartTag.RightArm => 1,
                    BodyPartTag.LeftArm => 2,
                    BodyPartTag.RightFrontLeg => 1,
                    BodyPartTag.RightRearLeg => 1,
                    BodyPartTag.LeftFrontLeg => 1,
                    BodyPartTag.LeftRearLeg => 1,
                    BodyPartTag.RightFrontPaw => 1,
                    BodyPartTag.RightRearPaw => 1,
                    BodyPartTag.LeftFrontPaw => 1,
                    BodyPartTag.LeftRearPaw => 1,
                    BodyPartTag.Body => 2.5f,
                    BodyPartTag.Tail => 1,
                    BodyPartTag.Tentacle => 1,
                    BodyPartTag.FirstTentacle => 1,
                    BodyPartTag.SecondTentacle => 1,
                    BodyPartTag.Fin => 1,
                    BodyPartTag.FirstFin => 1,
                    BodyPartTag.SecondFin => 1,
                    BodyPartTag.Teeth => 0,
                    BodyPartTag.RightClaw => 1,
                    BodyPartTag.LeftClaw => 1,
                    BodyPartTag.Fists => 0,
                    BodyPartTag.LowerBody => 1,
                    _ => throw new ArgumentOutOfRangeException(nameof(bodyPartTag), bodyPartTag, null)
                };



            }

            void spawnDust(Entity entity)
            {
                if(entity != PhysicsSystem.Ground && !entity.CurrentTile().isAbyss)
                {
                    if (entity.HasComponent<ImpulseComponent>() && entity.GetComponentData<AnatomyComponent>().GetRootPart() == entity)
                    {
                        TempObjectSystem.SpawnTempObject(TempObjectType.Dust, entity.CurrentTile());

                    }
                }
            }

            //entity.SetComponentData(health);
            //entity.SetComponentData(anatomy);


        });

        PostUpdateCommands.DestroyEntitiesForEntityQuery(collisionQuery);

        //Debug.Log((UnityEngine.Time.realtimeSinceStartup - startTime) * 1000f);

    }

    void ProcessCollision(Entity passiveEntity, Entity activeEntity, Entity responsibleEntity, out float damage, out float bonusDamage)
    {
        var activeAnatomy = activeEntity.GetComponentData<AnatomyComponent>();
        var passiveAnatomy = passiveEntity.GetComponentData<AnatomyComponent>();

        bonusDamage = 0f;
        float resistance = 0;

       
        var activeEntityPhysics = activeEntity.GetComponentData<PhysicsComponent>();
        var passiveEntityPhysics = passiveEntity.GetComponentData<PhysicsComponent>();

        if (!activeEntity.HasEffect(EffectType.ArmourIgnoring))
        {
            resistance = passiveEntityPhysics.resistance;

            var root = passiveEntity.GetComponentData<AnatomyComponent>().GetRootPart();
            if (root.HasComponent<EquipmentComponent>())
            {
                var equip = root.GetComponentData<EquipmentComponent>();
                Entity[] equipPieces = new Entity[] { equip.helmet, equip.chestPlate, equip.itemInOffHand };
                foreach (var piece in equipPieces)
                {
                    if (piece != Entity.Null && activeEntity != piece)
                    {   
                        resistance += piece.GetComponentData<PhysicsComponent>().resistance;
                    }
                }
                        

            }

            resistance = UnityEngine.Random.Range(0, resistance);
        }


        foreach (var part in activeAnatomy.GetUpperHierarchy())
        {
            if (part.HasComponent<ImpulseComponent>())
            {
                bonusDamage += part.GetComponentData<ImpulseComponent>().bonusDamage;
                PostUpdateCommands.RemoveComponent<ImpulseComponent>(part);

                damage = (int)Math.Max(0, activeEntityPhysics.damage + bonusDamage - resistance);

                DurabilitySystem.ChangeDurability(passiveEntity, (int)-damage);
                //if(passiveEntity.HasComponent<DurabilityChangedOnThisTick>())
                //{
                //    var change = passiveEntity.GetComponentData<DurabilityChangedOnThisTick>();
                //    change.value -= (int)damage;
                //    PostUpdateCommands.SetComponent(passiveEntity, change);
                //}
                //else
                //{
                //    var change = new DurabilityChangedOnThisTick(-(int)damage);

                //    PostUpdateCommands.AddComponent(passiveEntity, change);

                //}
                if (passiveAnatomy.GetParentJoint() != Joint.Null)
                {
                    passiveAnatomy.GetParentJoint().TryToBreak(damage,responsibleEntity);
                }

                break;
            }
        }



        
        if(activeEntity.HasBuffer<EffectOnConsumptionElement>() && passiveEntity != PhysicsSystem.Ground)
        {
            foreach (var effect in activeEntity.GetBuffer<EffectOnConsumptionElement>())
            {
                EffectSystem.AddEffect(passiveEntity, effect.effect);
            }
                    
        }

        if(passiveEntity.HasTag(Tag.ExtraFragile))
        {
            passiveEntity.SetComponentData(new DestroyTag());
        }
        if(passiveEntity.HasTag(Tag.Explosive))
        {
            passiveEntity.SetComponentData(new DestroyTag());
            ExplosionSystem.SchaduleExplosion(passiveEntity.CurrentTile(), 100);
        }               




        
            
        

        damage = 0;
        bonusDamage = 0;
    }
}
