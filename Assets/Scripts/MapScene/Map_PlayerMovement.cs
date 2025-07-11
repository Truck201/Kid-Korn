using UnityEngine;

public class Map_PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    private Rigidbody2D playerRb;
    private Vector2 moveInput;

    private void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(moveX, moveY).normalized;
    }

    private void FixedUpdate()
    {
        playerRb.MovePosition(playerRb.position + moveInput * speed * Time.fixedDeltaTime);

        // Asegura que se mantenga en Z = 0
        Vector3 pos = transform.position;
        pos.z = 0f;
        transform.position = pos;
    }
}
