using UnityEngine;

public class ItemData : MonoBehaviour
{
    public ItemDefinition definition;
    public int ownedByPlayer = 0; // 0 = nadie, 1 = jugador1, 2 = jugador2

    public int row, col;

    public void InitializeFromPrefab()
    {
        ItemDefinitionHolder holder = GetComponent<ItemDefinitionHolder>();
        if (holder != null)
        {
            definition = holder.GetDefinition();
            Debug.Log("Cargado: " + definition.name + " - $" + definition.price);
        }
        else
        {
            Debug.LogWarning("No se encontró ItemDefinitionHolder en " + gameObject.name);
        }
    }


    public void Select(int player)
    {
        ownedByPlayer = player;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = player == 1 ? new Color(1f, 0.2f, 0.2f, 1f) : new Color(0.2f, 0.2f, 1f, 1f); // Menos intensos (1f, 0.4f, 0.4f, 0.6f) : (0.4f, 0.4f, 1f, 0.6f);
        }
    }

    public void Deselect()
    {
        ownedByPlayer = 0;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = Color.white;
        }
    }
}
