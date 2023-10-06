using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class ImagesStack : RenderersStack
{
    public void DrawItem(Entity entity)
    {

        Clear();
        if (entity != Entity.Null)
        {
            
            var partSprites = entity.GetComponentData<AnatomyComponent>().GetLoverHierarchy()
                .Select(p => p.GetComponentObject<RendererComponent>()).ToList();
            
            for (int i = 0; i < partSprites.Count; i++)
            {
                var renderer = renderers[i];

                renderer.sprite = partSprites[i].sprite;
                renderer.color = Color.white;

                //renderer.material.SetColor("TintColor", renderer.material.GetColor("TintColor"));
                



            }
        }
    }

}
