using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemInfoPanel : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI descriptionText;

    public void SetItemInfo(ItemDefinition def)
    {
        if (def == null)
        {
            nameText.text = "";
            priceText.text = "";
            descriptionText.text = "";
            return;
        }

        nameText.text = def.name;
        priceText.text = "$" + def.price.ToString();
        descriptionText.text = def.description;
    }
}
