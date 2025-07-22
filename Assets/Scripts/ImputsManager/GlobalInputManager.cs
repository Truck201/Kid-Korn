using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class GlobalInputManager : MonoBehaviour
{
    public static GlobalInputManager Instance;

    private GameInputActions inputActions;

    public Vector2 moveP1 { get; private set; }
    public Vector2 moveP2 { get; private set; }

    public bool selectP1 { get; private set; }
    public bool selectP2 { get; private set; }

    public bool skipP1 { get; private set; }
    public bool skipP2 { get; private set; }

    public bool pauseP1 { get; private set; }
    public bool pauseP2 { get; private set; }

    public bool moveLeftP1PressedThisFrame { get; private set; }
    public bool moveRightP1PressedThisFrame { get; private set; }
    public bool moveLeftP2PressedThisFrame { get; private set; }
    public bool moveRightP2PressedThisFrame { get; private set; }

    private Gamepad gamepadP1;
    private Gamepad gamepadP2;

    public GameInputActions InputActions => inputActions;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        inputActions = new GameInputActions();
        inputActions.Gameplay.Enable();

        AssignGamepads();

        // Movimiento Player 1
        inputActions.Gameplay.MoveP1.performed += ctx =>
        {
            // Aceptar teclado SIEMPRE, y joystick si está asignado
            if (ctx.control.device is Keyboard || ctx.control.device == gamepadP1)
                moveP1 = ctx.ReadValue<Vector2>();
        };
        inputActions.Gameplay.MoveP1.canceled += ctx =>
        {
            if (ctx.control.device is Keyboard || ctx.control.device == gamepadP1)
                moveP1 = Vector2.zero;
        };

        // Movimiento Player 2
        inputActions.Gameplay.MoveP2.performed += ctx =>
        {
            if (ctx.control.device is Keyboard || ctx.control.device == gamepadP2)
                moveP2 = ctx.ReadValue<Vector2>();
        };
        inputActions.Gameplay.MoveP2.canceled += ctx =>
        {
            if (ctx.control.device is Keyboard || ctx.control.device == gamepadP2)
                moveP2 = Vector2.zero;
        };

        // Selección Player 1
        inputActions.Gameplay.SelectP1.performed += ctx =>
        {
            if (ctx.control.device is Keyboard || ctx.control.device == gamepadP1)
                selectP1 = true;
        };

        // Selección Player 2
        inputActions.Gameplay.SelectP2.performed += ctx =>
        {
            if (ctx.control.device is Keyboard || ctx.control.device == gamepadP2)
                selectP2 = true;
        };


        inputActions.Gameplay.SkipP1.performed += ctx =>
        {
            if (ctx.control.device is Keyboard || ctx.control.device == gamepadP1)
                skipP1 = true;
        };
        inputActions.Gameplay.SkipP2.performed += ctx =>
        {
            if (ctx.control.device is Keyboard || ctx.control.device == gamepadP2)
                skipP2 = true;
        };

        inputActions.Gameplay.PauseP1.performed += ctx =>
        {
            if (ctx.control.device is Keyboard || ctx.control.device == gamepadP1)
                pauseP1 = true;
        };
        inputActions.Gameplay.PauseP2.performed += ctx =>
        {
            if (ctx.control.device is Keyboard || ctx.control.device == gamepadP2)
                pauseP2 = true;
        };
    }

    private void AssignGamepads()
    {
        var gamepads = Gamepad.all;

        if (gamepads.Count >= 1)
        {
            gamepadP1 = gamepads[0];
        }

        if (gamepads.Count >= 2)
        {
            gamepadP2 = gamepads[1];
        }

        Debug.Log($"Gamepads detectados: {gamepads.Count}. P1: {gamepadP1?.displayName}, P2: {gamepadP2?.displayName}");
    }

    private void LateUpdate()
    {
        selectP1 = selectP2 = skipP1 = skipP2 = pauseP1 = pauseP2 = false;
    }

    private void Update()
    {
        // PLAYER 1
        moveLeftP1PressedThisFrame = false;
        moveRightP1PressedThisFrame = false;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.wasPressedThisFrame)
                moveLeftP1PressedThisFrame = true;
            if (Keyboard.current.dKey.wasPressedThisFrame)
                moveRightP1PressedThisFrame = true;
        }

        if (gamepadP1 != null)
        {
            if (gamepadP1.leftStick.left.wasPressedThisFrame)
                moveLeftP1PressedThisFrame = true;
            if (gamepadP1.leftStick.right.wasPressedThisFrame)
                moveRightP1PressedThisFrame = true;
        }

        // PLAYER 2
        moveLeftP2PressedThisFrame = false;
        moveRightP2PressedThisFrame = false;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
                moveLeftP2PressedThisFrame = true;
            if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
                moveRightP2PressedThisFrame = true;
        }

        if (gamepadP2 != null)
        {
            if (gamepadP2.leftStick.left.wasPressedThisFrame)
                moveLeftP2PressedThisFrame = true;
            if (gamepadP2.leftStick.right.wasPressedThisFrame)
                moveRightP2PressedThisFrame = true;
        }
    }


    public void AssignGamepadToPlayer(int player, Gamepad pad)
    {

        if (player == 1)
            gamepadP1 = pad;
        else if (player == 2)
            gamepadP2 = pad;
    }

    public Gamepad GetGamepadForPlayer(int player)
    {
        if (player == 1) return gamepadP1;
        if (player == 2) return gamepadP2;
        return null;
    }
}

