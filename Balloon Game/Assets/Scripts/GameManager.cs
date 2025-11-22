using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject startText;
    [SerializeField] private TextMeshProUGUI airText;
    [SerializeField] private Player player;
    [SerializeField] private AirBarUI airBarUI;

    bool gameStarted = false;
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
        airText.gameObject.SetActive(true);
        airBarUI.gameObject.SetActive(true);
    }

    void RemoveGameStartUI()
    {
        airText.gameObject.SetActive(false);
        airBarUI.gameObject.SetActive(false);
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
        airText.text = "Air: " + player.GetAir();
        airBarUI.UpdateAirBar((float)player.GetAir() / player.GetMaxAir());
        if (player.GetAir() <= 0)
        {
            EndGame();
        }
    }

    private bool IsStartGameInputPressed()
    {
        return Input.GetMouseButtonDown(0); // Left mouse button
    }
}
