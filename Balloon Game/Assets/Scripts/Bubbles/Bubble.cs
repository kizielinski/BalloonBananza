using UnityEngine;

public abstract class Bubble : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                ApplyEffect(player);
                Destroy(gameObject);
            }
        }
    }

    // Each bubble type implements this differently
    protected abstract void ApplyEffect(Player player);
}
