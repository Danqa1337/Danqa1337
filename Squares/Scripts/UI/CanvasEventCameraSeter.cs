using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class CanvasEventCameraSeter : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Canvas>().worldCamera = MainCameraHandler.Camera;
    }
}