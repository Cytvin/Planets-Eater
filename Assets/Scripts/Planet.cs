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
    private int _shipCount;
    [SerializeField]
    private PlanetState _state;
    private List<Ship> _ships = new List<Ship>();
    private List<Ship> _enemyShips = new List<Ship>();
    private float _radius;
    private int _shipIndex = 0;
    private float _searchRadius = 7.5f;

    private float _timeFromLastShipProducted = 0;
    [SerializeField]
    private float _timeToNewShipProducted = 1f;

    public event Action<int> ShipsCountChanged;
    public Vector3 Coordinate => transform.position;
    public Player Owner => _owner;
    public int ShipCount => _shipCount;
    public float Radius => _radius;
    public PlanetState State => _state;

    public void Init(Player owner)
    {
        _owner = owner;
        Material material = GetComponent<MeshRenderer>().material;
        if (_owner != null)
        {
            material.color = _owner.Color;
            _state = PlanetState.Captured;
        }

        _radius = transform.localScale.x / 2;
    }

    private void Update()
    {
        if (_state != PlanetState.NotCaptured) 
        {
            SearchShips();
            ShipsCountChanged?.Invoke(_shipCount);

            if (_enemyShips.Count > 0)
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
            _timeFromLastShipProducted += Time.deltaTime;
            if (_timeFromLastShipProducted > _timeToNewShipProducted)
            {
                CreateShip();
                _timeFromLastShipProducted = 0;
            }
        }
        else if (_state == PlanetState.UnderSiege)
        {
            foreach (Ship ship in _ships.Where(s => s.State == Ship.ShipState.Holding))
            {
                ship.DefendPlanet();
            }
        }
    }

    public void SendAllShipsTo(Planet planet)
    {
        foreach (Ship ship in _ships.Where(s => s.State == Ship.ShipState.Holding))
        {
            ship.FlyToPlanet(planet);
        }
    }

    public void TakeForLanding(Ship ship)
    {
        _shipCount--;
        ShipsCountChanged?.Invoke(_shipCount);
        _ships.Remove(ship);

        if (_shipCount <= 0)
        {
            PlanetCaptured(ship.Owner);
            _shipCount = _ships.Count;
            ShipsCountChanged?.Invoke(_shipCount);
        }
    }

    public void SearchShips()
    {
        _shipCount = 0;
        _ships.Clear();
        _enemyShips.Clear();
        Collider[] searchResult = Physics.OverlapSphere(transform.position, _searchRadius);

        foreach (Collider collider in searchResult)
        {
            if (collider.TryGetComponent<Ship>(out Ship ship) && ship.TargetPlanet == this)
            {
                if (ship.Owner == _owner)
                {
                    _ships.Add(ship);
                    _shipCount++;
                }
                else
                {
                    _enemyShips.Add(ship);
                }
            }
        }
    }

    private void PlanetCaptured(Player owner)
    {
        _owner = owner;
        Material material = GetComponent<MeshRenderer>().material;
        material.color = owner.Color;
        _state = PlanetState.Captured;
    }

    private Ship CreateShip()
    {
        Ship ship = Instantiate(_shipPrefab, transform.position, _shipPrefab.transform.rotation);
        ship.Init(this, _owner);
        ship.gameObject.name = $"Ship {_shipIndex++} from {gameObject.name}";
        return ship;
    }
}