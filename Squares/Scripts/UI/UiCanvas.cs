using UnityEditor;
using UnityEngine;


public class UiCanvas : MonoBehaviour
{
    private Canvas _canvas;
    public UiManager.UIName uIName;
    protected virtual void Awake()
    {
        _canvas = GetComponent<Canvas>();
    }
    public virtual void Show()
    {
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
    }
    public virtual void Hide()
    {
        _canvas.renderMode = RenderMode.WorldSpace;
    }
}
