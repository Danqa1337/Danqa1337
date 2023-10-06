using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using UnityEngine.UI;

[DisableAutoCreation]
public class PhysicsSystem : ComponentSystem
{
    public static Entity Ground
    {
        get
        {
            return _ground;
        }

    }
    private static Entity _ground;
    protected override void OnCreate()
    {
        Init();
    }

    public static void Init()
    {
        _ground = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntity();
        _ground.SetName("Ground");
        _ground.AddComponentData(new DescriptionComponent("Ground"));
        _ground.AddComponentData(new DurabilityComponent(-1, Ground));
        _ground.AddComponentData(new AnatomyComponent(Ground, BodyPartTag.Body));
        _ground.AddComponentData(new PhysicsComponent() { damage = 0.5f, mass = int.MaxValue });
        _ground.AddBuffer<TagBufferElement>();
    }

    protected override void OnUpdate()
    {

        EntityQuery query = GetEntityQuery 
        (
            ComponentType.ReadWrite<ImpulseComponent>()
        );
        Entities.With(query).ForEach((Entity entity, ref ImpulseComponent impulseComponent) =>
        {
            
            var currentTile = entity.CurrentTile();

           

            if (impulseComponent.H > 0 &&  !impulseComponent.axelerationVector.Equals( float2.zero) && !entity.GetTags().Contains(Tag.Unmovable))// && !physicsData.axelerationVector.Equals(float2.zero))
            {

                TileData nextTile = impulseComponent.calculateNextTile(currentTile, out impulseComponent.err);
                if(entity.GetTags().Contains(Tag.Projectile) && entity.GetComponentData<AnatomyComponent>().GetParentJoint() == Joint.Null )
                {
                    var transform = entity.GetComponentObject<Transform>();
                    transform.rotation = Quaternion.LookRotation(Vector3.forward, impulseComponent.axelerationVector.ToRealPosition());
                }    

                if (nextTile.SolidLayer != Entity.Null) // collision
                {

                    currentTile.impact = true;
                    currentTile.Save();

                    var obstacle = nextTile.SolidLayer;

                    // var entityHitPart = entity.GetComponentData<AnatomyComponent>().GetbodyParts();
                    var collisionEntity = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntity();
                    collisionEntity.SetName(entity.GetName() + " and " + obstacle.GetName() + " collision");

                    Collision collision = new Collision(entity, obstacle, impulseComponent.responsibleEntity);
                    PostUpdateCommands.AddComponent(collisionEntity, collision);


                }
                else // no collision
                {
                    if (entity.HasComponent<VirtualBodypart>()) // is virtual part
                    {
                        
                        PostUpdateCommands.RemoveComponent<ImpulseComponent>(entity);

                    }
                    else
                    {
                        //if(entity.GetComponentData<AnatomyComponent>().GetParentJoint() != Joint.Null) //is attached to something
                        //{
                        //    entity.GetComponentData<AnatomyComponent>().GetParentJoint().TryToBreak(impulseComponent.V, impulseComponent.responsibleEntity);
                        //}


                        if(entity.GetComponentData<AnatomyComponent>().GetParentJoint() == Joint.Null) //free
                        {

                            impulseComponent.H--;


                            var moveComponent = new MoveComponent(currentTile, nextTile, MovemetType.Forced);
                            if (entity.HasComponent<MoveComponent>())
                            {
                                entity.SetComponentData(moveComponent);
                            }
                            else
                            {
                                PostUpdateCommands.AddComponent(entity, moveComponent);
                            }




                        }
                        else //is still attached
                        {
                            PostUpdateCommands.RemoveComponent<ImpulseComponent>(entity);

                        }
                    }
                }
            }
            else
            {
                if(impulseComponent.bonusDamage >= 1)
                {
                    var collisionEntity = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntity();
                    collisionEntity.SetName(entity.GetName() + " and Ground collision");

                    Collision collision = new Collision(entity, Ground, impulseComponent.responsibleEntity);
                    PostUpdateCommands.AddComponent(collisionEntity, collision);
                }
                else
                {
                    PostUpdateCommands.RemoveComponent<ImpulseComponent>(entity);
                }

            }
        });   
    }

   
}


