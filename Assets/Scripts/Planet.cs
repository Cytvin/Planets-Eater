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
    private Color _freedomColor = Color.gray;
    [SerializeField]
    private Color _playerColor = Color.blue;
    [SerializeField]
    private Color _enemyColor = Color.red;
    [SerializeField]
    private Owner _owner;
    private List<Ship> _ships = new List<Ship>();
    [SerializeField]
    private int _shipCount;

    public event Action<int> ShipsCountChanged;
    public Vector3 Coordinate => transform.position;
    public Owner Owner => _owner;
    public int ShipCount => _shipCount;


    private void Awake()
    {
        Material material = GetComponent<MeshRenderer>().material;
        if (_owner == Owner.None)
        {
            material.color = _freedomColor;
        }
        else if (_owner == Owner.Player)
        {
            material.color = _playerColor;
            StartCoroutine(CreatingShips());
        }
        else
        {
            material.color = _enemyColor;
            StartCoroutine(CreatingShips());
        }
    }

    public void SendAllShipsTo(Planet planet)
    {
        foreach (Ship ship in _ships.Where(s => s.State == Ship.ShipState.Holding))
        {
            ship.FlyToPlanet(planet);
        }
    }

    private void PlanetCaptured(Owner owner)
    {
        _owner = owner;
        Material material = GetComponent<MeshRenderer>().material;
        material.color = _playerColor;
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
            else if (_owner == Owner.None)
            {
                _ships.Add(ship);
                _shipCount--;
                ShipsCountChanged?.Invoke(_shipCount);

                if (_shipCount <= 0)
                {
                    PlanetCaptured(ship.Owner);
                    _shipCount = _ships.Count;
                    ShipsCountChanged?.Invoke(_shipCount);
                }
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