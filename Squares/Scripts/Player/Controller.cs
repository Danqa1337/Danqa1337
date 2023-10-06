using TarodevController;
using Unity.Mathematics;
using UnityEngine;

public enum ControllerAction
{
    NULL,
    Move,
    Jump,
}

public enum ActionMap
{
    Default,
    Menuing,
}

public class Controller : Singleton<Controller>
{
    [SerializeField] private Joystick _movementJoystic;

    private bool _areControllsEnabeld;
    private FrameInput _currentFrameInput;

    private bool _jumpDownTuch;
    private bool _jumpUpTuch;

    public static bool IsRemoteControlled
    {
        get
        {
#if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isRemoteConnected)
            {
                return true;
            }
#endif
            return false;
        }
    }

    public static bool AreControllsEnabeld => instance._areControllsEnabeld;
    public static FrameInput CurrentFrameInput => instance._currentFrameInput;

    private void OnEnable()
    {
        Player.OnPlayerSpawned += EnabeControlls;
        Player.OnPlayerDied += DisableControlls;
    }

    private void OnDisable()
    {
        Player.OnPlayerSpawned -= EnabeControlls;
        Player.OnPlayerDied -= DisableControlls;
    }

    private void Update()
    {
        if (AreControllsEnabeld)
        {
            _currentFrameInput.JumpDown = _jumpDownTuch || Input.GetButtonDown("Jump") || math.sign(_movementJoystic.Vertical) > 0.5;
            _currentFrameInput.JumpUp = _jumpUpTuch || Input.GetButtonUp("Jump");

            _currentFrameInput.X = _movementJoystic.Horizontal != 0 ? math.sign(_movementJoystic.Horizontal) : Input.GetAxisRaw("Horizontal");
        }
        _jumpDownTuch = false;
        _jumpUpTuch = false;
    }

    public static void EnabeControlls()
    {
        instance._areControllsEnabeld = true;
    }

    public static void DisableControlls()
    {
        instance._areControllsEnabeld = false;
    }

    public static void ApplyActionMapControlls(ActionMap actionMap)
    {
        if (instance != null)
        {
            if (actionMap == ActionMap.Default)
            {
                EnabeControlls();
            }
            else
            {
                DisableControlls();
            }
        }
    }

    public void JumpDown()
    {
        _jumpDownTuch = true;
    }

    public void JumpUp()
    {
        _jumpUpTuch = true;
    }
}