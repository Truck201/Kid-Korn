[System.Serializable]
public class ItemDefinition
{
    public string name;
    public int price;
    public string description;

    public ItemDefinition(string name, int price, string description)
    {
        this.name = name;
        this.price = price;
        this.description = description;
    }
}