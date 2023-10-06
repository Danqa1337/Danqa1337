using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using System.Linq;
using Unity.Jobs;

//public struct EvaluateBehaviourhob : IJobChunk
//{
//    public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
//    {
       
//    }
//}


//[DisableAutoCreation]
//public class AiSystem : ComponentSystem
//{
//    IAIBehaviour[] behaviors; 
//    protected override void OnCreate()
//    {
//        base.OnCreate();
//        behaviors = new IAIBehaviour[]
//        {
//            new WaitBehaviour(),
//            new StepForwardBehaviour(),
//            new StepAsideBehaviour(),
//            new StepBackBehaviour(),
//            new ChaseBehaviour(),
//            new AtackBehaviour(),
//            new RoamBehaviour(),
//            new ThrowBehaviour(),
//            new ShootBehaviour(),
//            new ReloadBehaviour(),
//            new FleeBehaviour(),
//            new PicupWeaponBehaviour(),
//        };
//    }

//    protected override void OnUpdate()
//    {
//        EntityQuery query = GetEntityQuery(

//        ComponentType.ReadOnly<AIComponent>()
//        );
//        PlayerAbilitiesSystem.CreaturesInSight.Clear();
//        Entities.With(query).ForEach((Entity self, ref AIComponent aIComponent, ref CreatureComponent creatureComponent) =>
//        {
//            TileData currentTile = self.CurrentTile();

//            if(currentTile.visible && currentTile.GetSqrDistance(PlayerAbilitiesSystem.playerEntity.CurrentTile()) < Mathf.Pow(creatureComponent.viewDistance + 1, 2) )
//            {
//                //if(PlayerAbilitiesSystem.CreaturesInSight.Contains(entity))
//                {
//                    PlayerAbilitiesSystem.CreaturesInSight.Add(self);
//                   // Debug.Log(entity.GetName() + " is in sight");
//                }
//            }
//            aIComponent.abilityCooldown = Mathf.Max(aIComponent.abilityCooldown - 1, 0);
//            aIComponent.targetSearchCooldown = Mathf.Max(aIComponent.targetSearchCooldown - 1, 0);


//            if (aIComponent.AbilityReady && !self.HasComponent<ImpulseComponent>())
//            {

//                if (aIComponent.targetSearchCooldown == 0 || aIComponent.target == Entity.Null)
//                {
//                    if (self.CurrentTile().visible || PlayerAbilitiesSystem.playerEntity.CurrentTile().GetSqrDistance(currentTile) <= 64)
//                    {

//                        aIComponent.targetSearchCooldown = 40;

//                        if (aIComponent.target != Entity.Null
//                        && (!aIComponent.target.HasComponent<AIComponent>()
//                        || aIComponent.target.CurrentTile().GetSqrDistance(currentTile) > Mathf.Pow(creatureComponent.viewDistance + 2, 2)
//                        || (aIComponent.target.IsInInvis() && BaseMethodsClass.Chance(100))
//                        || (!currentTile.ClearLineOfSight(aIComponent.target.CurrentTile()) && BaseMethodsClass.Chance(25))))
//                        {
//                            aIComponent.target = Entity.Null;
//                        }

//                        if (aIComponent.target == Entity.Null)
//                        {

//                            HashSet<TileData> tiles = BaseMethodsClass.GetTilesInRadius(currentTile, creatureComponent.viewDistance);
//                            foreach (var tile in tiles)
//                            {

//                                if (tile.SolidLayer != Entity.Null && tile.SolidLayer != self)
//                                {

//                                    if (tile.SolidLayer.IsHostile(self) || self.IsHostile(tile.SolidLayer))
//                                    {
//                                        if (!tile.SolidLayer.IsInInvis() || BaseMethodsClass.Chance(50))
//                                        {
//                                            aIComponent.target = tile.SolidLayer;
//                                            break;
//                                        }
//                                    }
//                                }

//                            }
//                        }
//                    }
//                }










//                BaseMethodsClass.StartTest();
//                if (self.HasComponent<DamagedByPlayerTag>() && !creatureComponent.GetHostileTags().Contains(Tag.Player))
//                {
//                    PostUpdateCommands.RemoveComponent<DamagedByPlayerTag>(self);
//                    self.GetBuffer<InterestTagBufferElement>().Add(new InterestTagBufferElement(Tag.Player));
//                }






//                foreach (var behavior in behaviors)
//                {
//                    behavior.Init(self);
//                }
//                var bestScore = 0f;
//                var bestBehaviour = behaviors[0];

//                foreach (var item in behaviors)
//                {
//                    var evaluation = item.Evaluate();
//                    if (evaluation > bestScore)
//                    {
//                        bestScore = evaluation;
//                        bestBehaviour = item;
//                    }

//                }


//                Debug.Log(self.GetName() + " decides to use " + bestBehaviour.ToString());
//                bestBehaviour.Execute();




//            }  
//        });
//    }
//}
