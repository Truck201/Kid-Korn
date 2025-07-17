using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class LevelMapManager : MonoBehaviour
{
    public List<LevelButtonUI> levelButtons;
    public Image playerSelectorIcon;
    private int currentIndex = 0;

    private float inputCooldown = 0.2f;
    private float inputTimer = 0f;

    private void Start()
    {
        UpdateSelectorPosition();
        UpdateHighlights();
    }

    private void Update()
    {
        inputTimer -= Time.deltaTime;

        var gamepad = Gamepad.current;
        if (gamepad == null) return;

        // Sincronizar con el botón actualmente seleccionado en la navegación
        SyncWithNavigationSelection();

        if (inputTimer <= 0f)
        {
            float x = gamepad.leftStick.x.ReadValue();
            bool right = gamepad.dpad.right.wasPressedThisFrame || x > 0.7f;
            bool left = gamepad.dpad.left.wasPressedThisFrame || x < -0.7f;

            if (right)
            {
                TryMove(1);
                inputTimer = inputCooldown;
            }
            else if (left)
            {
                TryMove(-1);
                inputTimer = inputCooldown;
            }
        }

        if (gamepad.buttonSouth.wasPressedThisFrame || gamepad.startButton.wasPressedThisFrame)
        {
            SelectCurrentLevel();
        }
    }

    private void SyncWithNavigationSelection()
    {
        GameObject selectedObj = EventSystem.current.currentSelectedGameObject;
        if (selectedObj == null) return;

        LevelButtonUI hoveredButton = selectedObj.GetComponent<LevelButtonUI>();
        if (hoveredButton == null) return;

        int index = levelButtons.IndexOf(hoveredButton);
        if (index == -1) return;

        if (index != currentIndex)
        {
            levelButtons[currentIndex].RemoveHighlight();
            currentIndex = index;

            if (!hoveredButton.isLocked)
            {
                UpdateSelectorPosition();
            }

            UpdateHighlights();
        }
    }

    private void TryMove(int direction)
    {
        var currentButton = levelButtons[currentIndex].GetComponent<Button>();
        if (currentButton == null) return;

        Navigation nav = currentButton.navigation;
        Selectable nextSelectable = direction > 0 ? nav.selectOnRight : nav.selectOnLeft;
        if (nextSelectable == null) return;

        LevelButtonUI nextLevelButton = nextSelectable.GetComponent<LevelButtonUI>();
        if (nextLevelButton == null) return;

        int nextIndex = levelButtons.IndexOf(nextLevelButton);
        if (nextIndex == -1) return;

        levelButtons[currentIndex].RemoveHighlight();
        currentIndex = nextIndex;

        // Solo mover el ícono si el nuevo nivel NO está bloqueado
        if (!levelButtons[currentIndex].isLocked)
        {
            UpdateSelectorPosition();
        }

        UpdateHighlights();
    }

    private void UpdateSelectorPosition()
    {
        Vector3 basePos = levelButtons[currentIndex].transform.position + Vector3.up * 50f;
        Vector3 jumpPos = basePos + Vector3.up * 15f;

        LeanTween.cancel(playerSelectorIcon.gameObject);

        LeanTween.move(playerSelectorIcon.gameObject, jumpPos, 0.1f).setEaseOutQuad()
            .setOnComplete(() =>
            {
                LeanTween.move(playerSelectorIcon.gameObject, basePos, 0.1f).setEaseInQuad();
            });
    }

    private void UpdateHighlights()
    {
        for (int i = 0; i < levelButtons.Count; i++)
        {
            if (i == currentIndex)
                levelButtons[i].Highlight();
            else
                levelButtons[i].RemoveHighlight();
        }
    }

    private void SelectCurrentLevel()
    {
        var current = levelButtons[currentIndex];
        if (!current.isLocked)
        {
            Debug.Log("Load Level " + current.levelId);
            // SceneManager.LoadScene(current.levelId);
        }
        else
        {
            Debug.Log("Nivel bloqueado");
        }
    }

    public void SetCurrentSelection(LevelButtonUI button)
    {
        int index = levelButtons.IndexOf(button);
        if (index == -1) return;

        levelButtons[currentIndex].RemoveHighlight();
        currentIndex = index;

        // Solo mover el ícono si el nuevo nivel no está bloqueado
        if (!button.isLocked)
        {
            UpdateSelectorPosition();
        }

        UpdateHighlights();
    }
}
