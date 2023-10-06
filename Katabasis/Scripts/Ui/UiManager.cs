using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiManager : Singleton<UiManager>
{  
    public enum Menu
    {
        Inventory,
        Pause,
        Console,
        Describer,
        Death,
        Log,
    }
    public enum UICanvasState
    {
        On,
        Off,
        Null,
    }
    public Camera mainCam;
    public Canvas inventoryCanvas;
    public Canvas pauseCanvas;
    public Canvas consoleCanvas;
    public Canvas describerCanvas;
    public Canvas deathCanvas;
    public Canvas logCanvas;


    [SerializeField] public Stack<Menu> menuStack;
    private void Awake()
    {
        
        mainCam = Camera.main;
        menuStack = new Stack<Menu>();
    }


    public void ToggleUICanvas(Menu menu, UICanvasState state = UICanvasState.Null)
    {

        Canvas[] canvases = null;
        switch (menu)
        {
            case Menu.Inventory:
                canvases = new Canvas[] { inventoryCanvas};
                break;
            case Menu.Pause:
                canvases = new Canvas[] { pauseCanvas };
                break;
            case Menu.Console:
                canvases = new Canvas[] { consoleCanvas };
                break;
            case Menu.Describer:
                canvases = new Canvas[] { describerCanvas };
                break;
            case Menu.Death:
                canvases = new Canvas[] { deathCanvas };
                break;
            case Menu.Log:
                canvases = new Canvas[] { logCanvas };
                break;

        }

        if (state == UICanvasState.Null)
        {

            if (canvases[0].enabled) CloseLast();
            else ToggleUICanvas(menu, UICanvasState.On);
        }

        switch (state)
        {
            case UICanvasState.On:
                foreach (var camera in canvases)
                {
                    camera.enabled = true;
                    camera.renderMode = RenderMode.ScreenSpaceOverlay;
                   

                }
                menuStack.Push(menu);
                break;
            case UICanvasState.Off:
                foreach (var camera in canvases)
                {
                    camera.enabled = false;

                    camera.renderMode = RenderMode.WorldSpace;
                    camera.GetComponent<RectTransform>().position = new Vector3(0, 0, 0);

                }
                if (menuStack.Count > 0) menuStack.Pop();
                break;
        }

        if (menuStack.Count == 0)
        {
            Controller.controllerState = Controller.ControllerState.movement;
            Debug.Log("menuStack is empty");
        }
        else
        {
            Controller.controllerState = GetStateFromMenu(menuStack.Peek());
        }
    }
    public void CloseAll()
    {
        ToggleUICanvas(Menu.Console, UICanvasState.Off);
        ToggleUICanvas(Menu.Describer, UICanvasState.Off);
        ToggleUICanvas(Menu.Death, UICanvasState.Off);
        ToggleUICanvas(Menu.Inventory, UICanvasState.Off);
        ToggleUICanvas(Menu.Pause, UICanvasState.Off);
    }
    private Controller.ControllerState GetStateFromMenu(Menu menu) => (menu) switch
    {
        Menu.Inventory => Controller.ControllerState.inventory,
        Menu.Pause => Controller.ControllerState.pause,
        Menu.Console => Controller.ControllerState.consoling,
        Menu.Describer => Controller.ControllerState.describer,
        Menu.Death => Controller.ControllerState.dead,
        Menu.Log => Controller.ControllerState.movement,
    };

    public void CloseLast()
    {
        if (menuStack.Count > 0)
        {
            Menu menu = menuStack.Pop();
            ToggleUICanvas(menu, UICanvasState.Off);
            if (menuStack.Count == 0) Controller.controllerState = Controller.ControllerState.movement;
        }
    }
    

}
