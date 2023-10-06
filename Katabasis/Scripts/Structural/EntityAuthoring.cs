using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Entities;
using UnityAsync;
using UnityEngine;

public class EntityAuthoring : MonoBehaviour
{
    public Entity_ItemTable.Param param;
    public ObjectData composition;
    public Transform partsHolder;
    public RendererComponent bodyRenderer;
    public TrailRenderer trail;
    public Action OnDeath;



    public async void Desolve()
    {
        OnDeath?.Invoke();
        for (float f = 0; f < 1f; f += 0.05f)
        {
            bodyRenderer.spriteRenderer.material.SetFloat("Desolve", f);
            await Task.Delay(1);
        }
        transform.rotation = Quaternion.Euler(0,0,0);
        Pooler.PutObjectBackToPool(gameObject);
        bodyRenderer.spriteRenderer.material.SetFloat("Desolve", 0);
    }
}
