using UnityEngine;

public class AirBubble : Bubble
{
    [SerializeField] private int airAmount = 20;

    protected override void ApplyEffect(Player player)
    {
        player.RestoreAir(airAmount);
    }
}