using UnityEngine;

public class ItemDefinitionHolder : MonoBehaviour
{
    public string itemName;
    public int price;
    [TextArea] public string description;

    public ItemDefinition GetDefinition()
    {
        return new ItemDefinition(itemName, price, description);
    }
}
