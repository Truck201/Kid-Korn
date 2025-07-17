using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneChanger : MonoBehaviour
{
    public string sceneName;
    public float delay = 0f;
    public bool isButton = false;
    public Animator animator;

    private bool hasChanged = false;
    private Coroutine sceneCoroutine;

    void Start()
    {
        if (!isButton)
        {
            if (delay > 0f)
                sceneCoroutine = StartCoroutine(DelayedSceneLoad());
            else
                LoadScene();
        }
    }

    public void ChangeScene()
    {
        if (hasChanged) return;

        Debug.Log("Pressed");

        if (isButton && animator != null)
        {
            animator.SetTrigger("isPlay");
        }

        if (delay > 0f)
        {
            sceneCoroutine = StartCoroutine(DelayedSceneLoad());
        }
        else
        {
            LoadScene();
        }
    }

    private IEnumerator DelayedSceneLoad()
    {
        float elapsed = 0f;

        while (elapsed < delay)
        {
            // Espera solo si el juego NO está pausado
            if (!PauseManager.isGameLogicPaused)
                elapsed += Time.unscaledDeltaTime;

            yield return null;
        }

        LoadScene();
    }

    private void LoadScene()
    {
        if (hasChanged || string.IsNullOrEmpty(sceneName)) return;

        hasChanged = true;

        if (SceneTransitionController.Instance != null)
            SceneTransitionController.Instance.TransitionToScene(sceneName);
        else
            SceneManager.LoadScene(sceneName);
    }
}
