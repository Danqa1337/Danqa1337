using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coursor : MonoBehaviour
{
    private void OnEnable()
    {
        TouchController.OnDrag += OnTouch;
    }

    private void OnDisable()
    {
        TouchController.OnDrag -= OnTouch;
    }

    private void OnTouch(Vector2 touchPosition)
    {
        transform.position = touchPosition;
    }
}