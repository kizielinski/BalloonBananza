using UnityEngine;

public class CactusDectection : MonoBehaviour
{
public bool detect {get; set;} = false;


    public static CactusDectection instanceCD {get; private set;}

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
        // Debug.Log($"Detect = {detect}");

    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            detect = true;
            // Debug.Log($"Trigger = {IsInvoking(collision.name)}");
        }

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            detect = false;
            // Debug.Log($"Trigger = {IsInvoking(collision.name)}");
        }
    }
}
