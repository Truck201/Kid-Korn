using UnityEngine;
using System.Collections;

public class KidKornManager : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private VersusGameManager gameManager;
    [SerializeField] private GameObject kidKornLeft;
    [SerializeField] private GameObject kidKornRight;
    [SerializeField] private GameObject kidKornBigContainer; // ¡nuevo!
    [SerializeField] private GameObject kidKornBig;
    private GameObject korn;

    [Header("Audio")]
    [SerializeField] private AudioSource appearSound;
    [SerializeField] private AudioSource goBackSound;
    [SerializeField] private AudioSource dialogueSound;
    [SerializeField] private AudioSource screamSound;

    private float lastKidKornTime;
    private bool bigKornTriggered = false;

    private bool isKornActive = false;

    private void Start()
    {
        StartCoroutine(KidKornRoutine());
    }

    private void Update()
    {
        if (PauseManager.isGameLogicPaused) return;
        // Aparece el BigKidKorn si faltan menos de 7 segundos y aún no apareció
        if (!bigKornTriggered && gameManager.TimeLeft < 10f)
        {
            bigKornTriggered = true;
            StartCoroutine(ShowBigKidKorn());
        }
    }

    private IEnumerator KidKornRoutine()
    {
        while (gameManager.TimeLeft > 0f)
        {
            yield return CoroutineUtils.WaitWhileUnpaused(Random.Range(0.5f, 1f));

            if (gameManager.TimeLeft > 12.2f && !isKornActive)
            {
                yield return StartCoroutine(ShowKidKornCoroutine());
            }
        }
    }

    private IEnumerator ShowKidKornCoroutine()
    {
        isKornActive = true;

        float centerChance = 0.1f;
        bool isCenter = Random.value < centerChance;

        if (isCenter)
        {
            korn = kidKornLeft;
            float centerX = 0f;

            korn.transform.position = new Vector2(centerX, korn.transform.position.y);
            korn.SetActive(true);
            appearSound.Play();
            dialogueSound.Play();
            gameManager.GeneratePopcornNow();
            yield return StartCoroutine(StayAtCenter(korn, () =>
            {
                gameManager.GeneratePopcornNow();
            }));

            yield return StartCoroutine(HideKidKorn(korn, -720f));
        }
        else
        {
            bool fromLeft = Random.Range(0, 2) == 0;
            korn = fromLeft ? kidKornLeft : kidKornRight;
            float startX = fromLeft ? -520f : 520f;
            float endX = fromLeft ? -400f : 400f;

            korn.transform.position = new Vector2(startX, korn.transform.position.y);
            korn.SetActive(true);
            appearSound.Play();
            dialogueSound.Play();

            yield return StartCoroutine(MoveKorn(korn, endX));
            gameManager.GeneratePopcornNow();
            yield return StartCoroutine(HideKidKorn(korn, fromLeft ? -720f : 720f));
        }

        isKornActive = false;
    }

    private IEnumerator MoveKorn(GameObject korn, float targetX)
    {
        float distance = Mathf.Abs(korn.transform.position.x - targetX);
        float speed = 270f;
        float duration = distance / speed;

        bool finished = false;

        LeanTween.moveX(korn, targetX, duration)
                 .setEase(LeanTweenType.easeOutQuad)
                 .setOnComplete(() => finished = true);

        yield return new WaitUntil(() => finished);
    }


    private IEnumerator HideKidKorn(GameObject korn, float exitX)
    {
        Debug.Log("Esconder Kid Korn");
        yield return CoroutineUtils.WaitWhileUnpaused(2f);
        goBackSound.Play();
        yield return MoveKorn(korn, exitX);
        korn.SetActive(false);
    }

    private IEnumerator ShowBigKidKorn()
    {
        float startY = 0f;
        float targetY = 608f;
        float endY = -380f;
        float speed = 650f;

        kidKornBigContainer.transform.localPosition = new Vector2(0f, startY);
        kidKornBig.SetActive(true);

        // PlayAnimation(kidKornBig, "Appear"); // ← NUEVO

        kidKornBig.transform.localScale = Vector3.one * 0.7f;

        appearSound.Play();
        screamSound.Play();
        dialogueSound.Play();

        // Escala tipo “pop” al aparecer
        LeanTween.scale(kidKornBig, new Vector3(1.3f, 0.8f, 1f), 0.15f)
            .setEase(LeanTweenType.easeOutQuad)
            .setOnComplete(() =>
            {
                LeanTween.scale(kidKornBig, Vector3.one, 0.15f).setEase(LeanTweenType.easeOutBounce);
            });

        // Iniciar shake local del sprite mientras está activo
        StartCoroutine(ShakeWhileActive(kidKornBig.transform, 4f, 0.03f));

        // Subida con tween (solo el contenedor)
        float upDistance = Mathf.Abs(targetY - startY);
        float upTime = upDistance / speed;

        LeanTween.moveLocalY(kidKornBigContainer, targetY, upTime).setEase(LeanTweenType.linear);
        yield return CoroutineUtils.WaitWhileUnpaused(upTime);

        // Lanzar muchos pochoclos
        for (int i = 0; i < 5; i++)
        {
            gameManager.GeneratePopcornNow(true);
            yield return CoroutineUtils.WaitWhileUnpaused(0.7f);
        }

        // Espera en pantalla antes de salir
        yield return CoroutineUtils.WaitWhileUnpaused(2.5f);

        goBackSound.Play();

        // Bajada
        float downDistance = Mathf.Abs(targetY - endY);
        float downTime = downDistance / speed;

        LeanTween.moveLocalY(kidKornBigContainer, endY, downTime).setEase(LeanTweenType.linear);
        yield return CoroutineUtils.WaitWhileUnpaused(downTime);

        // PlayAnimation(kidKornBig, "Exit"); // ← NUEVO
        yield return CoroutineUtils.WaitWhileUnpaused(0.7f);

        kidKornBig.SetActive(false);
        screamSound.Stop();
    }


    private IEnumerator ShakeWhileActive(Transform target, float magnitude, float frequency)
    {
        Vector3 originalPos = target.localPosition;

        while (target.gameObject.activeSelf)
        {
            float offsetX = Random.Range(-0.3f, 0.3f) * magnitude;
            float offsetY = Random.Range(-0.3f, 0.3f) * magnitude;

            target.localPosition = originalPos + new Vector3(offsetX, offsetY, 0f);
            yield return CoroutineUtils.WaitWhileUnpaused(frequency);
        }

        target.localPosition = originalPos;
    }

    private IEnumerator StayAtCenter(GameObject korn, System.Action onDone)
    {
        Debug.Log("Kid Korn al centro");
        float duration = 2f;
        yield return CoroutineUtils.WaitWhileUnpaused(duration);
        onDone?.Invoke();
    }

    private void PlayAnimation(GameObject obj, string trigger)
    {
        Animator animator = obj.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger(trigger);
        }
    }
}
