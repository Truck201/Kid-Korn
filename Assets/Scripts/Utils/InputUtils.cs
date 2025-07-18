using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;

public static class InputUtils
{
    public static string GetBindingDisplay(InputAction action, int playerIndex, InputDevice device)
    {
        if (action == null || device == null) return "";

        foreach (var binding in action.bindings)
        {
            // Ignorar bindings compuestos
            if (binding.isComposite || binding.isPartOfComposite) continue;

            // GAMEPAD
            if (device is Gamepad)
            {
                // Verificamos que el binding pertenezca a un gamepad
                if (!binding.effectivePath.Contains("Gamepad")) continue;

                // Extraemos el control (ej: <Gamepad>/buttonSouth)
                string controlPath = binding.effectivePath;

                return GetGamepadButtonDisplayName(controlPath, device);
            }

            // KEYBOARD
            if (device is Keyboard)
            {
                if (!binding.effectivePath.Contains("Keyboard")) continue;

                // Devolver tecla legible
                return InputControlPath.ToHumanReadableString(
                    binding.effectivePath,
                    InputControlPath.HumanReadableStringOptions.OmitDevice
                );
            }
        }

        return "";
    }

    private static string GetGamepadButtonDisplayName(string controlPath, InputDevice device)
    {
        if (device is DualShockGamepad || device is DualSenseGamepadHID)
        {
            // PlayStation botones
            if (controlPath.Contains("buttonSouth")) return "✕";
            if (controlPath.Contains("buttonEast")) return "◯";
            if (controlPath.Contains("buttonWest")) return "◯"; // o "□"
            if (controlPath.Contains("buttonNorth")) return "△";
        }
        else
        {
            // Xbox o genérico
            if (controlPath.Contains("buttonSouth")) return "A";
            if (controlPath.Contains("buttonEast")) return "B";
            if (controlPath.Contains("buttonWest")) return "X";
            if (controlPath.Contains("buttonNorth")) return "Y";
        }

        // Por defecto, mostrar el path como fallback
        return InputControlPath.ToHumanReadableString(
            controlPath,
            InputControlPath.HumanReadableStringOptions.OmitDevice
        );
    }
}
