using UnityEngine;

public class Cactus : MonoBehaviour
{
<<<<<<< Updated upstream
    public Cactus cactus;
    public Rigidbody2D rbCactus;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
    {
        if(cactus == null)
        {
            cactus = this;
        }
    }
    
    void Start()
    {
        rbCactus = GetComponent<Rigidbody2D>(); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
=======
    // Shared movement components
    protected Rigidbody2D rbCactus;
    protected Vector2 center;

    protected virtual void Start()
    {
        rbCactus = GetComponent<Rigidbody2D>();
        center = transform.position;
        
        // Initialize any additional base block functionality here
    }

    // Base block functionality - collision detection, destruction, etc.
    // Add block-specific logic here (like taking damage, scoring points, etc.)
}
    

>>>>>>> Stashed changes
