using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlanetSelectorView : MonoBehaviour
{
    [SerializeField]
    private Button _shipDamageUpgradeButton;
    [SerializeField]
    private TextMeshProUGUI _shipDamageLevel;
    [SerializeField]
    private Button _productionSpeedUpgradeButton;
    [SerializeField]
    private TextMeshProUGUI _productionSpeedLevel;

    public void Enable()
    {
        gameObject.SetActive(true);
    }

    public void SetDamageUpgradeAction(UnityAction action)
    {
        _shipDamageUpgradeButton.onClick.AddListener(action);
    }

    public void SetProductionSpeedUpgradeAction(UnityAction action)
    {
        _productionSpeedUpgradeButton.onClick.AddListener(action);
    }

    public void SetShipDamageLevel(int level)
    {
        _shipDamageLevel.SetText(level.ToString());
    }

    public void SetProductionSpeedLevel(int level)
    {
        _productionSpeedLevel.SetText(level.ToString());
    }

    public void Disable()
    {
        gameObject.SetActive(false);
        _productionSpeedUpgradeButton.onClick.RemoveAllListeners();
        _shipDamageUpgradeButton.onClick.RemoveAllListeners();
    }
}
