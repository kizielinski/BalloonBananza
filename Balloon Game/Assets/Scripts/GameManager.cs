using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject startText;
    [SerializeField] private Player player;
    [SerializeField] private AirBarUI airBarUI;
    [SerializeField] private LivesUI livesUI;

    [SerializeField] public float minX = -1f;
    [SerializeField] public float maxX = 1f;
    [SerializeField] public float minY = -5f;
    [SerializeField] public float maxY = 5f;

    bool gameStarted = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Game Loaded");
        RemoveGameStartUI();
        ShowGameOverUI();

    }

    // Update is called once per frame
    void Update()
    {
        if (!gameStarted)
        {
            if (IsStartGameInputPressed())
            {
                StartGame();
            }
        }
        else
        {
            ContinueGame();
        }
    }

    void StartGame()
    {
        gameStarted = true;
        Debug.Log("Game Started");
        RemoveGameOverUI();
        ShowGameStartUI();
    }

    void EndGame()
    {
        gameStarted = false;
        Debug.Log("Player has died!");
        RemoveGameStartUI();
        ShowGameOverUI();
        RestartGame();
    }

    void ShowGameStartUI()
    {
        airBarUI.gameObject.SetActive(true);
        livesUI.gameObject.SetActive(true);
    }

    void RemoveGameStartUI()
    {
        airBarUI.gameObject.SetActive(false);
        livesUI.gameObject.SetActive(false);
    }

    void ShowGameOverUI()
    {
        startText.SetActive(true);
    }

    void RemoveGameOverUI()
    {
        startText.SetActive(false);
    }

    void RestartGame()
    {
        SceneManager.LoadScene("Game");
    }

    void ContinueGame()
    {
        if (player.GetAir() <= 0)
        {
            player.LoseLife();
            if (player.GetLives() <= 0)
            {
                EndGame();
                return;
            }

            player.RestoreAir(player.GetMaxAir());
            player.RespawnAtInitialSpawn();
        }

        updatePlayerUI();

    }

    void updatePlayerUI()
    {
        airBarUI.UpdateAirUI(player.GetAir(), player.GetMaxAir());
        livesUI.UpdateLivesUI(player.GetLives());
    }

    private bool IsStartGameInputPressed()
    {
        return Input.GetMouseButtonDown(0); // Left mouse button
    }
}
