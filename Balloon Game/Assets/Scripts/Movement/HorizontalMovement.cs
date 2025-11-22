using UnityEngine;

public class HorizontalMovement : MonoBehaviour
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
        float movement = center.x + Mathf.Sin(Time.time * speedScale) * movementRadius;
        rb.MovePosition(new Vector2(movement, transform.position.y));
    }
}