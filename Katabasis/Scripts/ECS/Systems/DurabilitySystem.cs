using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using System.Linq;
[DisableAutoCreation]
public class DurabilitySystem : ComponentSystem
{
    public static void ChangeDurability(Entity entity, int num)
    {
        var change = new DurabilityChangedOnThisTick();
        if(entity.HasComponent<DurabilityChangedOnThisTick>())
        {
            change = entity.GetComponentData<DurabilityChangedOnThisTick>();
        }
        change.value += num;
        entity.SetComponentData(change);
    }
    protected override void OnUpdate()
    {
        EntityQuery destroyQuery = GetEntityQuery
        (
            ComponentType.ReadOnly<DurabilityComponent>(),
            ComponentType.ReadOnly<Transform>(),
            ComponentType.ReadOnly<DestroyTag>()
        );
        if (!destroyQuery.IsEmpty)
        {
            Entities.With(destroyQuery)
                .ForEach((Entity entity, Transform transform, ref DurabilityComponent durabilityComponent) =>
                {
                    Destroy(entity);
                });
        }

         destroyQuery = GetEntityQuery
        (
            ComponentType.ReadOnly<DurabilityComponent>(),
            ComponentType.ReadOnly<Transform>(),
            ComponentType.ReadOnly<DestroyImmediatelyTag>()
        );
        if (!destroyQuery.IsEmpty)
        {
            Entities.With(destroyQuery)
                .ForEach((Entity entity, Transform transform, ref DurabilityComponent durabilityComponent) =>
                {
                    DestroyImmediately(entity);
                });
        }


        EntityQuery query = GetEntityQuery
        (
            ComponentType.ReadOnly<DurabilityComponent>(),
            ComponentType.ReadOnly<Transform>(),
            ComponentType.ReadOnly<DurabilityChangedOnThisTick>()
        );
        Entities.With(query).ForEach(
        (Entity entity, Transform transform, ref DurabilityChangedOnThisTick damageAmmount, ref DurabilityComponent durabilityComponent) =>
        {
            var anatomy = entity.GetComponentData<AnatomyComponent>();
            var renderer = entity.GetComponentObject<RendererComponent>();

            PostUpdateCommands.RemoveComponent<DurabilityChangedOnThisTick>(entity);
            Color color = Color.clear;
            if (damageAmmount.value < 0) 
            { 
                color = Color.gray; 
                if (BaseMethodsClass.Chance(20))
                {
                    anatomy.SpillLiquid(entity.CurrentTile());
                }
            }
            if (damageAmmount.value > 0)
            {
                color = Color.gray;
            }
            renderer.DrawDamageAnimation();
            if (!damageAmmount.drawn)
            {
                PopUpCreator.i.CreatePopUp(transform, damageAmmount.value.ToString(), color, 1.6f);
            }

            durabilityComponent.currentDurability = Mathf.Clamp(
                durabilityComponent.currentDurability + damageAmmount.value,
                0,
                durabilityComponent.MaxDurability);

            var playerAnatomy = PlayerAbilitiesSystem.playerEntity.GetComponentData<AnatomyComponent>();



            if (durabilityComponent.currentDurability == 0) //destroy
            {
                Destroy(entity);
            }


        });

        void Destroy(Entity entity)
        {
            if(entity.HasComponent<LOSBlockTag>())
            {
                TileUpdater.FOVUpdateScheduled =true;
            }
            if (entity.HasComponent<PlayerTag>())
            {
                PlayerAbilitiesSystem.Die();
            }
            else
            {
            

                TileData tileData = entity.CurrentTile();
                var anatomy = entity.GetComponentData<AnatomyComponent>();
               
                if (entity.HasComponent<Inventory>())
                {
                    entity.GetComponentData<Inventory>().DropAll();
                }

                var parts = anatomy.GetBodypartsWithoutItself();
                foreach (var part in parts)
                {
                    anatomy.DropPart(part);
                }

                if (anatomy.GetParentJoint() != Joint.Null)
                {
                    anatomy.GetParentJoint().Destroy();
                }
                Debug.Log(entity.GetBuffer<DropElement>().Length);
                foreach (var item in entity.GetBuffer<DropElement>())
                {
                    Spawner.ScheduleSpawn(ItemDataBase.GetObject(item.id), entity.CurrentTile().position);
                }

                var authoring = entity.GetComponentObject<EntityAuthoring>();

                if (entity.HasComponent<InternalLiquidComponent>())
                {
                    if(!entity.CurrentTile().isAbyss) anatomy.SpillLiquid(entity.CurrentTile());

                    var tiles = entity.CurrentTile().GetNeibors(true).ToList();//
                    tiles.Add(entity.CurrentTile());
                    tiles = tiles.Where(t=>!t.isAbyss).ToList();
                    foreach (var item in tiles)
                    {
                        if(BaseMethodsClass.Chance(0.25f)) anatomy.SpillLiquid(item);

                    }
                  
                }
                if(entity.HasTag(Tag.Wall))
                {
                  
                }
                tileData.Remove(entity);
                authoring.Desolve();
                PostUpdateCommands.DestroyEntity(entity);
                Debug.Log(entity.GetName() + " is destroyed");
            }
        }

        void DestroyImmediately(Entity entity)
        {
            if (entity.HasComponent<PlayerTag>())
            {
                PlayerAbilitiesSystem.Die();
            }
            else
            {
                entity.CurrentTile().Remove(entity);
                PostUpdateCommands.DestroyEntity(entity);
                Pooler.PutObjectBackToPool(entity.GetComponentObject<EntityAuthoring>().gameObject);
            }
        }
    }
    
}
public struct DurabilityChangedOnThisTick : IComponentData
{
    public int value;
    public bool drawn;
    public DurabilityChangedOnThisTick(int change)
    {
        drawn = false;
        this.value = change;
    }
}
