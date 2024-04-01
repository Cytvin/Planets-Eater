using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Planet : MonoBehaviour
{
    [SerializeField]
    private Ship _shipPrefab;
    [SerializeField]
    private float _productionRate = 0.1f;
    [SerializeField]
    private Player _owner;
    [SerializeField]
    private int _shipCount;
    private List<Ship> _ships = new List<Ship>();
    private float _radius;
    private int _shipIndex = 0;

    public event Action<int> ShipsCountChanged;
    public Vector3 Coordinate => transform.position;
    public Player Owner => _owner;
    public int ShipCount => _shipCount;
    public float Radius => _radius;

    public void Init(Player owner)
    {
        _owner = owner;
        Material material = GetComponent<MeshRenderer>().material;
        if (_owner != null)
        {
            material.color = _owner.Color;
            StartCoroutine(CreatingShips());
        }

        _radius = transform.localScale.x / 2;
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

    private void PlanetCaptured(Player owner)
    {
        _owner = owner;
        Material material = GetComponent<MeshRenderer>().material;
        material.color = owner.Color;
        StartCoroutine(CreatingShips());
    }

    private IEnumerator CreatingShips()
    {
        float creatingProgress = 0;

        while (true)
        {
            yield return new WaitForSeconds(1);
            creatingProgress += _productionRate;

            if (creatingProgress >= 1)
            {
                InstantiateShip();
                creatingProgress = 0;
            }
        }
    }

    private Ship InstantiateShip()
    {
        Ship ship = Instantiate(_shipPrefab, transform.position, _shipPrefab.transform.rotation);
        ship.Init(this, _owner);
        ship.gameObject.name = $"Ship {_shipIndex++} from {gameObject.name}";
        return ship;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<Ship>(out Ship ship))
        {
            return;
        }

        if (ship.TargetPlanet == this)
        {
            if (ship.Owner == _owner)
            {
                _ships.Add(ship);
                _shipCount++;
                ShipsCountChanged?.Invoke(_shipCount);
            }
            else if (_owner == null)
            {
                _ships.Add(ship);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent<Ship>(out Ship ship))
        {
            return;
        }

        if (_ships.Contains(ship))
        {
            _ships.Remove(ship);
            _shipCount--;
            ShipsCountChanged?.Invoke(_shipCount);
        }
    }
}