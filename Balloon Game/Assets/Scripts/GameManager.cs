using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Android.Gradle;
using UnityEditor.Callbacks;
using System.Numerics;
using Unity.Mathematics;
using System;
using System.Reflection;


public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject startText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Player player;

    float delay = 1;

    Player player1;

    public GameObject Spike;
    private UnityEngine.Vector2 spawnPos = Cactus.instanceC.posCatcus;

    public float spawnRate;

    bool gameStarted = false;

    float rotate = 0;

    public int spikeSpeed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Game Loaded");
        player1 = player.player;

    }

    // Update is called once per frame
    void Update()
    {
        if (!gameStarted)
        {
            startText.SetActive(true);
            healthText.gameObject.SetActive(false);
            if (Input.GetMouseButtonDown(0))
            {
                // StartSpawning();
                gameStarted = true;
                Debug.Log("Game Started");
                startText.SetActive(false);
                healthText.gameObject.SetActive(true);
            }
        }
        else
        {
            healthText.text = "Health: " + player.GetHealth();
            if (player.GetHealth() <= 0)
            {
                Debug.Log("Player has died!");
                Destroy(gameObject);
                SceneManager.LoadScene("ErickGameScene");
            }
        }
        if(CactusDetection.instanceCD.detect)
        {
            StartSpawning();
        }
        else
        {
            CancelWithDelay(delay);
        }
        //Debug.Log($"Detection = {CactusDetection.instanceCD.detect} Invoking = {IsInvoking(nameof(SpawnSpike))}");
    }

     private void StartSpawning()
    {
    
        if(IsInvoking(nameof(SpawnSpike)))
        {
        
        }
        else
        {
            InvokeRepeating("SpawnSpike", 1.0f, spawnRate);
        }

        // if(IsInvoking(nameof(SpawnSpike)))
        // {
        // CancelInvoke("SpawnSpike"); 
        // }
        // else
        // {
        // InvokeRepeating("SpawnSpike", 1.0f, spawnRate);
        // }
    }

    private void StopSpawning()
    {
        if(IsInvoking(nameof(SpawnSpike)))
        {
           CancelInvoke("SpawnSpike"); 
        }
    }
    private void SpawnSpike()
    {
        int objectCount = 8;
        
        Debug.Log($"Spawn Pos{spawnPos}");

        for (int i = 0; i < objectCount; i++)
        {
            spawnPos = Cactus.instanceC.posCatcus;
            float radius = 1.0f;
            float theta = i * 2 * Mathf.PI / objectCount;
            float x = Mathf.Sin(theta) * radius;
            float y = Mathf.Cos(theta) * radius;
            // spawnPos = new UnityEngine.Vector2(x,y);
            spawnPos.x += x;
            spawnPos.y += y;
            GameObject s1 = Instantiate(Spike, spawnPos, UnityEngine.Quaternion.Euler(0, 0, rotate));
            Rigidbody2D rbTemp = s1.GetComponent<Rigidbody2D>();
            // Physics2D.IgnoreCollision(rbTemp.GetComponent<Collider2D>(), spawnPoint.GetComponent<Collider2D>());
            rbTemp.AddForce(new UnityEngine.Vector2(x, y).normalized * spikeSpeed);
            rotate -= 360 / objectCount;
                           
        }
        spikeSpeed+=10;
    }

public void CancelWithDelay(float delay)
{
StartCoroutine(BuildAfterDelay(delay));
}

private System.Collections.IEnumerator BuildAfterDelay(float delay)
{
yield return new WaitForSeconds(delay);
StopSpawning();
}



}
