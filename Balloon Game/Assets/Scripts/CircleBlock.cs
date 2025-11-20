using UnityEngine;

public class CircleBlock : Block
{
    void Update()
    {
        float movementY = center.y + Mathf.Sin(Time.time * speedScale) * movementRadius;
        float movementX = center.x + Mathf.Cos(Time.time * speedScale) * movementRadius;
        rb.MovePosition(new Vector2(movementX, movementY));
    }
}