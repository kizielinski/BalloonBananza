using UnityEngine;

public class ShieldBubble : Bubble
{
    [SerializeField] private float shieldDuration = 5f;

    protected override void ApplyEffect(Player player)
    {
        player.AddShield(shieldDuration);
    }
}