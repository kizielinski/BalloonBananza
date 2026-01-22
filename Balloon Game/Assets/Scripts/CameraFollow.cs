using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    void Start()
    {
        // Calculate the initial offset from the player to the camera
        offset = transform.position - player.position;
    }

    void LateUpdate()
    {
        if (player != null)
        {
            Vector3 desiredPosition = player.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

            smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, GameManager.Instance.minX, GameManager.Instance.maxX);
            smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, GameManager.Instance.minY, GameManager.Instance.maxY);

            transform.position = smoothedPosition;
        }
    }
}
