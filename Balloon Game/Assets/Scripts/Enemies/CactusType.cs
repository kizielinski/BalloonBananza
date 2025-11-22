using UnityEngine;

public class Cactus : MonoBehaviour
{
    // Shared movement components
    protected Rigidbody2D rbCactus;
    protected Vector2 center;

    protected virtual void Awake()
    {
        rbCactus = GetComponent<Rigidbody2D>();
        center = transform.position;
        
        // Initialize any additional base block functionality here
    }

    // Base block functionality - collision detection, destruction, etc.
    // Add block-specific logic here (like taking damage, scoring points, etc.)
}
    

