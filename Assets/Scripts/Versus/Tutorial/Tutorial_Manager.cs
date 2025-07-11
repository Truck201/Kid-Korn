using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class Tutorial_Manager : MonoBehaviour
{
    [SerializeField] private GameObject kidKorn;
    [SerializeField] private Transform kidTargetPosition;
    [SerializeField] private Transform kidTargetPosition2;
    [SerializeField] private Transform kidTargetPosition3;
    [SerializeField] private CanvasScaler canvasScaler;
    [SerializeField] private GameObject[] characters;
    [SerializeField] private GameObject tutorialText1;
    [SerializeField] private GameObject tutorialText2;
    [SerializeField] private GameObject mainBar;
    [SerializeField] private GameObject[] anillas;
    [SerializeField] private GameObject popcornPrefab;

    [SerializeField] private GameObject zoomPanel;

    [SerializeField] private TextMeshProUGUI player1ScoreText;
    [SerializeField] private TextMeshProUGUI player2ScoreText;
    [SerializeField] private GameObject[] SpritesPochoclos;

    private int player1Score = 0;
    private int player2Score = 0;

    [SerializeField] private GameObject finalText;
    [SerializeField] private GameObject[] finalSprites;


    public string sceneName;

    private int currentStep = 0;
    private bool readyForNext = false;

    private Coroutine popcornCoroutine;
    private List<GameObject> activePopcorns = new List<GameObject>();

    private void Start()
    {
        StartCoroutine(PlayStep(currentStep));
    }

    private void Update()
    {
        if (readyForNext && Input.GetKeyDown(KeyCode.Q))
        {
            currentStep++;
            StartCoroutine(PlayStep(currentStep));
        }
    }

    private IEnumerator PlayStep(int step)
    {
        readyForNext = false;

        switch (step)
        {
            case 0:
                yield return StartCoroutine(KidKornEntrance());
                break;

            case 1:
                yield return StartCoroutine(CanvasZoomIn());
                break;

            case 2:

                ShowCharacters(true);
                tutorialText1.SetActive(true);
                mainBar.SetActive(true);
                foreach (var item in anillas)
                {
                    item.SetActive(true);
                }
                break;

            case 3:
                StartCoroutine(KidKornMove2());
                tutorialText1.SetActive(false);
                tutorialText2.SetActive(true);
                if (player1Score > 0)
                {
                    player1ScoreText.gameObject.SetActive(true);
                    player2ScoreText.gameObject.SetActive(true);

                    foreach (var sprite in SpritesPochoclos)
                    {
                        sprite.SetActive(true);
                    }
                }
                UpdateScoreUI();
                ShowMainBar(true);
                ShowAnillas(true);

                popcornCoroutine = StartCoroutine(SpawnPopcornRoutine());
                break;

            case 4:
                StartCoroutine(KidKornMove3());
                tutorialText2.SetActive(false);
                ShowCharacters(false);
                ShowAnillas(false);
                ShowMainBar(false);

                foreach (var sprite in SpritesPochoclos)
                {
                    sprite.SetActive(false);
                }

                player1ScoreText.gameObject.SetActive(false);
                player2ScoreText.gameObject.SetActive(false);

                // Detener aparición de pochoclos
                if (popcornCoroutine != null)
                {
                    StopCoroutine(popcornCoroutine);
                    popcornCoroutine = null;
                }

                // Destruir todos los pochoclos activos
                foreach (var p in activePopcorns)
                {
                    if (p != null) Destroy(p);
                }
                activePopcorns.Clear();

                ShowFinal(true);
                break;
            case 5:
                LoadScene();
                break;
        }

        /* ⏱ ESPERA DE 2 s EN TODOS LOS PASOS */
        yield return new WaitForSeconds(1.5f);

        /* ────────── LÓGICA DE AVANCE ────────── */
        if (step < 3 || step == 4)                 // steps 0 y 1 → avance automático
        {
            currentStep++;
            StartCoroutine(PlayStep(currentStep));
        }
        else                          // steps ≥2 → esperar tecla Q
        {
            readyForNext = true;
        }
    }

    // Instancia 1: Movimiento de Kid Korn al centro con tween
    private IEnumerator KidKornEntrance()
    {
        float duration = 3f;
        Vector3 startPos = kidKorn.transform.position;
        Vector3 endPos = kidTargetPosition.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            kidKorn.transform.position = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }
    }

    private IEnumerator KidKornMove2()
    {
        float duration = 0.5f;
        Vector3 startPos = kidKorn.transform.position;
        Vector3 endPos = kidTargetPosition2.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            kidKorn.transform.position = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0,1,t));
            yield return null;
        }
    }

    private IEnumerator KidKornMove3()
    {
        float duration = 0.5f;
        Vector3 startPos = kidKorn.transform.position;
        Vector3 endPos = kidTargetPosition3.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            kidKorn.transform.position = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }
    }

    // Instancia 2: Zoom in de la cámara
    private IEnumerator CanvasZoomIn()
    {
        float duration = 0.6f;
        float startY = 1980f;
        float endY = 1080f;
        float elapsed = 0f;

        Vector2 originalRes = canvasScaler.referenceResolution;


        if (zoomPanel != null)
            zoomPanel.SetActive(true);

        canvasScaler.referenceResolution = new Vector2(originalRes.x, startY);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            float currentY = Mathf.Lerp(startY, endY, t);

            canvasScaler.referenceResolution = new Vector2(originalRes.x, currentY);
            yield return null;
        }

        // Aseguramos que termine exacto
        canvasScaler.referenceResolution = new Vector2(originalRes.x, endY);

        // Desactivar panel luego del zoom
        if (zoomPanel != null)
            zoomPanel.SetActive(false);
    }


    // Instancia 3 y 4: Mostrar personajes / barra / texto
    private void ShowCharacters(bool visible)
    {
        foreach (var c in characters)
            c.SetActive(visible);
    }

    private void ShowMainBar(bool visible)
    {
        if (mainBar != null)
            mainBar.SetActive(visible);
    }

    private void ShowAnillas(bool visible)
    {
        foreach (var a in anillas)
            a.SetActive(visible);
    }

    private void LoadScene()
    {
        SceneTransitionController.Instance.TransitionToScene(sceneName);
    }

    // Instancia 4: Generar pochoclos entre márgenes
    private IEnumerator SpawnPopcornRoutine()
    {
        SpriteRenderer barSprite = mainBar.GetComponent<SpriteRenderer>();
        float barWidth = barSprite.bounds.size.x;
        float barLeft = mainBar.transform.position.x - barWidth / 2f + barWidth * 0.2f;
        float barRight = mainBar.transform.position.x + barWidth / 2f - barWidth * 0.2f;
        float barY = mainBar.transform.position.y + 0.5f;

        while (true)
        {
            int count = Random.Range(5, 8);
            for (int i = 0; i < count; i++)
            {
                float x = Random.Range(barLeft, barRight);
                Vector2 pos = new Vector2(x, barY);
                GameObject popcorn = Instantiate(popcornPrefab, pos, Quaternion.identity);
                activePopcorns.Add(popcorn);
            }

            yield return new WaitForSeconds(4f);
        }
    }

    private void ShowFinal(bool visible)
    {
        if (finalText != null)
            finalText.SetActive(visible);

        foreach (var sprite in finalSprites)
        {
            if (sprite != null)
                sprite.SetActive(visible);
        }
    }
    private void UpdateScoreUI()
    {
        if (player1ScoreText != null)
            player1ScoreText.text = $"{player1Score.ToString("D2")}";

        if (player2ScoreText != null)
            player2ScoreText.text = $"{player2Score.ToString("D2")}";
    }

    public void AddScore(bool isPlayer1)
    {
        if (isPlayer1)
            player1Score++;
        else
            player2Score++;

        if (player1Score > 0 || player2Score > 0)
        {
            player1ScoreText.gameObject.SetActive(true);
            player2ScoreText.gameObject.SetActive(true);

            foreach (var sprite in SpritesPochoclos)
            {
                sprite.SetActive(true);
            }
        }

        UpdateScoreUI();
    }

}
