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
public class SwapSystem : ComponentSystem
{
    protected override void OnCreate()
    {

    }
    public static NativeArray<TileData> current;
    protected override void OnUpdate()
    {
        float startTime = UnityEngine.Time.realtimeSinceStartup;

        Entities.With(GetEntityQuery
        (
            ComponentType.ReadWrite<IsGoingToSwapTag>()
        )).ForEach((Entity entity, ref IsGoingToSwapTag swapTag, ref CurrentTileComponent currentTileComponent) =>
        {
            var transform1 = entity.GetComponentObject<Transform>();

            
            var entity2 = swapTag.EntityToSwapWith;
            var currenttile1 = currentTileComponent;
            var currenttile2 = entity2.GetComponentData<CurrentTileComponent>();

            var tile1 = entity.CurrentTile();
            var tile2 = entity2.CurrentTile();
            var transform2 = entity2.GetComponentObject<Transform>();

            tile1.SolidLayer = entity2;
            tile2.SolidLayer = entity;

            transform1.position = tile2.position.ToRealPosition();
            transform2.position = tile1.position.ToRealPosition();

            currenttile1.currentTileId = tile2.index;
            currenttile2.currentTileId = tile1.index;

            swapTag.swaped = true;

            entity.SetComponentData(currenttile1);
            entity2.SetComponentData(currenttile2);

            tile1.Save();
            tile2.Save();
            PostUpdateCommands.RemoveComponent<IsGoingToSwapTag>(entity);
            Debug.Log(entity.GetName() + " swaps with " + swapTag.EntityToSwapWith);
        });
        //Debug.Log((UnityEngine.Time.realtimeSinceStartup - startTime) * 1000f);

    }


}
