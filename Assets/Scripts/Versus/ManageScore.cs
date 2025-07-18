using UnityEngine;

public class ManageScore : MonoBehaviour
{
    public static ManageScore Instance { get; private set; }

    public int scorePlayer1 = 0;
    public int scorePlayer2 = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // evitar duplicados
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // persiste entre escenas
    }

    public void SetScore(int p1, int p2)
    {
        Debug.Log("sape");
        scorePlayer1 = p1;
        scorePlayer2 = p2;
    }

    public int GetScore(int playerId)
    {
        Debug.Log("pedilo");
        return playerId == 1 ? scorePlayer1 : scorePlayer2;
    }

    public void SpendPoints(int playerId, int amount)
    {
        if (playerId == 1)
            scorePlayer1 = Mathf.Max(0, scorePlayer1 - amount);
        else if (playerId == 2)
            scorePlayer2 = Mathf.Max(0, scorePlayer2 - amount);
    }
}
