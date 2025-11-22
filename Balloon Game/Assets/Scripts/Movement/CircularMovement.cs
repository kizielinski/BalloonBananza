using UnityEngine;

public class CircularMovement : MonoBehaviour
{
    [SerializeField] private float speedScale = 2f;
    [SerializeField] private float movementRadius = 3f;

    private Rigidbody2D rb;
    private Vector2 center;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        center = transform.position;
    }

    void Update()
    {
        float movementX = center.x + Mathf.Cos(Time.time * speedScale) * movementRadius;
        float movementY = center.y + Mathf.Sin(Time.time * speedScale) * movementRadius;
        rb.MovePosition(new Vector2(movementX, movementY));
    }
}