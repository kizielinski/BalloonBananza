using UnityEngine;

public class SpikeType : MonoBehaviour
{
    [SerializeField] private int extraDamage = 1;
    [SerializeField] private GameObject spikeEffect;

    private Enemy enemy;

    void Start()
    {
        enemy = GetComponent<Enemy>();

        // Increase damage for spike enemies
        if (enemy != null)
        {
            enemy.SetDamage(enemy.GetDamage() + extraDamage);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && spikeEffect != null)
        {
            // Play spike-specific effect
            Instantiate(spikeEffect, transform.position, Quaternion.identity);
        }
    }
}