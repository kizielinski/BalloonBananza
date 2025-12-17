using UnityEngine;

public class CactusDetection : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public bool detect {get; set; } = false;

    public static CactusDetection instanceCD {get; private set;}

    void Awake()
    {
        if(instanceCD == null)
        {
            instanceCD = this;
        }
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            detect = true;
            Debug.Log($"Trigger = {IsInvoking(collision.name)}");
        }
        
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        detect = false;
    }
}
    
