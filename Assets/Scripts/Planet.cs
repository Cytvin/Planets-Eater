using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    private List<Planet> _connectedPlanets = new List<Planet>();
    private List<PlanetConnector> _connectors = new List<PlanetConnector>();
    
    public Vector3 Coordinate => transform.position;

    public void AddNearPlanet(Planet planet)
    {
        _connectedPlanets.Add(planet);
    }

    public void Connect(Planet planet)
    {
        _connectedPlanets.Add(planet);
        //_connectors.Add(connector);
    }

    public void AddConnector(PlanetConnector connector)
    {
        _connectors.Add(connector);
    }

    public void Disconnect(Planet planet)
    {
        _connectedPlanets.Remove(planet);
    }

    public bool IsConnect(Planet planet)
    {
        return _connectedPlanets.Contains(planet);
    }

    private void Dead()
    {
        foreach(Planet planet in _connectedPlanets)
        {
            planet.Disconnect(this);
        }
    }

    private void OnMouseOver()
    {
        foreach (PlanetConnector connector in _connectors) 
        {
            connector.Select();
        }
    }

    private void OnMouseExit()
    {
        foreach (PlanetConnector connector in _connectors)
        {
            connector.UnSelect();
        }
    }
}
