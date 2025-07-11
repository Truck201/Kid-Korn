using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public string sceneName; // nombre exacto de la escena (debe estar en Build Settings)
    public float delay = 0f; // tiempo antes del cambio

    public bool isButton = false;

    public void ChangeScene()
    {
        if (delay > 0f)
        {
            Invoke(nameof(LoadScene), delay);
        }
        else
        {
            if (!isButton)
            {
                LoadScene();
            }
        }
    }

    private void LoadScene()
    {
        SceneTransitionController.Instance.TransitionToScene(sceneName);
    }

    private void FixedUpdate()
    {
        if (!isButton)
        {
            delay -= Time.deltaTime;
        }
    }

    private void Update()
    {
        if (delay <= 0f && sceneName != null)
        {
            SceneTransitionController.Instance.TransitionToScene(sceneName);
        }
    }
}

