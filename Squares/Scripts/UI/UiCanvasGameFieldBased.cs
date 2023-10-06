using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(GameField))]
public class UiCanvasGameFieldBased : UiCanvas
{
    private GameField _gameField;
    protected override void Awake()
    {
        _gameField = GetComponent<GameField>();
    }
    public override void Show()
    {
        MainCameraHandler.instance.transform.SetParent(transform);
        MainCameraHandler.instance.transform.localPosition = Vector3.zero;
        _gameField.Activate();
    }
    public override void Hide()
    {
        _gameField.Deactivate();
    }
}
