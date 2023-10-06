using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraHandler : Singleton<MainCameraHandler>
{
    public Camera cam
    {
        get
        {
            if (_cam == null) _cam = Camera.main;
            return _cam;
        }
    }
    private Camera _cam;
    
    public void ZoomIn()
    {
        if(cam.orthographicSize > 1)
        {
            cam.orthographicSize--;
        }
    } 
    public void ZoomOut()
    {
        if(cam.orthographicSize < 24)
        {
            cam.orthographicSize++;
        }
    }

    
}
