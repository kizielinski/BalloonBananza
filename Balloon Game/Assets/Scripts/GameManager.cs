using NUnit.Framework;
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
    }

    void EndGame()
    {
        gameStarted = false;
        Debug.Log("Player has died!");
        ShowGameOverUI();
        RestartGame();
    }

    void ShowGameOverUI()
    {
        startText.SetActive(true);
        healthText.gameObject.SetActive(false);
    }

    void RemoveGameOverUI()
    {
        startText.SetActive(false);
        healthText.gameObject.SetActive(true);
    }

    void RestartGame()
    {
        SceneManager.LoadScene("Game");
    }

    void ContinueGame()
    {
        healthText.text = "Health: " + player.GetHealth();
        if (player.GetHealth() <= 0)
        {
            EndGame();
        }
    }

    private bool IsStartGameInputPressed()
    {
        return Input.GetMouseButtonDown(0); // Left mouse button
    }
}
