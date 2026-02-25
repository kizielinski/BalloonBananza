using TMPro;
using UnityEngine;

public class LivesUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI livesText;

    public void UpdateLivesUI(int lives)
    {
        livesText.text = $"Lives: {lives}";
    }
}
