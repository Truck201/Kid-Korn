using UnityEngine;

public class CombosManager : MonoBehaviour
{
    public static CombosManager Instance;

    private int comboP1 = 0;
    private int comboP2 = 0;

    [Header("Referencias")]
    [SerializeField] private ComboUI comboUI1;
    [SerializeField] private ComboUI comboUI2;
    [SerializeField] private PlayerCharacter player1;
    [SerializeField] private PlayerCharacter player2;

    private void Awake()
    {
        Instance = this;
    }

    public void AddCombo(int playerId)
    {
        if (playerId == 1)
        {
            comboP1 = Mathf.Min(10, comboP1 + 1);
            comboUI1.UpdateCombo(comboP1);
        }
        else if (playerId == 2)
        {
            comboP2 = Mathf.Min(10, comboP2 + 1);
            comboUI2.UpdateCombo(comboP2);
        }

        // Check si alguien llegó a combo 10
        if (comboP1 >= 10)
        {
            TriggerComboVictory(winner: 1);
        }
        else if (comboP2 >= 10)
        {
            TriggerComboVictory(winner: 2);
        }
    }

    private void TriggerComboVictory(int winner)
    {
        // Recompensa
        ManageScore.Instance?.SpendPoints(1, -20); // equivalente a sumar
        ManageScore.Instance?.SpendPoints(2, -20);

        // Animaciones
        if (winner == 1)
        {
            player1.PlayLaugh();
            player2.PlayAngry();
        }
        else
        {
            player2.PlayLaugh();
            player1.PlayAngry();
        }

        // Reset combos
        comboP1 = 0;
        comboP2 = 0;
        comboUI1.ResetCombo();
        comboUI2.ResetCombo();
    }

    public int GetCombo(int playerId)
    {
        return playerId == 1 ? comboP1 : playerId == 2 ? comboP2 : 0;
    }
}
