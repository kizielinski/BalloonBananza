using UnityEngine;
using TMPro;

public class AirBarUI : MonoBehaviour
{
    [SerializeField] private Transform airBar;
    [SerializeField] private TextMeshProUGUI airText;

    public void UpdateAirUI(float currentAir, float maxAir)
    {
        float airPercent = maxAir > 0f ? currentAir / maxAir : 0f;
        airBar.localScale = new Vector3(airPercent, 1f, 1f);
        airText.text = "Air: " + currentAir;
    }

}