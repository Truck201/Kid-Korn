using UnityEngine;
using System.Collections;


public class VersusGameManager : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private GameObject popcornPrefab;
    [SerializeField] private GameObject mainBar;
    [SerializeField] private HUDController hudController;

    [Header("Config")]
    [SerializeField] private int minPopcorn = 7;
    [SerializeField] private int maxPopcorn = 9;
    [SerializeField] private int gameDuration = 45; // segundos

    [SerializeField] private Transform scoreTargetP1;
    [SerializeField] private Transform scoreTargetP2;

    [SerializeField] private GameObject pointParticlePrefab;

    public float TimeLeft => timer;

    private float timer;
    private int scoreP1 = 0;
    private int scoreP2 = 0;

    private void Start()
    {
        timer = gameDuration;
    }

    private void Update()
    {
        if (PauseManager.isGameLogicPaused) return;
        timer -= Time.deltaTime;

        if (timer < 0)
        {
            timer = 0;

            // Guardar scores
            ManageScore.Instance?.SetScore(scoreP1, scoreP2);
        }

        int secondsLeft = Mathf.CeilToInt(timer);
        hudController.UpdateTimer(secondsLeft);
    }

    public void GeneratePopcornNow(bool isBig = false)
    {
        StartCoroutine(GeneratePopcornAfterUnpause(isBig));
    }

    private IEnumerator GeneratePopcornAfterUnpause(bool isBig)
    {
        yield return CoroutineUtils.WaitWhileUnpaused(0f); // espera si está pausado
        yield return StartCoroutine(GeneratePopcornRoutine(isBig));
    }

    private IEnumerator GeneratePopcornRoutine(bool isBig)
    {
        int totalPopcorn = isBig ? 9 : Random.Range(minPopcorn, maxPopcorn + 1);

        SpriteRenderer barSprite = mainBar.GetComponent<SpriteRenderer>();
        float barWidth = barSprite.bounds.size.x;

        float margin = barWidth * 0.02f;

        float barLeft = mainBar.transform.position.x - barWidth / 2f + margin;
        float barRight = mainBar.transform.position.x + barWidth / 2f - margin;
        float barY = barSprite.bounds.center.y + 1f;

        for (int i = 0; i < totalPopcorn; i++)
        {
            float randomX = Random.Range(barLeft, barRight); // sin +1/-1 ahora
            Vector2 spawnPos = new Vector2(randomX, barY);

            Instantiate(popcornPrefab, spawnPos, Quaternion.identity);

            yield return CoroutineUtils.WaitWhileUnpaused(Random.Range(0.1f, 0.3f));
        }
    }

    public void AddScore(int playerId, int combo)
    {
        int pointsToAdd = 1; // default para combo < 5

        switch (combo)
        {
            case 5: pointsToAdd = 2; break;
            case 6: pointsToAdd = 3; break;
            case 7: pointsToAdd = 4; break;
            case 8: pointsToAdd = 5; break;
            case 9: pointsToAdd = 6; break;
            case 10: pointsToAdd = 10; break;
        }

        if (playerId == 1)
        {
            scoreP1 += pointsToAdd;
            hudController.UpdateScore(1, scoreP1);
        }
        else if (playerId == 2)
        {
            scoreP2 += pointsToAdd;
            hudController.UpdateScore(2, scoreP2);
        }
    }

    public void SpawnScoreParticle(Vector3 fromWorldPos, int playerId)
    {
        Transform target = playerId == 1 ? scoreTargetP1 : scoreTargetP2;
        if (target == null) return;

        GameObject particle = Instantiate(pointParticlePrefab, fromWorldPos, Quaternion.identity);
        PointParticleMover mover = particle.GetComponent<PointParticleMover>();
        mover.SetTarget(target.position);

        // Elegí colores en hexadecimal
        string hexColor = playerId == 1 ? "#FD0E56" : "#2FB2FA"; // rojo o azul
        mover.SetColor(hexColor);
    }
}
