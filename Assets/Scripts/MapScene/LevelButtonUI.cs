using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LevelButtonUI : MonoBehaviour, IPointerEnterHandler
{
    public int levelId;
    public bool isLocked = false;
    public Image lockIcon;
    public Image buttonImage;

    private LevelMapManager mapManager;

    private void Start()
    {
        mapManager = Object.FindFirstObjectByType<LevelMapManager>();
        UpdateVisual();
    }

    public void UpdateVisual()
    {
        lockIcon.gameObject.SetActive(isLocked);
        buttonImage.color = Color.white;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mapManager.SetCurrentSelection(this);
    }

    public void Highlight()
    {
        buttonImage.color = isLocked ? Color.red : Color.yellow;
    }

    public void RemoveHighlight()
    {
        buttonImage.color = Color.white;
    }

    // NUEVA FUNCIÓN: para cambiar estado y actualizar íconos
    public void SetLockedState(bool locked)
    {
        isLocked = locked;
        UpdateVisual();
    }
}
