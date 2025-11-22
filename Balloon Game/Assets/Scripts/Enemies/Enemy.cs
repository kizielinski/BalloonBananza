using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int health = 1;
    [SerializeField] private int damage = 1;

    private void Start()
    {
        // Enemy initialization
    }

    // Collision behavior
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (player == null)
            {
                Debug.LogWarning("Player component not found on collided object.");
                return;
            }
            if (player.IsShielded() == false)
            {
                player.TakeDamage(damage);
            }
        }
    }

    // Damage system
    public void TakeDamage(int damageAmount)
    {
        health -= damageAmount;
        if (health <= 0)
            Die();
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    // Public getters for other components
    public int GetHealth() => health;
    public int GetDamage() => damage;
    public void SetDamage(int newDamage) => damage = newDamage;
    public void SetHealth(int newHealth) => health = newHealth;
    public void AddHealth(int amount) => health += amount;
    public void DealDamage(int amount) { } //maybe needed later
}
