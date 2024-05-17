using System;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    private Color _color;
    private float _resource;
    private List<Planet> _planets;
    private List<Ship> _ships;

    public event Action<int> PlanetCountChanged;
    public event Action<int> ShipCountChanged;
    public event Action<float> ResourceCountChanged;
    
    public Color Color => _color;
    public float RecourceCount => _resource;
    public IEnumerable<Planet> Planets => _planets;

    public Player(Color color)
    {
        _planets = new List<Planet>();
        _ships = new List<Ship>();
        _color = color;
    }

    public void AddPlanet(Planet planet)
    {
        planet.PlanetCaptured += OnPlanetRecaptured;
        _planets.Add(planet);
        PlanetCountChanged?.Invoke(_planets.Count);
    }

    public void AddShip(Ship ship)
    {
        ship.Dead += OnShipDead;
        _ships.Add(ship);
        ShipCountChanged?.Invoke(_ships.Count);
    }

    public void AddResource(float count)
    {
        if (count < 0) 
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Количество должно быть больше 0");
        }

        _resource += count;
        ResourceCountChanged?.Invoke(_resource);
    }

    public bool TryPay(float cost)
    {
        if (cost < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(cost), "Цена не может быть меньше нуля");
        }

        if (_resource < cost)
        {
            return false;
        }

        return true;
    }

    public void Pay(float cost)
    {
        if (cost < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(cost), "Цена не может быть меньше нуля");
        }

        _resource -= cost;
    }

    private void OnShipDead(Ship ship)
    {
        ship.Dead -= OnShipDead;
        _ships.Remove(ship);
        ShipCountChanged?.Invoke(_ships.Count);
    }

    private void OnPlanetRecaptured(Planet planet)
    {
        planet.PlanetCaptured -= OnPlanetRecaptured;
        _planets.Remove(planet);
        PlanetCountChanged?.Invoke(_planets.Count);
    }
}