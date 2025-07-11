using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransitionController : MonoBehaviour
{
    public static SceneTransitionController Instance;

    [Header("Transition Settings")]
    public Animator transitionAnimator;
    public float transitionDuration = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void TransitionToScene(string sceneName)
    {
        StartCoroutine(PerformTransition(sceneName));
    }

    private IEnumerator PerformTransition(string sceneName)
    {
        // Activar animación de cerrar círculo
        transitionAnimator.SetTrigger("Close");

        // Esperar animación + tiempo en negro
        yield return new WaitForSeconds(transitionDuration + 1f);

        // Cargar la nueva escena
        SceneManager.LoadScene(sceneName);

        // Esperar que cargue un frame antes de abrir
        yield return null;

        // Abrir la escena con animación inversa
        // transitionAnimator.SetTrigger("Open");
    }
}
