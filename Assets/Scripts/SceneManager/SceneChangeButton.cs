using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class SceneChangeButton : MonoBehaviour
{
    [Header("Asignaciones")]
    public Button startButton;
    public CanvasScaler canvasScaler;
    public Image lightOverlayImage;
    public string sceneToLoad = "GameScene";
    public Volume volume; // <-- Volume post-processing con Vignette

    [Header("Transición")]
    public float zoomDuration = 1f;
    public float zoomAmount = 1.5f;
    public bool zoomIn = true;

    private Vector2 originalResolution;
    private bool isTransitioning = false;

    private Vignette vignette;
    private Vector2 originalVignetteCenter;
    private float originalVignetteIntensity = 0f;

    private FilmGrain filmGrain;
    private float originalFilmGrainIntensity;

    void Start()
    {
        if (startButton != null)
            startButton.onClick.AddListener(OnStartButtonPressed);

        if (canvasScaler != null)
            originalResolution = canvasScaler.referenceResolution;

        if (lightOverlayImage != null)
            SetImageAlpha(lightOverlayImage, 0f);

        if (volume != null && volume.profile.TryGet(out vignette))
        {
            originalVignetteIntensity = vignette.intensity.value;
            originalVignetteCenter = vignette.center.value;
        }

        if (volume != null && volume.profile.TryGet(out vignette))
        {
            originalVignetteIntensity = vignette.intensity.value;
        }

        if (volume.profile.TryGet(out filmGrain))
        {
            originalFilmGrainIntensity = filmGrain.intensity.value;
        }
    }

    void OnStartButtonPressed()
    {
        if (!isTransitioning)
            StartCoroutine(ZoomAndFadeOut());
    }

    IEnumerator ZoomAndFadeOut()
    {
        isTransitioning = true;

        float t = 0f;
        Vector2 startRes = originalResolution;
        Vector2 targetRes = zoomIn ? startRes / zoomAmount : startRes * zoomAmount;

        float startAlpha = 0f;
        float targetAlpha = 1f;

        float startVignette = vignette != null ? vignette.intensity.value : 0f;
        float targetVignette = 0.9f;

        Vector2 startCenter = vignette != null ? vignette.center.value : new Vector2(0.5f, 0.5f);
        Vector2 targetCenter = new Vector2(0.5f, 0.5f); // fuera del centro para simular apagado

        while (t < zoomDuration)
        {
            t += Time.deltaTime;
            float progress = t / zoomDuration;

            if (canvasScaler != null)
                canvasScaler.referenceResolution = Vector2.Lerp(startRes, targetRes, progress);

            if (lightOverlayImage != null)
                SetImageAlpha(lightOverlayImage, Mathf.Lerp(startAlpha, targetAlpha, progress));

            if (vignette != null)
                vignette.intensity.value = Mathf.Lerp(startVignette, targetVignette, progress);

            if (vignette != null)
            {
                vignette.intensity.value = Mathf.Lerp(startVignette, targetVignette, progress);
                vignette.center.value = Vector2.Lerp(startCenter, targetCenter, progress);
            }

            if (filmGrain != null)
            {
                filmGrain.intensity.value = Mathf.Lerp(originalFilmGrainIntensity, 1f, progress); // va de 0 a 1
            }

            yield return null;
        }

        // Aseguramos los valores finales
        if (canvasScaler != null)
            canvasScaler.referenceResolution = targetRes;

        if (lightOverlayImage != null)
            SetImageAlpha(lightOverlayImage, targetAlpha);

        if (vignette != null)
            vignette.intensity.value = targetVignette;

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(sceneToLoad);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (canvasScaler != null)
            canvasScaler.referenceResolution = originalResolution;

        if (lightOverlayImage != null)
            StartCoroutine(FadeInLights());

        if (vignette != null)
            vignette.intensity.value = originalVignetteIntensity;

        if (vignette != null)
        {
            vignette.intensity.value = originalVignetteIntensity;
            vignette.center.value = originalVignetteCenter;
        }

        if (filmGrain != null)
        {
            filmGrain.intensity.value = originalFilmGrainIntensity;
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    IEnumerator FadeInLights()
    {
        float t = 0f;
        float duration = 0.8f;

        float startAlpha = 1f;
        float targetAlpha = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float progress = t / duration;

            SetImageAlpha(lightOverlayImage, Mathf.Lerp(startAlpha, targetAlpha, progress));
            yield return null;
        }

        SetImageAlpha(lightOverlayImage, 0f);
    }

    void SetImageAlpha(Image img, float alpha)
    {
        Color c = img.color;
        c.a = alpha;
        img.color = c;
    }
}
