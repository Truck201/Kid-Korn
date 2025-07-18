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
    [SerializeField] private GameObject tutorialText3;
    [SerializeField] private GameObject tutorialText4;
    [SerializeField] private GameObject tutorialText5;
    [SerializeField] private GameObject tutorialText6;
    [SerializeField] private GameObject tutorialText7;

    [SerializeField] private TutorialControlsUI tutorialControlsUI;

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

    [SerializeField] private UnityEngine.Rendering.Volume volumeToBlend;

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
        if (PauseManager.isGameLogicPaused) return;
        var input = GlobalInputManager.Instance;

        if (readyForNext && input.skipP1 || input.skipP2)
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
                tutorialControlsUI.gameObject.SetActive(false);
                break;

            case 1:
                yield return StartCoroutine(CanvasZoomIn());
                tutorialControlsUI.gameObject.SetActive(false);
                break;

            case 2:
                tutorialControlsUI.gameObject.SetActive(true); // Mostrar panel
                tutorialControlsUI.ShowOnlyPlayer1();          // Mostrar solo los de P1

                ShowCharacters(true);
                mainBar.SetActive(true);

                // Activar anillas
                foreach (var item in anillas)
                    item.SetActive(true);

                tutorialText1.SetActive(true);

                // Desactivar anilla P2 (grisar)
                SetAnillaState(0, true);   // Player 1 activa
                SetAnillaState(1, false);  // Player 2 inactiva/opaca

                yield return new WaitUntil(() => GlobalInputManager.Instance.skipP1); // Presiona Q
                tutorialText1.SetActive(false);
                tutorialText2.SetActive(true);
                SetAnillaState(0, false);  // Player 1 ahora inactiva
                SetAnillaState(1, true);   // Player 2 activa

                tutorialControlsUI.ShowOnlyPlayer2(); // Mostrar solo los de P2

                yield return new WaitUntil(() => GlobalInputManager.Instance.skipP2); // Presiona Q
                tutorialText2.SetActive(false);
                tutorialText3.SetActive(true);

                tutorialControlsUI.ShowDisable();
                // Ambas anillas activas
                SetAnillaState(0, true);
                SetAnillaState(1, true);

                break;


            case 3:
                StartCoroutine(KidKornMove2());
                tutorialControlsUI.ShowDisable();
                tutorialText3.SetActive(false);
                tutorialText4.SetActive(true);
                tutorialText5.SetActive(true);
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
                tutorialText4.SetActive(false);
                tutorialText5.SetActive(false);
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
        if (step < 2 || step == 4)                 // steps 0 y 1 → avance automático
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
        float duration = 0.2f;
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
        float duration = 0.2f;
        float startY = 1080f;
        float endY = 640f;
        float elapsed = 0f;

        Vector2 originalRes = canvasScaler.referenceResolution;

        if (zoomPanel != null)
            zoomPanel.SetActive(true);

        canvasScaler.referenceResolution = new Vector2(originalRes.x, startY);

        float startBlend = volumeToBlend != null ? volumeToBlend.blendDistance : 0f;
        float targetBlend = 400f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            float currentY = Mathf.Lerp(startY, endY, t);

            canvasScaler.referenceResolution = new Vector2(originalRes.x, currentY);

            if (volumeToBlend != null)
                volumeToBlend.blendDistance = Mathf.Lerp(startBlend, targetBlend, t);

            yield return null;
        }

        // Aseguramos valores exactos al final
        canvasScaler.referenceResolution = new Vector2(originalRes.x, endY);
        if (volumeToBlend != null)
            volumeToBlend.blendDistance = targetBlend;

        if (zoomPanel != null)
            zoomPanel.SetActive(false);
    }



    // Instancia 3 y 4: Mostrar personajes / barra / texto
    private void ShowCharacters(bool visible)
    {
        foreach (var c in characters)
            c.SetActive(visible);
    }

    private void SetAnillaState(int index, bool active)
    {
        if (index >= anillas.Length) return;

        var movement = anillas[index].GetComponent<AnillaMovement>();
        var sprite = anillas[index].GetComponent<SpriteRenderer>();

        if (movement != null)
            movement.canMove = active;

        if (sprite != null)
            sprite.color = active ? Color.white : new Color(1f, 1f, 1f, 0.3f); // opaco
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
            int count = Random.Range(6, 9);
            for (int i = 0; i < count; i++)
            {
                float x = Random.Range(barLeft, barRight);
                Vector2 pos = new Vector2(x, barY);
                GameObject popcorn = Instantiate(popcornPrefab, pos, Quaternion.identity);
                activePopcorns.Add(popcorn);
            }

            yield return new WaitForSeconds(3.5f);
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
