using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TouchController : MonoBehaviour
{
    public static event Action<Vector2> OnDrag;

    public static event Action<Vector2> OnTap;

    private float _touchTime;
    [SerializeField] private float _tapWindow = 0.1f;

    private Controlls _controlls;

    private void Awake()
    {
        _controlls = new Controlls();
    }

    private void OnEnable()
    {
        _controlls.Enable();
    }

    private void OnDisable()
    {
        _controlls.Disable();
    }

    private void Start()
    {
        _controlls.Mouse.Click.performed += Click;
        _controlls.Touch.Tap.performed += Tap;
    }

    private void Update()
    {
        if (_controlls.Mouse.Drag.inProgress)
        {
            DragMouse();
        }
        //if (_controlls.Touch.Drag.inProgress)
        //{
        //    DragTouch();
        //}
    }

    private void Click(InputAction.CallbackContext context)
    {
        OnTap?.Invoke(MainCameraController.Camera.ScreenToWorldPoint(Mouse.current.position.ReadValue()));
    }

    private void Tap(InputAction.CallbackContext context)
    {
        OnTap?.Invoke(MainCameraController.Camera.ScreenToWorldPoint(context.ReadValue<Vector2>()));
    }

    private void DragTouch()
    {
        OnDrag?.Invoke(MainCameraController.Camera.ScreenToWorldPoint(_controlls.Touch.Drag.ReadValue<Vector2>()));
    }

    private void DragMouse()
    {
        OnDrag?.Invoke(MainCameraController.Camera.ScreenToWorldPoint(Mouse.current.position.ReadValue()));
    }
}