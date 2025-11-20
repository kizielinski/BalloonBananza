using UnityEngine;

public class VerticalBlock : Block
{
    void Update()
    {
        float movement = center.y + Mathf.Sin(Time.time * speedScale) * movementRadius;
        rb.MovePosition(new Vector2(transform.position.x, movement));
    }
}