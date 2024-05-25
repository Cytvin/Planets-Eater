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
    private Player _owner;
    [SerializeField]
    private int _shipToCaptured;
    [SerializeField]
    private PlanetState _state;
    private int _maxShipCount = 50;
    private List<Ship> _ownerShips = new List<Ship>();
    private List<Ship> _enemyShips = new List<Ship>();
    private ShipFactory _factory;
    private float _radius;
    private float _searchRadius = 0;

    private Dictionary<Planet, float> _neighbors = new Dictionary<Planet, float>();

    private int _ownerShipsNearCount;
    private int _enemyShipsNearCount;

    [SerializeField]
    private float _resourcePerSecond;

    public event Action<Planet> PlanetCaptured;
    public event Action<int> ShipsCountChanged;
    public int ShipToCaptured => _shipToCaptured;
    public Vector3 Position => transform.position;
    public Player Owner => _owner;
    public int ShipCount => _ownerShipsNearCount;
    public int MaxShipCount => _maxShipCount;
    public float Radius => _radius;
    public float SearchRadius => _searchRadius;
    public PlanetState State => _state;
    public ShipFactory Factory => _factory;
    public int MaxShipToSend => _ownerShips.Count(s => s.IsHolding);
    public IReadOnlyDictionary<Planet, float> Neighbors => _neighbors;

    public void Init(int shipForCaptured, float resourcePerSecond, int maxShip)
    {
        _shipToCaptured = shipForCaptured;

        _radius = transform.localScale.x / 2;
        _searchRadius = _radius * 3;

        _resourcePerSecond = resourcePerSecond;
        _maxShipCount = maxShip;

        ShipsCountChanged?.Invoke(_shipToCaptured);
        _factory = new ShipFactory(this);
    }

    private void Update()
    {
        if (_state != PlanetState.NotCaptured) 
        {
            _ownerShipsNearCount = CountNearestShips(_ownerShips);
            _enemyShipsNearCount = CountNearestShips(_enemyShips);
            ShipsCountChanged?.Invoke(_ownerShipsNearCount);

            if (_enemyShipsNearCount > 0)
            {
                _state = PlanetState.UnderSiege;
            }
            else
            {
                _state = PlanetState.Captured;
            }

            if (_state == PlanetState.Captured)
            {
                _factory.IncrementProductionTime(Time.deltaTime);

                if (_factory.CanCreate && _ownerShips.Count < _maxShipCount)
                {
                    Ship ship = _factory.CreateShip(_owner.ShipPrefab);
                    AddShip(ship);
                }

                float currentResourceCount = _resourcePerSecond * Time.deltaTime;
                _owner.AddResource(currentResourceCount);
            }
        }
    }

    public int GetMaxShipToReceiveByPlayer(Player player)
    {
        if (_owner == player)
        {
            return _maxShipCount - _ownerShips.Count;
        }

        return _maxShipCount - _enemyShips.Count(s => s.Owner == player);
    }

    public void SendShipsByAmount(Planet planet, int amount)
    {
        if (MaxShipToSend < amount)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Количество которое пытаются отправить, больче чем количество кораблей доступных к отправке");
        }

        List<Ship> shipsToSend = _ownerShips.Where(s => s.IsHolding).Take(amount).ToList();

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

        if (_ownerShipsNearCount <= 0)
        {
            ChangeOwner(ship.Owner);
            ShipsCountChanged?.Invoke(_ownerShips.Count);
        }
    }

    public void AddShip(Ship ship)
    {
        if (ship.Owner == _owner)
        {
            _ownerShips.Add(ship);
        }
        else
        {
            _enemyShips.Add(ship);
        }

        ship.Dead += RemoveShip;
    }

    public void AddNeighbor(Planet neighbor, float distance)
    {
        _neighbors.Add(neighbor, distance);

        _neighbors = _neighbors.OrderBy(kv => kv.Value).ToDictionary(kv => kv.Key, kv => kv.Value);
    }

    public bool IsNeighbor(Planet planet)
    {
        if (_neighbors.ContainsKey(planet))
        {
            return true;
        }

        return false;
    }

    public float GetDistanceToNeighbor(Planet neighbor)
    {
        if (!_neighbors.ContainsKey(neighbor))
        {
            throw new ArgumentOutOfRangeException(nameof(neighbor), "Нет такого соседа");
        }

        return _neighbors[neighbor];
    }

    private void RemoveShip(Ship ship)
    {
        if (ship.Owner == _owner)
        {
            _ownerShips.Remove(ship);
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

        (_ownerShips, _enemyShips) = (_enemyShips.Where(s => s.Owner == owner).ToList(), _ownerShips);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _searchRadius);
    }
}