using System.Collections;
using UnityEngine;

public class MainCameraHandler : Singleton<MainCameraHandler>
{
    private Camera _camera;

    public static Camera Camera { get => instance._camera; }
}