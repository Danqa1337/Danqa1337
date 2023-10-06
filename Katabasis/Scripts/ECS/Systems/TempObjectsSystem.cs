using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public enum TempObjectType
{
    Dust,
    CuttingAttack,
    PiercingAttack,
    Explosion,
    InvisParticels,

}


public static class TempObjectSystem
{
    public static HashSet<SpawnTempObjectComponent> tempObjectList = new HashSet<SpawnTempObjectComponent>();

    public static void Update()
    {
        
        
        foreach (var tempComponent in tempObjectList)
        {

            GameObject tempGameObject = null;
            Vector3 scale = new Vector3(1,1,1);
            Quaternion rotation = quaternion.identity;

            switch (tempComponent.tempObjectType)
            {
                case TempObjectType.Dust:
                    if (tempComponent.position.ToFloat2().ToTileData().LiquidLayer != Entity.Null)
                    {

                    }
                    else
                    {
                        tempGameObject = Pooler.Take("Dust");
                    }


                    break;
                case TempObjectType.CuttingAttack: 
                    tempGameObject = Pooler.Take("CuttingAttack");
                    calculateParametersForAtackAnimation();
                    break;
                case TempObjectType.PiercingAttack:
                    tempGameObject = Pooler.Take("PiercingAttack");
                    calculateParametersForAtackAnimation();
                    break;
                case TempObjectType.Explosion:
                    tempGameObject = Pooler.Take("Explosion");
                    
                    break;
                case TempObjectType.InvisParticels:
                    tempGameObject = Pooler.Take("InvisParticles");
                    
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (tempGameObject != null)
            {
                tempGameObject.transform.position = tempComponent.position;
                tempGameObject.transform.localScale = scale;
                tempGameObject.transform.rotation = rotation;
            }

            void calculateParametersForAtackAnimation()
            {
                rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, tempComponent.direction) + UnityEngine.Random.Range(-20, 20));
                float r = UnityEngine.Random.Range(0.9f, 1.1f);
                float w = Mathf.Clamp(2 / 2, 0.5f, 2);
                scale = new Vector3(r * w, r * w * BaseMethodsClass.GetRandomMiroring(false).x, r); ;
            }

        }
        tempObjectList.Clear();
    }
    public static void SpawnTempObject(TempObjectType type, Vector3 position)
    {
        tempObjectList.Add(new SpawnTempObjectComponent(type, position));
    }
    public static void SpawnTempObject(TempObjectType type, TileData tile)
    {
        SpawnTempObject(type, tile.position.ToRealPosition());
    }

   
}

public struct SpawnTempObjectComponent
{
    public Vector3 position;
    public float2 direction;
    public TempObjectType tempObjectType;


    public SpawnTempObjectComponent(TempObjectType type, Vector3 position)
    {
        this.position = position;
        this.tempObjectType = type;
        this.direction = float2.zero;
    }
}


