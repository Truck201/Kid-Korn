using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Unity.VisualScripting;

public class TutorialControlsUI : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject keyboardUI_P1;
    public GameObject joystickUI_P1;
    public GameObject keyboardUI_P2;
    public GameObject joystickUI_P2;

    public TextMeshProUGUI skipText;

    [Header("Textos de acción por jugador")]
    public TextMeshProUGUI actionTextP1;
    public TextMeshProUGUI actionTextP2;


    private void Start()
    {
        UpdateControlHints();
    }

    public void UpdateControlHints()
    {
        var globalInput = GlobalInputManager.Instance;
        var inputActions = globalInput.InputActions;
        inputActions.Enable();

        var skipP1 = inputActions.Gameplay.SkipP1;
        var skipP2 = inputActions.Gameplay.SkipP2;

        var selectP1 = inputActions.Gameplay.SelectP1;
        var selectP2 = inputActions.Gameplay.SelectP2;

        bool p1UsesGamepad = globalInput.GetGamepadForPlayer(1) != null;
        bool p2UsesGamepad = globalInput.GetGamepadForPlayer(2) != null;

        InputDevice deviceP1 = p1UsesGamepad ? globalInput.GetGamepadForPlayer(1) : Keyboard.current;
        InputDevice deviceP2 = p2UsesGamepad ? globalInput.GetGamepadForPlayer(2) : Keyboard.current;

        // Mostrar texto de skip combinando ambos jugadores
        string skipTextP1 = InputUtils.GetBindingDisplay(skipP1, 1, deviceP1);
        string skipTextP2 = InputUtils.GetBindingDisplay(skipP2, 2, deviceP2);
        skipText.text = $"Presioná {skipTextP1} o {skipTextP2} para continuar";

        // Mostrar acción individual para cada jugador
        string actionBindP1 = InputUtils.GetBindingDisplay(selectP1, 1, deviceP1);
        string actionBindP2 = InputUtils.GetBindingDisplay(selectP2, 2, deviceP2);

        if (actionTextP1 != null) actionTextP1.text = $"Recolecta: ({actionBindP1})";
        if (actionTextP2 != null) actionTextP2.text = $"Recolecta: ({actionBindP2})";
    }


    public void ShowOnlyPlayer1()
    {
        var globalInput = GlobalInputManager.Instance;
        bool p1UsesGamepad = globalInput.GetGamepadForPlayer(1) != null;

        keyboardUI_P1?.SetActive(!p1UsesGamepad);
        joystickUI_P1?.SetActive(p1UsesGamepad);

        keyboardUI_P2?.SetActive(false);
        joystickUI_P2?.SetActive(false);

        if (skipText != null)
        {
            string inputText = p1UsesGamepad ? "Y" : "Q";
            skipText.text = $"({inputText}) para continuar";
            skipText.gameObject.SetActive(true);

            skipText.transform.position = new Vector2(-196f, 4f);
        }
    }

    public void ShowOnlyPlayer2()
    {
        var globalInput = GlobalInputManager.Instance;
        bool p2UsesGamepad = globalInput.GetGamepadForPlayer(2) != null;

        keyboardUI_P1?.SetActive(false);
        joystickUI_P1?.SetActive(false);

        keyboardUI_P2?.SetActive(!p2UsesGamepad);
        joystickUI_P2?.SetActive(p2UsesGamepad);

        if (skipText != null)
        {
            string inputText = p2UsesGamepad ? "Y" : "O";
            skipText.text = $"({inputText}) para continuar";
            skipText.gameObject.SetActive(true);

            skipText.transform.position = new Vector2(196f, 4f);
        }
    }

    public void ShowDisable()
    {
        var globalInput = GlobalInputManager.Instance;
        bool p1UsesGamepad = globalInput.GetGamepadForPlayer(1) != null;
        bool p2UsesGamepad = globalInput.GetGamepadForPlayer(2) != null;
        keyboardUI_P1?.SetActive(false);
        joystickUI_P1?.SetActive(false);

        keyboardUI_P2?.SetActive(false);
        joystickUI_P2?.SetActive(false);

        skipText.gameObject.SetActive(false);
    }

}
