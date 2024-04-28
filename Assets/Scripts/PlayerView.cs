using TMPro;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _planetCount;
    [SerializeField]
    private TextMeshProUGUI _shipCount;
    [SerializeField]
    private TextMeshProUGUI _resourceCount;

    public void Enable()
    {
        SetPlanetCount(0);
        SetShipCount(0);
        SetResourceCount(0);
    }

    public void SetPlanetCount(int planetCount)
    {
        _planetCount.SetText(planetCount.ToString());
    }

    public void SetShipCount(int shipCount) 
    {
        _shipCount.SetText(shipCount.ToString());
    }

    public void SetResourceCount(float count)
    {
        _resourceCount.SetText(Mathf.Round(count).ToString());
    }
}
