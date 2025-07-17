using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseCanvas;
    public static bool isGameLogicPaused = false;

    private bool isPaused = false;

    private void Update()
    {
        var input = GlobalInputManager.Instance;

        if (input == null) return;

        if ((input.pauseP1 || input.pauseP2) && !isPaused)
        {
            PauseGame();
        }
        else if ((input.pauseP1 || input.pauseP2) && isPaused)
        {
            ResumeGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        isGameLogicPaused = true;
        pauseCanvas.SetActive(true);
    }

    public void ResumeGame()
    {
        isPaused = false;
        isGameLogicPaused = false;
        pauseCanvas.SetActive(false);
    }

    public void QuitToMainMenu()
    {
        isGameLogicPaused = false;
        SceneManager.LoadScene("MainMenu");
    }
}
