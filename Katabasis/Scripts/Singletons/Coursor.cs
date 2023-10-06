using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class Coursor : Singleton<Coursor>
{
    public ImagesStack imagesStack;
    private RectTransform rt;
    private void Awake()
    {
        rt = GetComponent<RectTransform>();
    }
    private void Update()
    {
        rt.position = Mouse.current.position.ReadValue();
    }

}
