using UnityEngine;

public class SpeedBubble : Bubble
{
    [SerializeField] private float speedMultiplier = 2f;
    [SerializeField] private float duration = 5f;

    protected override void ApplyEffect(Player player)
    {
        player.ApplySpeedBoost(speedMultiplier, duration);
        Debug.Log($"Speed bubble: {speedMultiplier}x speed for {duration}s");
    }
}