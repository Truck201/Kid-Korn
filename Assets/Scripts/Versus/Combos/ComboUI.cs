using UnityEngine;
using TMPro;

public class ComboUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI comboText;
    private int currentCombo = 0;

    public void UpdateCombo(int value)
    {
        currentCombo = value;

        if (value >= 5)
        {
            comboText.gameObject.SetActive(true);
            comboText.text = $"{value}";
        }
        else
        {
            comboText.gameObject.SetActive(false);
        }
    }

    public void ResetCombo()
    {
        currentCombo = 0;
        comboText.gameObject.SetActive(false);
    }
}
