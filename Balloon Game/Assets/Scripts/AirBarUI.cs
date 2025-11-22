using UnityEngine;

public class AirBarUI : MonoBehaviour
{
    [SerializeField] private Transform airBar;

    public void UpdateAirBar(float airPercent)
    {
        airBar.localScale = new Vector3(airPercent, 1f, 1f);
    }
}