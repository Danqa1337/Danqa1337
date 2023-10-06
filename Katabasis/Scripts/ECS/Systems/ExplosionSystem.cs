using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Entities;
public static class ExplosionSystem 
{
    private static HashSet<ExplosionData> explosionDataList = new HashSet<ExplosionData>();
    public static void Update()
    {
        foreach (var explosionData in explosionDataList)
        {
            var power = explosionData.power;
            var epicenter = explosionData.position.ToTileData();


            TempObjectSystem.SpawnTempObject(TempObjectType.Explosion, epicenter);

                
            var objects = new HashSet<Entity>();
            var newObjects = new HashSet<Entity>();

            foreach (var tile in BaseMethodsClass.GetTilesInRadius(epicenter, 1))
            {
                var sqrDst = tile.GetSqrDistance(epicenter);

                if (tile.SolidLayer != Entity.Null) objects.Add(tile.SolidLayer);
                foreach (var item in tile.DropLayer)
                {
                    objects.Add(item);
                }




            }
            foreach (var item in objects)
            {
                var parts = item.GetComponentData<AnatomyComponent>().GetLoverHierarchy();

                foreach (var part in parts)
                {
                    var joint = part.GetComponentData<AnatomyComponent>().GetParentJoint();
                    if (joint != Joint.Null)
                    {
                        if (joint.TryToBreak(power, Entity.Null)) ;
                        {
                            newObjects.Add(part);
                        }

                    }
                }
                newObjects.Add(item);

            }
            foreach (var item in newObjects)
            {
                var damage = (int)BaseMethodsClass.GenerateNormalRandom(power, 0.5f);
                var Change = new DurabilityChangedOnThisTick();
                if (item.HasComponent<DurabilityChangedOnThisTick>())
                {
                    Change = item.GetComponentData<DurabilityChangedOnThisTick>();
                    
                }
                Change.value -= damage;

                item.SetComponentData(Change);



                float2 vector = (item.CurrentTile() - epicenter) + (UnityEngine.Random.insideUnitCircle.ToFloat2() * 0.2f);


                if (item.CurrentTile() == epicenter)
                {
                    vector = UnityEngine.Random.insideUnitCircle.ToFloat2();
                }
                item.AddComponentData(new ImpulseComponent(vector, damage, 10, Entity.Null));

            }

            Debug.Log("Explosion with power of: " + power + " on " + epicenter.position);

            
        }
        explosionDataList.Clear();
    }

    public static void SchaduleExplosion(TileData tile,int power)
    {
        explosionDataList.Add(new ExplosionData(power, tile.position));
    }
    private struct ExplosionData
    {
        public int power;
        public int2 position;

        public ExplosionData(int power, int2 position)
        {
            this.power = power;
            this.position = position;
        }
    }

}

