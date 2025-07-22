using UnityEngine;

public class PointParticleMover : MonoBehaviour
{
    private Vector3 targetPosition;
    private float speed = 300f;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetTarget(Vector3 target)
    {
        targetPosition = target;
    }

    public void SetColor(string hexColor)
    {
        if (ColorUtility.TryParseHtmlString(hexColor, out Color color))
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();

            spriteRenderer.color = color;
        }
        else
        {
            Debug.LogWarning("Color inválido: " + hexColor);
        }
    }

    void Update()
    {
        if (PauseManager.isGameLogicPaused) return;

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.05f)
        {
            Destroy(gameObject);
        }
    }
}
