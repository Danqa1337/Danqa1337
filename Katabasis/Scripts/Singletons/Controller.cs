using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System.Threading.Tasks;

public enum ControllerAction
{
    NULL,
    ToggleInventory,
    ToggleDescriber,
    TogglePause,
    ToggleMap,
    ToggleContainer,
    ToggleConsole,
    CloseMenu,

    MoveU,
    MoveUR,
    MoveR,
    MoveDR,
    MoveD,
    MoveDL,
    MoveL,
    MoveUL,

    Rest,
    Travel,
    WaitForOneTurn,
    WaitForOneTick,

    Eat,
    Grab,
    Throw,
    Jump,
    Explode,
    ActionWithMainhand,
    Kick,
    MakeFullBodySlam,
    GenerateMap,
    AnyKeyPressed,
    EnterStaircase,
    Zoom,
    SwapHands,
    

}
public class Controller : Singleton<Controller>
{



    public enum ControllerState
    {
        movement,
        pause,
        consoling,
        dead,
        describer,
        inventory,
        map,
    }

    public bool selectionEnabled
    {
        get
        {
            if (controllerState == ControllerState.consoling || controllerState == ControllerState.movement) return true;
            return false;
        }
    }




    public static ControllerState controllerState = ControllerState.dead;
    public PlayerInput playerInput;
    private bool buttonHeld = false;
    public void Listen(InputAction.CallbackContext context)
    {

        if (context.performed)
        {
            buttonHeld = true;
            ControllerAction action = DecodeCharSeparatedEnums<ControllerAction>(context.action.name)[0];
            if (IsCurrentControllerStateAllowAction(action))
            {
                MakeAction(action, context);
                //Debug.Log("!");
            }
        }
        if(context.canceled)
        {
            buttonHeld = false;

        }
    }


    private bool IsCurrentControllerStateAllowAction(ControllerAction action) => (action) switch

    {
        
        ControllerAction.NULL => false,
        ControllerAction.AnyKeyPressed => true, 
        ControllerAction.CloseMenu 
        => controllerState == ControllerState.inventory
        || controllerState == ControllerState.describer
        || controllerState == ControllerState.map,

        ControllerAction.ToggleInventory => controllerState == ControllerState.movement||controllerState==ControllerState.inventory,
        ControllerAction.ToggleDescriber => controllerState == ControllerState.movement || controllerState == ControllerState.describer,
        ControllerAction.TogglePause => controllerState == ControllerState.movement || controllerState == ControllerState.pause,
        ControllerAction.ToggleMap => controllerState == ControllerState.movement || controllerState == ControllerState.map,
        ControllerAction.ToggleConsole => controllerState == ControllerState.movement || controllerState == ControllerState.consoling,
        ControllerAction.MoveU => controllerState == ControllerState.movement,
        ControllerAction.MoveUR => controllerState == ControllerState.movement,
        ControllerAction.MoveR => controllerState == ControllerState.movement,
        ControllerAction.MoveDR => controllerState == ControllerState.movement,
        ControllerAction.MoveD => controllerState == ControllerState.movement,
        ControllerAction.MoveDL => controllerState == ControllerState.movement,
        ControllerAction.MoveL => controllerState == ControllerState.movement,
        ControllerAction.MoveUL => controllerState == ControllerState.movement,
        ControllerAction.Rest => controllerState == ControllerState.movement,
        ControllerAction.Travel => controllerState == ControllerState.movement,
        ControllerAction.WaitForOneTurn => controllerState == ControllerState.movement,
        ControllerAction.WaitForOneTick => controllerState == ControllerState.movement,
        ControllerAction.Eat => controllerState == ControllerState.movement,
        ControllerAction.Grab => controllerState == ControllerState.movement,
        ControllerAction.Jump => controllerState == ControllerState.movement,
        ControllerAction.Explode => controllerState == ControllerState.movement,
        ControllerAction.ActionWithMainhand => controllerState == ControllerState.movement,
        ControllerAction.Kick => controllerState == ControllerState.movement,
        ControllerAction.MakeFullBodySlam => controllerState == ControllerState.movement,
        ControllerAction.GenerateMap => controllerState == ControllerState.movement,
        ControllerAction.Throw => controllerState == ControllerState.movement,
        ControllerAction.EnterStaircase => controllerState == ControllerState.movement,
        ControllerAction.Zoom => controllerState == ControllerState.movement,
        ControllerAction.SwapHands => controllerState == ControllerState.movement,
    };



 
  
    //private void WaitForInput(ControllerAction action)
    //{
    //    string actionName = action.ToString();
    //    if(playerInput.actions.FindAction(actionName).triggered)
    //    {
    //        MakeAction(action);
    //    }
        
        
        
    //}
    
    private async void MakeAction(ControllerAction action, InputAction.CallbackContext context)
    {
        if (PlayerAbilitiesSystem.controlled)
        {
            if (context.performed)
            {
                switch (action)
                {
                    case ControllerAction.NULL:
                        break;
                    case ControllerAction.ToggleInventory:
                        UiManager.i.ToggleUICanvas(UiManager.Menu.Inventory);
                        PlayersInventoryInterface.i.UpdatePortrait();
                        AnatomyScreen.i.UpdateBars();

                        break;
                    case ControllerAction.ToggleDescriber:
                        if (Selector.selectedTile.visible)
                        {
                            UiManager.i.ToggleUICanvas(UiManager.Menu.Describer);
                            DeepDescriber.i.DescribeTile(Selector.selectedTile);
                        }
                        break;
                    case ControllerAction.TogglePause:
                        UiManager.i.ToggleUICanvas(UiManager.Menu.Pause);


                        break;
                    case ControllerAction.ToggleMap:
                        break;
                    case ControllerAction.ToggleContainer:
                        break;
                    case ControllerAction.ToggleConsole:
                        UiManager.i.ToggleUICanvas(UiManager.Menu.Console);

                        if (UiManager.i.consoleCanvas.enabled)
                        {
                            CheatConsole.i.consoleInputField.gameObject.SetActive(true);
                            CheatConsole.i.consoleInputField.ActivateInputField();

                        }
                        else
                        {
                            CheatConsole.i.consoleInputField.gameObject.SetActive(false);
                            CheatConsole.i.consoleInputField.DeactivateInputField();

                        }
                        break;
                    case ControllerAction.CloseMenu:
                        UiManager.i.CloseLast();
                        break;
                    case ControllerAction.MoveU:
                        while (buttonHeld)
                        {
                            await Task.Delay(10);
                            if (PlayerAbilitiesSystem.controlled)
                            {
                                PlayerAbilitiesSystem.i.UseMakeStep(PlayerAbilitiesSystem.playerEntity.CurrentTile() + Direction.U);
                            }
                        }
                        break;
                    case ControllerAction.MoveUR:
                        while (buttonHeld)
                        {
                            await Task.Delay(100);
                            if (PlayerAbilitiesSystem.controlled)
                            {
                                PlayerAbilitiesSystem.i.UseMakeStep(PlayerAbilitiesSystem.playerEntity.CurrentTile() + Direction.UR);
                            }
                        }
                        break;
                    case ControllerAction.MoveR:
                        while (buttonHeld)
                        {
                            await Task.Delay(10);
                            if (PlayerAbilitiesSystem.controlled)
                            {
                                PlayerAbilitiesSystem.i.UseMakeStep(PlayerAbilitiesSystem.playerEntity.CurrentTile() + Direction.R);
                            }
                        }
                        break;
                    case ControllerAction.MoveDR:
                        while (buttonHeld)
                        {
                            await Task.Delay(10);
                            if (PlayerAbilitiesSystem.controlled)
                            {
                                PlayerAbilitiesSystem.i.UseMakeStep(PlayerAbilitiesSystem.playerEntity.CurrentTile() + Direction.DR);
                            }
                        }
                        break;
                    case ControllerAction.MoveD:
                        while (buttonHeld)
                        {
                            await Task.Delay(10);
                            if (PlayerAbilitiesSystem.controlled)
                            {
                                PlayerAbilitiesSystem.i.UseMakeStep(PlayerAbilitiesSystem.playerEntity.CurrentTile() + Direction.D);
                            }
                        }
                        break;
                    case ControllerAction.MoveDL:
                        while (buttonHeld)
                        {
                            await Task.Delay(10);
                            if (PlayerAbilitiesSystem.controlled)
                            {
                                PlayerAbilitiesSystem.i.UseMakeStep(PlayerAbilitiesSystem.playerEntity.CurrentTile() + Direction.DL);
                            }
                        }
                        break;
                    case ControllerAction.MoveL:
                        while (buttonHeld)
                        {
                            await Task.Delay(10);
                            if (PlayerAbilitiesSystem.controlled)
                            {
                                PlayerAbilitiesSystem.i.UseMakeStep(PlayerAbilitiesSystem.playerEntity.CurrentTile() + Direction.L);
                            }
                        }
                        break;
                    case ControllerAction.MoveUL:
                        while (buttonHeld)
                        {
                            await Task.Delay(10);
                            if (PlayerAbilitiesSystem.controlled)
                            {
                                PlayerAbilitiesSystem.i.UseMakeStep(PlayerAbilitiesSystem.playerEntity.CurrentTile() + Direction.UL);
                            }
                        }
                        break;
                    case ControllerAction.Rest:

                        break;
                    case ControllerAction.Travel:
                        PlayerAbilitiesSystem.i.MoveToSelected();
                        break;
                    case ControllerAction.WaitForOneTurn:
                        // PlayerControllerSystem.SpendTime(10);
                        break;
                    case ControllerAction.WaitForOneTick:
                        PlayerAbilitiesSystem.i.SpendTime(1);
                        break;
                    case ControllerAction.Eat:
                        PlayerAbilitiesSystem.i.UseEat();
                        break;
                    case ControllerAction.Grab:
                        PlayerAbilitiesSystem.i.UsePicUp();
                        break;
                    case ControllerAction.Throw:
                        PlayerAbilitiesSystem.i.UseThrow();
                        break;
                    case ControllerAction.Jump:
                        PlayerAbilitiesSystem.i.UseJump();
                        break;
                    case ControllerAction.Explode:
                        if (Selector.selectedTile != TileData.Null)
                        {
                            ExplosionSystem.SchaduleExplosion(Selector.selectedTile, 100);
                            PlayerAbilitiesSystem.i.SpendTime(10);
                        }


                        break;
                    case ControllerAction.ActionWithMainhand:
                        PlayerAbilitiesSystem.i.UseAtack();
                        break;
                    case ControllerAction.Kick:
                        PlayerAbilitiesSystem.i.UseKickSelected();
                        break;
                    case ControllerAction.MakeFullBodySlam:
                        //  Player.i.MakeFullBodySlam();
                        break;
                    case ControllerAction.GenerateMap:
                        Engine.i.Restart();
                        break;
                    case ControllerAction.EnterStaircase:
                        PlayerAbilitiesSystem.i.EnterStaircase();
                        break;
                    case ControllerAction.Zoom:
                        {
                            if (context.ReadValue<float>() > 0) MainCameraHandler.i.ZoomIn();
                            else MainCameraHandler.i.ZoomOut();
                        }
                        break;
                    case ControllerAction.SwapHands:
                        {
                            PlayerAbilitiesSystem.SwapHands();
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        if (action == ControllerAction.AnyKeyPressed)
        {
            PlayerAbilitiesSystem.pathMovementStoped = true;
            
        }
    }




    //private void MainActions()
    //{

    //    if (Player.i.controlled) WaitForInput(ControllerAction.Rest);
    //    if (Player.i.controlled) WaitForInput(ControllerAction.Travel);
    //    if (Player.i.controlled) WaitForInput(ControllerAction.WaitForOneTick);
    //    if (Player.i.controlled) WaitForInput(ControllerAction.WaitForOneTurn);
    //    if (Player.i.controlled) WaitForInput(ControllerAction.Eat);
    //    if (Player.i.controlled) WaitForInput(ControllerAction.Grab);
    //    if (Player.i.controlled) WaitForInput(ControllerAction.Jump);
    //    if (Player.i.controlled) WaitForInput(ControllerAction.Explode);
    //    if (Player.i.controlled) WaitForInput(ControllerAction.ActionWithMainhand);
    //    if (Player.i.controlled) WaitForInput(ControllerAction.Kick);
    //    if (Player.i.controlled) WaitForInput(ControllerAction.MakeFullBodySlam);
    //    if (Player.i.controlled) WaitForInput(ControllerAction.GenerateMap);

    //    if (Player.i.controlled) WaitForInput(ControllerAction.MoveU);
    //    if (Player.i.controlled) WaitForInput(ControllerAction.MoveUR);
    //    if (Player.i.controlled) WaitForInput(ControllerAction.MoveR);
    //    if (Player.i.controlled) WaitForInput(ControllerAction.MoveDR);
    //    if (Player.i.controlled) WaitForInput(ControllerAction.MoveD);
    //    if (Player.i.controlled) WaitForInput(ControllerAction.MoveDL);
    //    if (Player.i.controlled) WaitForInput(ControllerAction.MoveL);
    //    if (Player.i.controlled) WaitForInput(ControllerAction.MoveUL);

    //    if (Player.i.controlled) WaitForInput(ControllerAction.Travel);
        
    //}

    public class InputActionData
    {
        public InputAction action;
        public List<ControllerState> alowedStates;
        public InputActionData(InputAction _action)
        {
            action = _action;
            alowedStates = new List<ControllerState>();
        }
    }
}


