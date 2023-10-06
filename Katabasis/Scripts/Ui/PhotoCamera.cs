using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PhotoCamera : MonoBehaviour
{
    private Camera camera;
    private void Awake()
    {
        camera = GetComponent<Camera>();
    }

    public void MakePhoto(Entity thing)
    {
        var objectTransform = thing.GetComponentObject<RendererComponent>().transform;
        var objectPosition = objectTransform.position;
        

        objectTransform.position = new Vector3(objectPosition.x, objectPosition.y, -100);
        camera.transform.position = objectTransform.position;
        camera.enabled = true;
        camera.Render();
        camera.enabled = false;
        objectTransform.position = objectPosition;
    }
}
