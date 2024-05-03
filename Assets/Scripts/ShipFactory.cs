using UnityEngine;

public class ShipFactory
{
    private Planet _planet;
    private int _shipIndex = 0;

    private int _productionSpeedLevel = 1;
    private float _productionTime = 0;
    private float _productionDelay = 4f;
    private float _productionDelayReduction = 0.25f;
    private float _productionUpgradeCost = 500;
    private float _productionUpgradeCostReduction = 500;

    private int _shipDamageLevel = 1;
    private float _shipDamage = 1f;
    private float _shipDamageGain = 0.2f;
    private float _shipUpgradeCost = 500;
    private float _shipUpgradeCostReduction = 500;

    public event System.Action<int, float> ProductionLevelChanged;
    public event System.Action<int, float> ShipDamageLevelChanged;

    public int ProdutionSpeedLevel => _productionSpeedLevel;
    public int ShipDamageLevel => _shipDamageLevel;
    public float ProductionUpgradeCost => _productionUpgradeCost;
    public float ShipUpgradeCost => _shipUpgradeCost;
    public bool CanCreate => _productionTime >= _productionDelay;

    public ShipFactory(Planet planet)
    {
        _planet = planet;
    }

    public Ship CreateShip(Ship shipPrefab)
    {
        if (_productionTime < _productionDelay)
        {
            throw new System.ArgumentOutOfRangeException(nameof(_productionTime), "Текущее время производства не может быть меньше кулдауна");
        }

        Ship ship = Object.Instantiate(shipPrefab, _planet.Position, shipPrefab.transform.rotation);
        ship.Init(_planet, _planet.Owner, _shipDamage);
        ship.gameObject.name = $"Ship {_shipIndex++} from {_planet.gameObject.name}";
        _planet.Owner.AddShip(ship);

        _productionTime = 0;

        return ship;
    }

    public void IncrementProductionTime(float deltaTime)
    {
        if (deltaTime < 0)
        {
            throw new System.ArgumentOutOfRangeException(nameof(deltaTime), "Дельта времени не может быть меньше 0");
        }

        _productionTime += deltaTime;
    }

    public void UpgradeProductionSpeed()
    {
        if (!_planet.Owner.TryPay(_productionUpgradeCost))
        {
            return;
        }

        _planet.Owner.Pay(_productionUpgradeCost);
        _productionUpgradeCost += _productionUpgradeCostReduction;
        _productionSpeedLevel++;
        _productionDelay -= _productionDelayReduction;
        ProductionLevelChanged?.Invoke(_productionSpeedLevel, _productionUpgradeCost);
    }

    public void UpgradeShipDamage()
    {
        if (!_planet.Owner.TryPay(_shipUpgradeCost))
        {
            return;
        }

        _planet.Owner.Pay(_shipUpgradeCost);
        _shipUpgradeCost += _shipUpgradeCostReduction;
        _shipDamageLevel++;
        _shipDamage += _shipDamageGain;
        ShipDamageLevelChanged?.Invoke(_shipDamageLevel, _shipUpgradeCost);
    }
}
