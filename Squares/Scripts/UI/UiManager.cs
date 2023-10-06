using System;
using System.Collections.Generic;
using UnityEngine;



public class UiManager : Singleton<UiManager>
{
    public static UIName CurrentUi => instance._currentUiName;
    private UIName _currentUiName;
    private UiCanvas[] _allUiCanvases;
    private List<UIName> _openCanvases = new List<UIName>();

    public enum UIName
    {
        Null,
        Inventory,
        Pause,
        Console,
        Describer,
        Death,
        Log,
        CharacterSellection,
        Map,
        Settings,
        Controlls,
        RadialMenu,
        Aiming,
        Scoreboard,
        ModeSelection,
        MainMenu,
        Login,
        UserData,
        Registration,
        Hud,
    }

    private void Awake()
    {
        _allUiCanvases = FindObjectsOfType<UiCanvas>();
        _openCanvases = new List<UIName>();
    }

    public static void SwichToUi(UIName uiName)
    {
        instance.CloseAll();
        UiCanvas uiCanvas = GetUiCanvasByName(uiName);

        uiCanvas.Show();
        Debug.Log(uiName + " opened");
        Controller.ApplyActionMapControlls(GetActionMapFromUiCanvasName(uiName));
        Push(uiName);
        instance._currentUiName = uiName;
    }

    private static UIName PeekLast()
    {
        var uiName = instance._openCanvases[instance._openCanvases.Count - 1];
        return uiName;
    }

    private static void Push(UIName uIName)
    {
        if (instance._openCanvases.Contains(uIName))
        {
            instance._openCanvases.Remove(uIName);
        }
        instance._openCanvases.Add(uIName);
    }

    public static void CloseUiCanvas(UIName uIName)
    {
        if (instance._openCanvases.Contains(uIName))
        {
            instance._openCanvases.Remove(uIName);
            var uiCanvas = GetUiCanvasByName(uIName);
            uiCanvas.Hide();
            Debug.Log(uIName + " closed");
            if (instance._openCanvases.Count == 0)
            {
                Controller.ApplyActionMapControlls(ActionMap.Default);
            }
            else
            {
                Controller.ApplyActionMapControlls(GetActionMapFromUiCanvasName(PeekLast()));
            }
            instance._currentUiName = UIName.Null;
            GetActionOnClose(uIName).Invoke();
        }
    }

    private static UiCanvas GetUiCanvasByName(UIName uiName)
    {
        UiCanvas uiCanvas = null;

        foreach (var item in instance._allUiCanvases)
        {
            if (item.uIName == uiName)
            {
                uiCanvas = item;
                break;
            }
        }

        if (uiCanvas == null) throw new System.Exception("UiCanvas " + uiName + " not found");
        return uiCanvas;
    }

    private void CloseAll()
    {
        for (int i = 0; i < _openCanvases.Count; i++)
        {
            CloseUiCanvas(_openCanvases[i]);
        }
    }

    private static ActionMap GetActionMapFromUiCanvasName(UIName uiCanvasName) => (uiCanvasName) switch
    {
        UIName.Hud => ActionMap.Default,
        _ => ActionMap.Menuing,
    };

    public static Action GetActionOnClose(UIName uiCanvasName) => (uiCanvasName) switch
    {
        _ => delegate { }
    };
}