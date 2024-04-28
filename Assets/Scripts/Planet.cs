using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Planet : MonoBehaviour
{
    public enum PlanetState
    {
        NotCaptured,
        Captured,
        UnderSiege
    }

    [SerializeField]
    private Ship _shipPrefab;
    [SerializeField]
    private Player _owner;
    [SerializeField]
    private int _shipToCaptured;
    [SerializeField]
    private PlanetState _state;
    private int _maxShip = 50;
    private List<Ship> _ships = new List<Ship>();
    private List<Ship> _enemyShips = new List<Ship>();
    private float _radius;
    private int _shipIndex = 0;
    private float _searchRadius = 0;

    private int _ownerShipsCountNear;
    private int _enemyShipsCountNear;

    [SerializeField]
    private float _resourcePerSecond;

    #region Factory
    private int _productionSpeedLevel = 1;
    private float _productionTime = 0;
    [SerializeField]
    private float _productionDelay = 4f;
    [SerializeField]
    private float _productionDelayReduction = 0.25f;
    [SerializeField]
    private float _productionUpgradeCost = 500;
    [SerializeField]
    private float _productionUpgradeCostReduction = 500;
    public event Action<int, float> ProductionLevelChanged; 

    private int _shipDamageLevel = 1;
    private float _shipDamage = 1f;
    private float _shipDamageGain = 0.2f;
    [SerializeField]
    private float _shipUpgradeCost = 500;
    [SerializeField]
    private float _shipUpgradeCostReduction = 500;
    public event Action<int, float> ShipDamageLevelChanged;

    public int ProdutionSpeedLevel => _productionSpeedLevel;
    public int ShipDamageLevel => _shipDamageLevel;
    public float ProductionUpgradeCost => _productionUpgradeCost;
    public float ShipUpgradeCost => _shipUpgradeCost;
    #endregion

    public event Action<Planet> PlanetCaptured;
    public event Action<int> ShipsCountChanged;
    public int ShipToCaptured => _shipToCaptured;
    public Vector3 Position => transform.position;
    public Player Owner => _owner;
    public int ShipCount => _ownerShipsCountNear;
    public int MaxShip => _maxShip;
    public float Radius => _radius;
    public float SearchRadius => _searchRadius;
    public PlanetState State => _state;

    public int MaxShipToSend => _ships.Count(s => s.State == Ship.ShipState.Holding);
    public int MaxOwnerShipToReceive => _maxShip - _ships.Count;

    public void Init(Player owner, float resourcePerSecond, int maxShip)
    {
        _owner = owner;
        Material material = GetComponent<MeshRenderer>().material;
        material.color = _owner.Color;
        _state = PlanetState.Captured;

        _radius = transform.localScale.x / 2;
        _searchRadius = _radius * 3;
       
        _resourcePerSecond = resourcePerSecond;
        _maxShip = maxShip;
    }

    public void Init(int shipForCaptured, float resourcePerSecond, int maxShip, Ship shipPrefab)
    {
        _shipToCaptured = shipForCaptured;

        _radius = transform.localScale.x / 2;
        _searchRadius = _radius * 3;

        _resourcePerSecond = resourcePerSecond;
        _maxShip = maxShip;

        _shipPrefab = shipPrefab;

        ShipsCountChanged?.Invoke(_shipToCaptured);
    }

    private void Update()
    {
        if (_state != PlanetState.NotCaptured) 
        {
            _ownerShipsCountNear = CountNearestShips(_ships);
            _enemyShipsCountNear = CountNearestShips(_enemyShips);
            ShipsCountChanged?.Invoke(_ownerShipsCountNear);

            if (_enemyShipsCountNear > 0)
            {
                _state = PlanetState.UnderSiege;
            }
            else
            {
                _state = PlanetState.Captured;
            }
        }

        if (_state == PlanetState.Captured)
        {
            _productionTime += Time.deltaTime;
            if (_productionTime > _productionDelay && _ships.Count < _maxShip)
            {
                AddShip(CreateShip());
                _productionTime = 0;
            }

            float currentResourceCount = _resourcePerSecond * Time.deltaTime;
            _owner.AddResource(currentResourceCount);
        }
        else if (_state == PlanetState.UnderSiege)
        {
            foreach (Ship ship in _ships.Where(s => s.State == Ship.ShipState.Holding))
            {
                ship.DefendPlanet();
            }
        }
    }

    public int GetMaxEnemyShipToReceive(Player enemy)
    {
        return _maxShip - _enemyShips.Count(s => s.Owner == enemy);
    }

    public void SendShipsByAmount(Planet planet, int amount)
    {
        if (MaxShipToSend < amount)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Количество которое пытаются отправить, больче чем количество кораблей доступных к отправке");
        }

        List<Ship> shipsToSend = _ships.Where(s => s.State == Ship.ShipState.Holding).Take(amount).ToList();

        foreach (Ship ship in shipsToSend)
        {
            ship.FlyToPlanet(planet);
            RemoveShip(ship);
        }
    }

    public void TakeForLanding(Ship ship)
    {
        if (_state == PlanetState.NotCaptured)
        {
            _shipToCaptured--;
            ShipsCountChanged?.Invoke(_shipToCaptured);

            if (_shipToCaptured <= 0)
            {
                ChangeOwner(ship.Owner);
            }
            return;
        }

        if (_ownerShipsCountNear <= 0)
        {
            ChangeOwner(ship.Owner);
            ShipsCountChanged?.Invoke(_ships.Count);
        }
    }

    public void AddShip(Ship ship)
    {
        if (ship.Owner == _owner)
        {
            _ships.Add(ship);
        }
        else
        {
            _enemyShips.Add(ship);
        }

        ship.Dead += RemoveShip;
    }

    #region FactoryUpgrade
    public void UpgradeProductionSpeed()
    {
        if (!_owner.TryPay(_productionUpgradeCost))
        {
            return;
        }

        _owner.Pay(_productionUpgradeCost);
        _productionUpgradeCost += _productionUpgradeCostReduction;
        _productionSpeedLevel++;
        _productionDelay -= _productionDelayReduction;
        ProductionLevelChanged?.Invoke(_productionSpeedLevel, _productionUpgradeCost);
    }

    public void UpgradeShipDamage()
    {
        if (!_owner.TryPay(_shipUpgradeCost))
        {
            return;
        }

        _owner.Pay(_shipUpgradeCost);
        _shipUpgradeCost += _shipUpgradeCostReduction;
        _shipDamageLevel++;
        _shipDamage += _shipDamageGain;
        ShipDamageLevelChanged?.Invoke(_shipDamageLevel, _shipUpgradeCost);
    }
    #endregion

    private void RemoveShip(Ship ship)
    {
        if (ship.Owner == _owner)
        {
            _ships.Remove(ship);
        }
        else
        {
            _enemyShips.Remove(ship);
        }

        ship.Dead -= RemoveShip;
    }

    private int CountNearestShips(List<Ship> ships)
    {
        int nearestShips = 0;

        foreach (Ship ship in ships)
        {
            if (Vector3.Distance(transform.position, ship.transform.position) <= _searchRadius)
            {
                nearestShips++;
            }
        }

        return nearestShips;
    }

    public void ChangeOwner(Player owner)
    {
        PlanetCaptured?.Invoke(this);

        _shipToCaptured = 0;
        _owner = owner;
        _owner.AddPlanet(this);
        Material material = GetComponent<MeshRenderer>().material;
        material.color = owner.Color;
        _state = PlanetState.Captured;

        (_ships, _enemyShips) = (_enemyShips.Where(s => s.Owner == owner).ToList(), _ships);
    }

    private Ship CreateShip()
    {
        Ship ship = Instantiate(_shipPrefab, transform.position, _shipPrefab.transform.rotation);
        ship.Init(this, _owner, _shipDamage);
        ship.gameObject.name = $"Ship {_shipIndex++} from {gameObject.name}";
        _owner.AddShip(ship);
        return ship;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _searchRadius);
    }
}