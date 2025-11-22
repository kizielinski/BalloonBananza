using UnityEngine;

public class BlockType : MonoBehaviour
{
    [SerializeField] private bool canBeDestroyed = true;
    [SerializeField] private int healthModifier = 2; // Add this much health to base enemy health
    [SerializeField] private GameObject destroyEffect;

    private Enemy enemy;

    void Start()
    {
        enemy = GetComponent<Enemy>();

        // Modify enemy health for blocks
        if (enemy != null)
        {
            // Blocks are typically tougher - add extra health
            enemy.AddHealth(healthModifier);
            Debug.Log($"Block health set to: {enemy.GetHealth()}");
        }
    }

    public bool CanBeDestroyed() => canBeDestroyed;

    public void OnDestroyed()
    {
        if (destroyEffect != null)
        {
            Instantiate(destroyEffect, transform.position, Quaternion.identity);
        }
    }
}