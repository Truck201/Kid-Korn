using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class JoystickAssignmentManager : MonoBehaviour
{
    [Header("UI")]
    public Image player1JoystickIcon;
    public Image player2JoystickIcon;
    public TextMeshProUGUI connectionStatusText;

    [Header("Audio")]
    public AudioSource sfxSource;
    public AudioClip connectClip;

    private void Start()
    {
        InputSystem.onDeviceChange += OnDeviceChange;
        AssignJoysticks(); // Ahora las referencias públicas ya están asignadas
    }

    private void OnEnable()
    {
        InputSystem.onDeviceChange += OnDeviceChange;
        AssignJoysticks();
    }

    private void OnDisable()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (device is Gamepad)
        {
            if (change == InputDeviceChange.Added || change == InputDeviceChange.Disconnected || change == InputDeviceChange.Removed)
            {
                AssignJoysticks();

                if (change == InputDeviceChange.Added && connectClip != null)
                    sfxSource?.PlayOneShot(connectClip);
            }
        }
    }

    private void AssignJoysticks()
    {
        var gamepads = Gamepad.all;

        var globalInput = GlobalInputManager.Instance;

        Debug.Log($"Gamepads detectados: {gamepads.Count}");

        // Reset UI
        if (player1JoystickIcon != null)
            player1JoystickIcon.gameObject.SetActive(false);
        if (player2JoystickIcon != null)
            player2JoystickIcon.gameObject.SetActive(false);

        // Reset global input
        globalInput.AssignGamepadToPlayer(1, null);
        globalInput.AssignGamepadToPlayer(2, null);

        if (gamepads.Count >= 1)
        {
            globalInput.AssignGamepadToPlayer(1, gamepads[0]);
            if (player1JoystickIcon != null) player1JoystickIcon.gameObject.SetActive(true);
            Debug.Log("P1: " + gamepads[0].displayName);
        }
        else
        {
            Debug.Log("P1 sin joystick, usará teclado");
        }

        if (gamepads.Count >= 2)
        {
            globalInput.AssignGamepadToPlayer(2, gamepads[1]);
            if (player2JoystickIcon != null) player2JoystickIcon.gameObject.SetActive(true);
            Debug.Log("P2: " + gamepads[1].displayName);
        }
        else
        {
            Debug.Log("P2 sin joystick, usará teclado");
        }

        if (connectionStatusText != null)
        {
            if (gamepads.Count == 0)
                connectionStatusText.text = "No joysticks conectados";
            else if (gamepads.Count == 1)
                connectionStatusText.text = "1 joystick conectado.";
            else
                connectionStatusText.text = "2 joysticks conectados.";
        }
    }


    public void OnStartGame()
    {
        // Cargar siguiente escena con inputs asignados
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
