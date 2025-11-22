using UnityEngine;
using UnityEngine.Rendering;

public class CactusSpike : Cactus
{
    [SerializeField] private int spikeSpeed;
    [SerializeField] private int objectCount = 8;
    [SerializeField] private float spawnRate;
    Spike Spike1;
    private float rotate = 0;


    // Update is called once per frame
    void Start()
    {
        StartSpawning();
    }
    void Update()
    {

    }
    private void StartSpawning()
    {
        InvokeRepeating("SpawnSpike", 2.0f, spawnRate);   
        
        // if(IsInvoking())
        // {
        
        // }
        // else
        // {
        // InvokeRepeating("SpawnSpike", 2.0f, spawnRate);
        // }
    }    
    private void SpawnSpike()
    {
        for (int i = 0; i < objectCount; i++)
        {
            float radius = 1.0f;
            float theta = i * 2 * Mathf.PI / objectCount;
            float x = Mathf.Sin(theta) * radius;
            float y = Mathf.Cos(theta) * radius;
            center.x += x;
            center.y += y;
            GameObject s1 = Instantiate(Spike1.gameSpike, center, UnityEngine.Quaternion.Euler(0, 0, rotate));
            Rigidbody2D rbTemp = s1.GetComponent<Rigidbody2D>();
            rbTemp.AddForce(new UnityEngine.Vector2(x, y).normalized * spikeSpeed);
            rotate -= 360 / objectCount;
            center = rbCactus.transform.position;               
        }
    }
}
