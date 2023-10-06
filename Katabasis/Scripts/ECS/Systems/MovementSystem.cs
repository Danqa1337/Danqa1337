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

[DisableAutoCreation]
public class MovementSystem : ComponentSystem
{

    protected override void OnUpdate()
    {
        float startTime = UnityEngine.Time.realtimeSinceStartup;

        Entities.With(GetEntityQuery
        (
            ComponentType.ReadWrite<MoveComponent>(),
            ComponentType.ReadOnly<CurrentTileComponent>()
        )).ForEach((Entity entity, ref CurrentTileComponent currentTileComponent, ref MoveComponent moveComponent) =>
        {
            
            TileData currentTile = moveComponent.prevTileId.ToTileData();
            TileData nextTile = moveComponent.nextTileId.ToTileData();
            AnimationType animationType = AnimationType.PositionChange;
            //Debug.Log(entity.GetName()); 
            if (currentTileComponent.objectType == ObjectType.Hovering || nextTile.SolidLayer == Entity.Null )
            {
                

                //if (currentTile != nextTile)
                {
                   // Debug.Log("!!");
                    switch (moveComponent.movemetType)
                    {
                        case MovemetType.SelfPropeled:
                            animationType = AnimationType.Step;
                            break;
                        case MovemetType.Forced:
                            animationType = AnimationType.Flip;
                            if (entity.GetTags().Contains(Tag.Projectile))
                            {
                                animationType = AnimationType.Flight;
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }


                    switch (currentTileComponent.objectType)
                    {
                        case ObjectType.Solid:
                            if (currentTile.SolidLayer == entity)
                            { 
                                currentTile.SolidLayer = Entity.Null;
                            }
                            else
                            {
                                
                                //throw new Exception("currentTile solid is " + currentTile.SolidLayer.GetName());
                                
                            }
                            nextTile.SolidLayer = entity;

                            break;
                        case ObjectType.Hovering:
                            if(currentTile.HoveringLayer == entity) currentTile.HoveringLayer = Entity.Null;
                            nextTile.HoveringLayer = entity;

                            break;
                        case ObjectType.Drop:
                            TileUpdater.TileDropContainers[currentTile.index].Remove(entity);
                            //Debug.Log(TileUpdater.TileDropContainers[currentTile.index].Contains(entity));
                            TileUpdater.TileDropContainers[nextTile.index].Add(entity);

                            break;
                    
                    }

                

                
                    
                    currentTileComponent.currentTileId = moveComponent.nextTileId;
                    nextTile.updateScheduled = true;
                    currentTile.Save();
                    nextTile.Save();

                    var animationElement = new AnimationElement(currentTile, nextTile, animationType);
                    AnimationSystem.AddAnimation(entity, animationElement);
                    if (entity.HasComponent<LOSBlockTag>() || entity.HasComponent<PlayerTag>()) TileUpdater.FOVUpdateScheduled = true;
                    if(nextTile.visible)
                    {
                        nextTile.ShowUnmapable();
                    }
                    Debug.Log(entity.GetName() + " moves to " + nextTile.position);
                }
                //else
                //{
                //    throw new Exception("trying to move to the same tile");
                //}



            }
            else
            {
                Debug.Log(entity.GetName() + " trying to move to a tile occupied by " + nextTile.SolidLayer.GetName());

            }
            PostUpdateCommands.RemoveComponent<MoveComponent>(entity);
        });
        //Debug.Log((UnityEngine.Time.realtimeSinceStartup - startTime) * 1000f);

    }


}
