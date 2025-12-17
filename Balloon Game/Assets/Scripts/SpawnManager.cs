using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log($"Detect = {CactusDectection.instanceCD.detect}");
        if(CactusDectection.instanceCD.detect)
        {
            CactusSpike.instanceCS.StartSpawning();
        }
        else
        {
            CactusSpike.instanceCS.StopSpawning();
        }
    }
}
