using UnityEngine;

public class AirBubble : Bubble
{
    [SerializeField] private int airAmount = 3;

    protected override void ApplyEffect(Player player)
    {
        player.RestoreAir(airAmount);
        Debug.Log($"Health bubble: +{airAmount} health");
    }
}