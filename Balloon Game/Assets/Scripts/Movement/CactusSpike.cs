using UnityEngine;
using UnityEngine.Rendering;

public class CactusSpike : Cactus
{
    [SerializeField] private int spikeSpeed;
    [SerializeField] private int objectCount = 8;
    [SerializeField] private float spawnRate;
    [SerializeField] GameObject Spike;
    public static CactusSpike instanceCS {get; private set;}
    private float rotate = 0;


    // Update is called once per frame

    void Start()
    {
        if(instanceCS == null)
        {
            instanceCS = this;
        }
    }
    void Update()
    {

    }
    public void StartSpawning()
    {
        if(IsInvoking(nameof(SpawnSpike)))
        {

        }
        else
        {
            InvokeRepeating(nameof(SpawnSpike), 1.0f, 1);
        }
    }

    public void StopSpawning()
    {
        if(IsInvoking(nameof(SpawnSpike)))
        {
           CancelInvoke(nameof(SpawnSpike)); 
        }
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
            GameObject s1 = Instantiate(Spike, center, Quaternion.Euler(0, 0, rotate));
            Rigidbody2D rbTemp = s1.GetComponent<Rigidbody2D>();
            rbTemp.AddForce(new Vector2(x, y).normalized * spikeSpeed);
            rotate -= 360 / objectCount;
            center = rbCactus.transform.position;               
        }
    }
}
