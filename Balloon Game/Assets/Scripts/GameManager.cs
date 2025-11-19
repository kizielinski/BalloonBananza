using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Android.Gradle;
using UnityEditor.Callbacks;
using System.Numerics;
using Unity.Mathematics;


public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject startText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Player player;

    Player player1;

    UnityEngine.Vector2 playerPos;
    Cactus cactus1;
    public GameObject Spike;
    UnityEngine.Vector2 spawnPos;
    
    public Transform spawnPoint;
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
                StartSpawning();
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
        if(math.abs(player1.rb.position.x - spawnPoint.position.x) < 2 && math.abs(player1.rb.position.y - spawnPoint.position.y) < 2)
        {
            StartSpawning();
        }
        Debug.Log($"X:{math.abs(player1.rb.position.x - spawnPoint.position.x)}Y:{math.abs(player1.rb.position.y - spawnPoint.position.y)}");
    }

     private void StartSpawning()
    {   
        if(IsInvoking())
        {
        CancelInvoke("SpawnSpike"); 
        }
        else
        {
        InvokeRepeating("SpawnSpike", 1.0f, spawnRate);
        }
    }
    private void SpawnSpike()
    {
        spawnPos = spawnPoint.position;
        int objectCount = 8;
        
        Debug.Log($"Spawn Pos{spawnPos}");

        for (int i = 0; i < objectCount; i++)
        {
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
            spawnPos = spawnPoint.position;               
        }
        spikeSpeed+=10;
    }

}
