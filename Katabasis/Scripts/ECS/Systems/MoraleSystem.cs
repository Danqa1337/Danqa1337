using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
[DisableAutoCreation]
public class MoraleSystem : ComponentSystem
{
    protected override void OnUpdate()

    {
        EntityQuery query = GetEntityQuery
        (
            ComponentType.ReadOnly<CreatureComponent>(),
            ComponentType.ReadOnly<AIComponent>(),
            ComponentType.ReadOnly<Transform>()
        );
        Entities.With(query).ForEach((Entity entity, Transform transform, ref CreatureComponent creatureComponent, ref AIComponent aIComponent) =>
        {
            if (!entity.GetTags().Contains(Tag.Amoral))
            {
                var anatomy = entity.GetComponentData<AnatomyComponent>();

                float MoraleChange = 0;
                foreach (var part in anatomy.GetLoverHierarchy())
                {
                    if (part.HasComponent<DurabilityChangedOnThisTick>())
                    {
                        var change = part.GetComponentData<DurabilityChangedOnThisTick>();
                        MoraleChange += (change.value /2); //(change.value / creatureComponent.MaxHealth) * 100;
                    }
                }
                if (aIComponent.target == Entity.Null)
                {
                    MoraleChange += 1;
                }
                else
                {
                    MoraleChange += 0.4f;
                }


                aIComponent.morale = Mathf.Clamp(aIComponent.morale + MoraleChange, 0, 100);
            }
        });
    }

}
