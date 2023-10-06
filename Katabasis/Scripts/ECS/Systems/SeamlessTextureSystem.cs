using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
[DisableAutoCreation]
public class SeamlessTextureSystem : ComponentSystem
{
    protected override void OnUpdate()

    {
        EntityQuery query = GetEntityQuery
        (
ComponentType.ReadOnly<SeamlessTextureTag>(),
                ComponentType.ReadOnly<RendererComponent>(),
                ComponentType.ReadOnly<CurrentTileComponent>()
            
        );
        Entities.With(query).ForEach((Entity entity, RendererComponent renderer) =>
        {
            var spriteNum = SeamlessTextureAligner.GetSpriteNum(entity);
            if (spriteNum > renderer.sprites.Count -1)
            {
                Debug.Log(spriteNum + " " + renderer.sprites.Count);
            }
            renderer.sprite = renderer.sprites[spriteNum];

        });
    }

}

