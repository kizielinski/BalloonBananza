using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject startText;
    [SerializeField] private TextMeshProUGUI airText;
    [SerializeField] private Player player;
    [SerializeField] private AirBarUI airBarUI;

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
    void Start()
    {
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
        RemoveGameOverUI();
        ShowGameStartUI();
    }

    void EndGame()
    {
        gameStarted = false;
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
