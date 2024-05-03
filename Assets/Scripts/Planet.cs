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
    private int _maxShipCount = 50;
    private List<Ship> _ships = new List<Ship>();
    private List<Ship> _enemyShips = new List<Ship>();
    private ShipFactory _factory;
    private float _radius;
    private float _searchRadius = 0;

    private int _ownerShipsCountNear;
    private int _enemyShipsCountNear;

    [SerializeField]
    private float _resourcePerSecond;

    public event Action<Planet> PlanetCaptured;
    public event Action<int> ShipsCountChanged;
    public int ShipToCaptured => _shipToCaptured;
    public Vector3 Position => transform.position;
    public Player Owner => _owner;
    public int ShipCount => _ownerShipsCountNear;
    public int MaxShip => _maxShipCount;
    public float Radius => _radius;
    public float SearchRadius => _searchRadius;
    public PlanetState State => _state;
    public ShipFactory Factory => _factory;
    public int MaxShipToSend => _ships.Count(s => s.IsHolding);
    public int MaxOwnerShipToReceive => _maxShipCount - _ships.Count;

    public void Init(int shipForCaptured, float resourcePerSecond, int maxShip, Ship shipPrefab)
    {
        _shipToCaptured = shipForCaptured;

        _radius = transform.localScale.x / 2;
        _searchRadius = _radius * 3;

        _resourcePerSecond = resourcePerSecond;
        _maxShipCount = maxShip;

        _shipPrefab = shipPrefab;

        ShipsCountChanged?.Invoke(_shipToCaptured);
        _factory = new ShipFactory(this);
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
            _factory.IncrementProductionTime(Time.deltaTime);

            if (_factory.CanCreate && _ships.Count < _maxShipCount)
            {
                Ship ship = _factory.CreateShip(_shipPrefab);
                AddShip(ship);
            }

            float currentResourceCount = _resourcePerSecond * Time.deltaTime;
            _owner.AddResource(currentResourceCount);
        }
    }

    public int GetMaxEnemyShipToReceive(Player enemy)
    {
        return _maxShipCount - _enemyShips.Count(s => s.Owner == enemy);
    }

    public void SendShipsByAmount(Planet planet, int amount)
    {
        if (MaxShipToSend < amount)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Количество которое пытаются отправить, больче чем количество кораблей доступных к отправке");
        }

        List<Ship> shipsToSend = _ships.Where(s => s.IsHolding).Take(amount).ToList();

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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _searchRadius);
    }
}