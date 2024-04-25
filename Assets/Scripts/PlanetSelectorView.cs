using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlanetSelectorView : MonoBehaviour
{
    [SerializeField]
    private Button _shipUpgradeButton;
    [SerializeField]
    private TextMeshProUGUI _shipUpgradeLevel;
    [SerializeField]
    private TextMeshProUGUI _shipUpgradeCost;
    [SerializeField]
    private Button _productionSpeedUpgradeButton;
    [SerializeField]
    private TextMeshProUGUI _productionSpeedLevel;
    [SerializeField]
    private TextMeshProUGUI _productionUpgradeCost;

    public void Enable()
    {
        gameObject.SetActive(true);
    }

    public void SetShipUpgradeAction(UnityAction action)
    {
        _shipUpgradeButton.onClick.AddListener(action);
    }

    public void SetProductionSpeedUpgradeAction(UnityAction action)
    {
        _productionSpeedUpgradeButton.onClick.AddListener(action);
    }

    public void SetShipUpgradeInfo(int level, float cost)
    {
        _shipUpgradeLevel.SetText(level.ToString());
        _shipUpgradeCost.SetText(cost.ToString());
    }

    public void SetProductionSpeedInfo(int level, float cost)
    {
        _productionSpeedLevel.SetText(level.ToString());
        _productionUpgradeCost.SetText(cost.ToString());
    }

    public void Disable()
    {
        gameObject.SetActive(false);
        _productionSpeedUpgradeButton.onClick.RemoveAllListeners();
        _shipUpgradeButton.onClick.RemoveAllListeners();
    }
}
