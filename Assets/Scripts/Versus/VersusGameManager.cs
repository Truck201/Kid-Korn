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
        if (PauseManager.isGameLogicPaused) return;
        StartCoroutine(GeneratePopcornRoutine(isBig));
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

    public void AddScore(int playerId)
    {
        if (playerId == 1)
        {
            scoreP1++;
            hudController.UpdateScore(1, scoreP1);
        }
        else if (playerId == 2)
        {
            scoreP2++;
            hudController.UpdateScore(2, scoreP2);
        }
    }
}
