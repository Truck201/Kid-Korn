using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class ItemShopManager : MonoBehaviour
{
    [Header("Grid Configuration")]
    public int rows = 4;
    public int cols = 4;
    public float itemSize = 82f;
    public float spacing = 10f;
    public Vector2 gridStartPosition; // centro de la grilla
    public Transform gridParent;

    [Header("Item Prefabs")]
    public List<GameObject> itemPrefabs; // Prefabs con datos definidos (nombre, precio, etc.)
    public int maxRepeatsPerItem = 3;

    [Header("Cursor")]
    public GameObject cursorPrefab;

    [Header("Puntaje y slots")]
    public int player1Points = 15;
    public int player2Points = 15;
    public Transform player1SlotParent;
    public Transform player2SlotParent;
    public int maxItemsPerPlayer = 3;

    [Header("UI de Skip")]
    public TextMeshProUGUI skipTextP1;
    public TextMeshProUGUI skipTextP2;

    [Header("Info Panels")]
    public ItemInfoPanel player1InfoPanel;
    public ItemInfoPanel player2InfoPanel;

    private List<ItemData> allItems = new();
    private List<ItemData> selectedItemsPlayer1 = new();
    private List<ItemData> selectedItemsPlayer2 = new();

    private Vector2Int player1Pos, player2Pos;
    private GameObject player1Cursor, player2Cursor;

    private List<GameObject> player1SlotVisuals = new();
    private List<GameObject> player2SlotVisuals = new();

    [Header("Puntos UI")]
    public TextMeshProUGUI player1PointsText;
    public TextMeshProUGUI player2PointsText;

    private bool inputsEnabled = false;
    private float inputDelay = 0.2f;
    private float p1InputTimer = 0f;
    private float p2InputTimer = 0f;

    private bool p1WantsToSkip = false;
    private bool p2WantsToSkip = false;

    private void Start()
    {
        player1Points = ManageScore.Instance != null ? ManageScore.Instance.GetScore(1) : 0;
        player2Points = ManageScore.Instance != null ? ManageScore.Instance.GetScore(2) : 0;

        CreateGridItems();

        player1Pos = new Vector2Int(0, 0);
        player2Pos = new Vector2Int(cols - 1, 0); // arriba derecha

        player1Cursor = Instantiate(cursorPrefab, gridParent);
        player2Cursor = Instantiate(cursorPrefab, gridParent);
        player1Cursor.GetComponent<SpriteRenderer>().color = Color.red;
        player2Cursor.GetComponent<SpriteRenderer>().color = Color.blue;

        UpdatePointsUI();
        UpdateCursorPositions();
        ShowInitialItemInfo();

        EnableInputs(); // ⬅️ Asegurate de llamarlo aquí si no se hace desde otro lugar
        UpdateSkipTextUI();
    }

    private void CreateGridItems()
    {
        allItems.Clear();
        int totalItems = rows * cols;
        List<GameObject> shuffledItems = GenerateShuffledItems(totalItems, maxRepeatsPerItem);

        // Centrar la grilla
        Vector2 totalSize = new Vector2((cols - 1) * (itemSize + spacing), (rows - 1) * (itemSize + spacing));
        Vector2 startOffset = gridStartPosition - totalSize / 2f;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                int index = row * cols + col;
                if (index >= shuffledItems.Count) return;

                Vector3 position = new Vector3(
                    startOffset.x + col * (itemSize + spacing),
                    startOffset.y - row * (itemSize + spacing),
                    0f
                );

                GameObject itemGO = Instantiate(shuffledItems[index], gridParent);
                itemGO.transform.localPosition = position;

                ItemData itemData = itemGO.GetComponent<ItemData>();
                itemData.row = row;
                itemData.col = col;
                itemData.InitializeFromPrefab();

                allItems.Add(itemData);
            }
        }
    }

    private List<GameObject> GenerateShuffledItems(int totalItems, int maxPerType)
    {
        List<GameObject> itemPool = new();

        foreach (var prefab in itemPrefabs)
        {
            for (int i = 0; i < maxPerType; i++)
                itemPool.Add(prefab);
        }

        Shuffle(itemPool);
        return itemPool.GetRange(0, Mathf.Min(totalItems, itemPool.Count));
    }

    private void Shuffle(List<GameObject> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }

    private void Update()
    {
        if (!inputsEnabled) return;
        if (PauseManager.isGameLogicPaused) return;

        UpdateSkipTextUI();

        var input = GlobalInputManager.Instance;
        Debug.Log($"P1: {input.moveP1} | P2: {input.moveP2}");

        p1InputTimer -= Time.deltaTime;
        p2InputTimer -= Time.deltaTime;

        if (input.skipP1) p1WantsToSkip = true;
        if (input.skipP2) p2WantsToSkip = true;

        if (p1WantsToSkip && p2WantsToSkip)
        {
            Debug.Log("Ambos jugadores skipearon la tienda.");
            SceneManager.LoadScene("BattleScene"); // o lo que corresponda
            return;
        }

        HandlePlayerInput(1, ref player1Pos, input.moveP1, input.selectP1, ref p1InputTimer);
        HandlePlayerInput(2, ref player2Pos, input.moveP2, input.selectP2, ref p2InputTimer);
        UpdateCursorPositions();
    }

    private void HandlePlayerInput(int player, ref Vector2Int pos, Vector2 moveInput, bool select, ref float inputTimer)
    {
        if (inputTimer <= 0f)
        {
            Vector2Int dir = Vector2Int.zero;

            if (moveInput.y > 0.5f) dir = Vector2Int.down; // up
            else if (moveInput.y < -0.5f) dir = Vector2Int.up; // down
            else if (moveInput.x < -0.5f) dir = Vector2Int.left;
            else if (moveInput.x > 0.5f) dir = Vector2Int.right;

            if (dir != Vector2Int.zero)
            {
                pos += dir;
                pos.x = Mathf.Clamp(pos.x, 0, cols - 1);
                pos.y = Mathf.Clamp(pos.y, 0, rows - 1);
                inputTimer = inputDelay;

                ItemData hoveredItem = GetItemAtPosition(pos.y, pos.x);
                if (hoveredItem != null)
                {
                    if (player == 1 && player1InfoPanel != null)
                        player1InfoPanel.SetItemInfo(hoveredItem.definition);
                    else if (player == 2 && player2InfoPanel != null)
                        player2InfoPanel.SetItemInfo(hoveredItem.definition);
                }
            }
        }

        if (select)
        {
            ItemData item = GetItemAtPosition(pos.y, pos.x);
            if (item != null)
                TrySelectItem(player, item);
        }
    }


    private void TrySelectItem(int player, ItemData item)
    {
        if (!inputsEnabled) return;
        if (player == 1)
        {
            // Deseleccionar
            if (item.ownedByPlayer == 1 && selectedItemsPlayer1.Contains(item))
            {
                int index = selectedItemsPlayer1.IndexOf(item);
                selectedItemsPlayer1.RemoveAt(index);
                player1Points += item.definition.price;
                item.Deselect();

                if (index < player1SlotVisuals.Count)
                {
                    Destroy(player1SlotVisuals[index]);
                    player1SlotVisuals.RemoveAt(index);
                    RepositionSlotVisuals(player1SlotVisuals, player1SlotParent);
                }
            }
            // No permitir si fue comprado por el otro
            else if (item.ownedByPlayer != 0 && item.ownedByPlayer != 1)
            {
                Debug.Log("Jugador 1 no puede comprar este ítem.");
                return;
            }
            // Comprar
            else if (selectedItemsPlayer1.Count < maxItemsPerPlayer && player1Points >= item.definition.price)
            {
                selectedItemsPlayer1.Add(item);
                player1Points -= item.definition.price;
                item.Select(1);

                GameObject visual = Instantiate(item.gameObject);
                visual.transform.SetParent(player1SlotParent);
                visual.transform.localScale = Vector3.one * 0.7f;
                visual.transform.localPosition = Vector3.zero;

                Destroy(visual.GetComponent<ItemData>());
                Destroy(visual.GetComponent<Collider2D>());

                player1SlotVisuals.Add(visual);
                RepositionSlotVisuals(player1SlotVisuals, player1SlotParent);
            }
        }
        else if (player == 2)
        {
            if (item.ownedByPlayer == 2 && selectedItemsPlayer2.Contains(item))
            {
                int index = selectedItemsPlayer2.IndexOf(item);
                selectedItemsPlayer2.RemoveAt(index);
                player2Points += item.definition.price;
                item.Deselect();

                if (index < player2SlotVisuals.Count)
                {
                    Destroy(player2SlotVisuals[index]);
                    player2SlotVisuals.RemoveAt(index);
                    RepositionSlotVisuals(player2SlotVisuals, player2SlotParent);
                }
            }
            else if (item.ownedByPlayer != 0 && item.ownedByPlayer != 2)
            {
                Debug.Log("Jugador 2 no puede comprar este ítem.");
                return;
            }
            else if (selectedItemsPlayer2.Count < maxItemsPerPlayer && player2Points >= item.definition.price)
            {
                selectedItemsPlayer2.Add(item);
                player2Points -= item.definition.price;
                item.Select(2);

                GameObject visual = Instantiate(item.gameObject);
                visual.transform.SetParent(player2SlotParent);
                visual.transform.localScale = Vector3.one * 0.7f;
                visual.transform.localPosition = Vector3.zero;

                Destroy(visual.GetComponent<ItemData>());
                Destroy(visual.GetComponent<Collider2D>());

                player2SlotVisuals.Add(visual);
                RepositionSlotVisuals(player2SlotVisuals, player2SlotParent);
            }
        }

        UpdatePointsUI();
    }

    private void UpdateCursorPositions()
    {
        ItemData item1 = GetItemAtPosition(player1Pos.y, player1Pos.x);
        ItemData item2 = GetItemAtPosition(player2Pos.y, player2Pos.x);

        if (item1 != null) player1Cursor.transform.localPosition = item1.transform.localPosition;
        if (item2 != null) player2Cursor.transform.localPosition = item2.transform.localPosition;
    }

    private ItemData GetItemAtPosition(int row, int col)
    {
        return allItems.Find(i => i.row == row && i.col == col);
    }

    private void UpdatePointsUI()
    {
        if (player1PointsText != null)
            player1PointsText.text = $"{player1Points}";

        if (player2PointsText != null)
            player2PointsText.text = $"{player2Points}";
    }

    private void ShowInitialItemInfo()
    {
        // Obtener ítem actual de cada jugador
        ItemData item1 = GetItemAtPosition(player1Pos.y, player1Pos.x);
        ItemData item2 = GetItemAtPosition(player2Pos.y, player2Pos.x);

        if (item1 != null && player1InfoPanel != null)
            player1InfoPanel.SetItemInfo(item1.definition);

        if (item2 != null && player2InfoPanel != null)
            player2InfoPanel.SetItemInfo(item2.definition);
    }

    private void RepositionSlotVisuals(List<GameObject> visuals, Transform parent)
    {
        float spacingY = itemSize * 7f; // Espaciado vertical
        for (int i = 0; i < visuals.Count; i++)
        {
            if (visuals[i] != null)
            {
                visuals[i].transform.localPosition = new Vector3(0f, -i * spacingY, 0f);
            }
        }
    }

    public void EnableInputs()
    {
        inputsEnabled = true;
    }

    private void UpdateSkipTextUI()
    {
        var globalInput = GlobalInputManager.Instance;
        if (globalInput == null) return;

        var inputActions = globalInput.InputActions;

        InputDevice deviceP1 = (InputDevice)globalInput.GetGamepadForPlayer(1);
        if (deviceP1 == null) deviceP1 = Keyboard.current;

        InputDevice deviceP2 = (InputDevice)globalInput.GetGamepadForPlayer(2);
        if (deviceP2 == null) deviceP2 = Keyboard.current;

        var skipP1 = inputActions.Gameplay.SkipP1;
        var skipP2 = inputActions.Gameplay.SkipP2;

        string skipDisplayP1 = InputUtils.GetBindingDisplay(skipP1, 1, deviceP1);
        string skipDisplayP2 = InputUtils.GetBindingDisplay(skipP2, 2, deviceP2);

        if (skipTextP1 != null)
        {
            string colorTag = p1WantsToSkip ? "#FE004D" : "white"; // amarillo dorado
            skipTextP1.text = $"<color={colorTag}>P1 - Presiona ({skipDisplayP1}) para continuar</color>";
        }

        if (skipTextP2 != null)
        {
            string colorTag = p2WantsToSkip ? "#28ADFE" : "white"; // amarillo dorado 28ADFE   055AB5
            skipTextP2.text = $"<color={colorTag}>P2 - Presiona ({skipDisplayP2}) para continuar</color>";
        }
    }

}
