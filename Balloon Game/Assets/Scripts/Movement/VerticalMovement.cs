using UnityEngine;

public class VerticalMovement : MonoBehaviour
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
        float movement = center.y + Mathf.Sin(Time.time * speedScale) * movementRadius;
        rb.MovePosition(new Vector2(transform.position.x, movement));
    }
}