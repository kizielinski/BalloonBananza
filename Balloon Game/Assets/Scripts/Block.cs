using UnityEngine;

public class Block : MonoBehaviour
{
    // Shared movement parameters
    [SerializeField] protected float speedScale = 2f;
    [SerializeField] protected float movementRadius = 3f;
    
    // Shared movement components
    protected Rigidbody2D rb;
    protected Vector2 center;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        center = transform.position;
        
        // Initialize any additional base block functionality here
    }
    
    // Base block functionality - collision detection, destruction, etc.
    // Add block-specific logic here (like taking damage, scoring points, etc.)
}
