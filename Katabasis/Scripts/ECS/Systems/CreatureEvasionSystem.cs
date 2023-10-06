using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using System.Linq;

[DisableAutoCreation]
public class CreatureEvasionSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAll<CreatureComponent>().WithNone<MoveComponent>().WithNone<ImpulseComponent>().ForEach(
            (Entity creature, ref CreatureComponent creatureComponent) =>
            {


                if (BaseMethodsClass.Chance(150f / creature.GetComponentData<CreatureComponent>().baseMovementCost) && !creature.GetTags().Contains(Tag.Dummy))
                {
                    bool dangerFound = false;
                    ImpulseComponent dangerImpulse = new ImpulseComponent();

                    Entities.WithAll<ImpulseComponent>().ForEach((Entity projectile, ref ImpulseComponent impulse) =>
                    {
                        if (impulse.calculateNextTile(projectile.CurrentTile()) == creature.CurrentTile() && impulse.responsibleEntity != creature)
                        {
                            dangerFound = true;
                            dangerImpulse = impulse;
                        }
                    });

                    if (dangerFound)
                    {
                        if (dangerImpulse.responsibleEntity == PlayerAbilitiesSystem.playerEntity)
                        {
                            creature.AddComponentData(new DamagedByPlayerTag());
                        }

                        var tiles = creature.CurrentTile().GetNeibors(true).ToList();
                        tiles.Shuffle();
                        foreach (var tile in tiles)
                        {
                            if (tile.isWalkable(creature))
                            {
                                var moveComponent = new MoveComponent
                                {
                                    prevTileId = creature.CurrentTile().index,
                                    nextTileId = tile.index,
                                    movemetType = MovemetType.SelfPropeled
                                };

                                Debug.Log("Evaded!");
                                PostUpdateCommands.AddComponent(creature, moveComponent);
                                break;
                            }
                        }
                    }
                }


            });


    }

    //private bool isDanger(Entity entity)
    //{
      
    //}
}
