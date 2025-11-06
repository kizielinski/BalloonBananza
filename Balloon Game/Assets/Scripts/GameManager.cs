using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject startText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Player player;

    bool gameStarted = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Game Loaded");

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
                SceneManager.LoadScene("Game");
            }
        }
    }
}
