using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform player;                    // Referencia al jugador
    [SerializeField] private SpriteRenderer mapBoundsRenderer;    // Sprite del mapa (con SpriteRenderer)
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);

    private float camHalfWidth;
    private float camHalfHeight;

    private float minX, maxX, minY, maxY;

    private void Start()
    {
        Camera cam = Camera.main;
        camHalfHeight = cam.orthographicSize;
        camHalfWidth = cam.orthographicSize * cam.aspect;

        if (mapBoundsRenderer != null)
        {
            Bounds bounds = mapBoundsRenderer.bounds;

            minX = bounds.min.x + camHalfWidth;
            maxX = bounds.max.x - camHalfWidth;
            minY = bounds.min.y + camHalfHeight;
            maxY = bounds.max.y - camHalfHeight;
        }
    }

    private void LateUpdate()
    {
        if (player == null) return;

        Vector3 targetPos = player.position + offset;

        // Clamp posición de la cámara dentro de los bordes del fondo
        float clampedX = Mathf.Clamp(targetPos.x, minX, maxX);
        float clampedY = Mathf.Clamp(targetPos.y, minY, maxY);

        transform.position = new Vector3(clampedX, clampedY, offset.z);
    }
}
