using UnityEngine;

public class Cactus : MonoBehaviour
{
    
    public static Cactus instanceC {get; private set;}
    Rigidbody2D rbCactus;

    public Vector2 posCatcus {get; set;}
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if(instanceC == null)
        {
            instanceC = this;
        }
        
    }
    
    void Start()
    {
        rbCactus = GetComponent<Rigidbody2D>();
        posCatcus = rbCactus.position; 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
