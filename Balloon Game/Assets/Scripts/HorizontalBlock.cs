using UnityEngine;

public class HorizontalBlock : Block
{
    void Update()
    {
        float movement = center.x + Mathf.Sin(Time.time * speedScale) * movementRadius;
        rb.MovePosition(new Vector2(movement, transform.position.y));
    }
}