using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    private List<Planet> _nearPlanet;

    public Vector3 Coordinate => transform.position;

    private void Start()
    {
        _nearPlanet = new List<Planet>();
    }

    public void AddNearPlanet(Planet planet)
    {
        _nearPlanet.Add(planet);
    }

    private void OnMouseOver()
    {
        PlanetConnector[] connectors = GetComponentsInChildren<PlanetConnector>();

        foreach (PlanetConnector connector in connectors) 
        {
            connector.Select();
        }
    }

    private void OnMouseExit()
    {
        PlanetConnector[] connectors = GetComponentsInChildren<PlanetConnector>();

        foreach (PlanetConnector connector in connectors)
        {
            connector.UnSelect();
        }
    }
}
