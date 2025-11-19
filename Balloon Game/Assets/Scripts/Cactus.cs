using UnityEngine;

public class Cactus : MonoBehaviour
{
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
