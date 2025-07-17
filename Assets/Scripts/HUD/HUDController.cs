using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI player1ScoreText;
    [SerializeField] private TextMeshProUGUI player2ScoreText;

    public void UpdateTimer(int secondsLeft)
    {
        timerText.text = "" + secondsLeft.ToString();
    }

    public void UpdateScore(int playerId, int score)
    {
        if (playerId == 1)
            player1ScoreText.text = $"{score.ToString("D2")}";
        else if (playerId == 2)
            player2ScoreText.text = $"{score.ToString("D2")}";
    }
}
