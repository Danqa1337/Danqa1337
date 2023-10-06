using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
[DisableAutoCreation]
public class HealthSystem : ComponentSystem
{
    protected override void OnUpdate()

    {
        EntityQuery query = GetEntityQuery
        (
            ComponentType.ReadOnly<CreatureComponent>()
        );
        Entities.With(query).ForEach((Entity entity, Transform transform, ref CreatureComponent creatureComponent) =>
        {
            var anatomy = entity.GetComponentData<AnatomyComponent>();
            
            int HealthChange = 0;
            foreach (var part in anatomy.GetLoverHierarchy())
            {
                if (part.HasComponent<DurabilityChangedOnThisTick>())
                {
                    var change = part.GetComponentData<DurabilityChangedOnThisTick>();
                    HealthChange += change.value;
                    change.drawn = true;
                    PostUpdateCommands.SetComponent(part, change);
                }
            }
            creatureComponent.ChangeHealth(HealthChange);

            if (HealthChange != 0)
            {
                Debug.Log(entity.GetName() + " 's health changed " + HealthChange);
                Color color = Color.clear;
                if (HealthChange < 0) color = Color.red;
                if (HealthChange > 0) color = Color.green;
                PopUpCreator.i.CreatePopUp(transform, HealthChange.ToString(), color);
                
            }
            

            if (creatureComponent.currentHealth == 0 
                || anatomy.GetMissingPartTags().Contains(BodyPartTag.Head)
                || anatomy.GetMissingPartTags().Contains(BodyPartTag.LowerBody)
                || anatomy.GetBodyPart(BodyPartTag.Body).GetComponentData<DurabilityComponent>().currentDurability == 0)
            {
                Die();
            }


            void Die()
            {
                TileData tileData = entity.CurrentTile();

                if (!entity.IsPlayer())
                {
                    //Debug.Log("!!");
                    if (entity.HasComponent<DamagedByPlayerTag>())
                    {
                        var xp = entity.GetComponentData<CreatureComponent>().XPOnDeath;
                        XPCounter.AddXP( xp);
                    }
                    PostUpdateCommands.RemoveComponent<AIComponent>(entity);
                    PostUpdateCommands.RemoveComponent<CreatureComponent>(entity);
                    entity.GetComponentData<Inventory>().DropAll();
                    foreach (var part in entity.GetComponentData<EquipmentComponent>().GetEquipment())
                    {
                        tileData.Drop(entity.GetComponentData<EquipmentComponent>().UnequipItem(part));
                    }
                    tileData.SolidLayer = Entity.Null;
                    tileData.Drop(entity);
                    tileData.Save();
                    
                    PopUpCreator.i.CreatePopUp(transform, PopupType.Death);
                    Debug.Log(entity.GetName() + " is dead");
                }
                else
                {
                   PlayerAbilitiesSystem.Die();
                }
            }
        });
    }

}
