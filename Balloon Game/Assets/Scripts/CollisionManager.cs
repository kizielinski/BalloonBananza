using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
      // A list to store all hits in the current frame
    private List<Collider> currentHits = new List<Collider>();
    private bool gotHitThisFrame = false;

    void OnTriggerEnter(Collider other)
    {
        // Add the hit collider to the list
        currentHits.Add(other);
        gotHitThisFrame = true;
    }

    void LateUpdate()
    {
        if (gotHitThisFrame)
        {
            // Sort the hits by their priority value (e.g., lowest value has priority)
            var priorityHit = currentHits.OrderBy(hit => hit.GetComponent<CollisionPriority>().priorityValue).FirstOrDefault();

            if (priorityHit != null)
            {
                // Process only the highest priority hit
                Debug.Log("Processed collision with the highest priority object: " + priorityHit.name);
                // Add your specific action logic here
            }

            // Reset for the next frame
            currentHits.Clear();
            gotHitThisFrame = false;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
