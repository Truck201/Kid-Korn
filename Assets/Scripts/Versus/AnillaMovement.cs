using UnityEngine;

public class AnillaMovement : MonoBehaviour
{
    [Header("Configuraci�n")]
    [SerializeField] private GameObject mainBar;
    [SerializeField] private float speed = 3f;
    [SerializeField] private bool isPlayer1 = true;
    [SerializeField, Range(0f, 0.5f)] private float bounceMarginPercent = 0.008f; // margen relativo (1.8% por defecto)

    private Rigidbody2D ringBody;
    private SpriteRenderer spriteRenderer;
    private VersusGameManager gameManager;

    private int direction; // 1 = derecha, -1 = izquierda
    private float barLeftLimit;
    private float barRightLimit;
    private float ringHalfWidth;
    private float bounceMargin;

    public bool isInTutorial = false;
    private void Start()
    {
        ringBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        SpriteRenderer barSprite = mainBar.GetComponent<SpriteRenderer>();

        gameManager = Object.FindFirstObjectByType<VersusGameManager>();

        // Direcci�n inicial
        direction = isPlayer1 ? 1 : -1;

        // Calcular l�mites del mainBar
        float barWidth = barSprite.bounds.size.x;
        bounceMargin = barWidth * bounceMarginPercent;

        float barCenterX = mainBar.transform.position.x;
        barLeftLimit = barCenterX - (barWidth / 2f);
        barRightLimit = barCenterX + (barWidth / 2f);

        // Calcular la mitad del ancho de la anilla para evitar que se salga visualmente
        ringHalfWidth = spriteRenderer.bounds.size.x / 2f;

        // Posicionar al borde izquierdo o derecho
        float startX = isPlayer1 ? barLeftLimit + ringHalfWidth + bounceMargin
                                 : barRightLimit - ringHalfWidth - bounceMargin;
        float centerY = barSprite.bounds.center.y;
        ringBody.position = new Vector2(startX, centerY);
    }

    private void Update()
    {
        if (PauseManager.isGameLogicPaused) return;
        var input = GlobalInputManager.Instance;
        Vector2 inputDir = isPlayer1 ? input.moveP1 : input.moveP2;

        if (inputDir.x < -0.5f) direction = -1;
        else if (inputDir.x > 0.5f) direction = 1;

        if ((isPlayer1 && input.selectP1) || (!isPlayer1 && input.selectP2))
        {
            TryCollectPopcorn();
        }
    }

    private void TryCollectPopcorn()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.3f);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Popcorn"))
            {
                Destroy(hit.gameObject);
                if (gameManager != null)
                {
                    gameManager.AddScore(isPlayer1 ? 1 : 2);
                }

                if (isInTutorial == true)
                {
                    Tutorial_Manager tutorialManager = Object.FindFirstObjectByType<Tutorial_Manager>();
                    if (tutorialManager != null)
                    {
                        tutorialManager.AddScore(isPlayer1);
                    }
                }
                break;
            }
        }
    }

    private void FixedUpdate()
    {
        if (PauseManager.isGameLogicPaused) return;
        Vector2 velocity = new Vector2(direction * speed, 0);
        Vector2 newPosition = ringBody.position + velocity * Time.fixedDeltaTime;

        // Chequear rebote en bordes
        if (newPosition.x - ringHalfWidth <= barLeftLimit + bounceMargin)
        {
            newPosition.x = barLeftLimit + bounceMargin + ringHalfWidth;
            direction = 1; // Rebota hacia la derecha
        }
        else if (newPosition.x + ringHalfWidth >= barRightLimit - bounceMargin)
        {
            newPosition.x = barRightLimit - bounceMargin - ringHalfWidth;
            direction = -1; // Rebota hacia la izquierda
        }

        ringBody.MovePosition(newPosition);
    }
}
