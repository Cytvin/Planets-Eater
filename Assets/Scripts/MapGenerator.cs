using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGenerator
{
    private class PlanetParameter
    {
        public int Size;
        public int MaxShip;
        public int MinShipToCaptured;
        public int MaxShipToCaptured;
        public int ResourcePerSecond;
        public float SearchRadius;
    }

    private MapGeneratorRules _rules;
    private Transform _planetHolder;

    private List<PlanetParameter> _planetParameters;
    private List<Planet> _planets;

    private int _maxGeneratePositionAttempts = 1000;
    private int _generatePositionAttemptsCount = 0;
    private int _maxGenerateMapAttempts = 10;
    private int _generateMapAttempts = 0;

    public MapGenerator(MapGeneratorRules rules, Transform planetHolder)
    {
        _planetParameters = new List<PlanetParameter>();
        _planets = new List<Planet>();

        _rules = rules;
        _planetHolder = planetHolder;
        InitParameter(rules);
    }

    private void InitParameter(MapGeneratorRules rules)
    {
        for (int i = 0; i < rules.LittlePlanetCount; i++)
        {
            PlanetParameter planetParameter = new PlanetParameter()
            {
                Size = rules.LittlePlanetSize,
                MaxShip = rules.LittlePlanetMaxShip,
                MinShipToCaptured = rules.LittlePlanetMinShipToCaptured,
                MaxShipToCaptured = rules.LittlePlanetMaxShipToCaptured,
                ResourcePerSecond = rules.LittlePlanetResourcePerSecondProduction,
                SearchRadius = (rules.LittlePlanetSize / 2f) * 3f
            };

            _planetParameters.Add(planetParameter);
        }

        for (int i = 0; i < rules.MiddlePlanetCount; i++)
        {
            PlanetParameter planetParameter = new PlanetParameter()
            {
                Size = rules.MiddlePlanetSize,
                MaxShip = rules.MiddlePlanetMaxShip,
                MinShipToCaptured = rules.MiddlePlanetMinShipToCaptured,
                MaxShipToCaptured = rules.MiddlePlanetMaxShipToCaptured,
                ResourcePerSecond = rules.MiddlePlanetResourcePerSecondProduction,
                SearchRadius = (rules.MiddlePlanetSize / 2f) * 3f
            };

            _planetParameters.Add(planetParameter);
        }

        for (int i = 0; i < rules.BigPlanetCount; i++)
        {
            PlanetParameter planetParameter = new PlanetParameter()
            {
                Size = rules.BigPlanetSize,
                MaxShip = rules.BigPlanetMaxShip,
                MinShipToCaptured = rules.BigPlanetMinShipToCaptured,
                MaxShipToCaptured = rules.BigPlanetMaxShipToCaptured,
                ResourcePerSecond = rules.BigPlanetResourcePerSecondProduction,
                SearchRadius = (rules.BigPlanetSize / 2f) * 3f
            };

            _planetParameters.Add(planetParameter);
        }
    }

    public IEnumerable<Planet> GenerateMap()
    {
        PlanetBuilder planetBuilder = new PlanetBuilder(_rules.PlanetView, _planetHolder);

        bool mapNotGenerated;

        do
        {
            mapNotGenerated = false;

            foreach (PlanetParameter parameter in _planetParameters)
            {
                Vector3 planetPosition = GeneratePlanetPosition(parameter);

                if (_generatePositionAttemptsCount > _maxGeneratePositionAttempts)
                {
                    break;
                }

                Planet planet = CreatePlanet(planetBuilder, parameter, planetPosition);

                _planets.Add(planet);
            }

            if (_planets.Count < _rules.MaxPlanet)
            {
                _generateMapAttempts++;

                if (_generateMapAttempts > _maxGenerateMapAttempts)
                {
                    throw new System.ArgumentException("Íå óäàëîñü ñãåíåðèðîâàòü êàðòó, ïðîâåðüòå ïðàâèëà ãåíåðàöèè");
                }

                mapNotGenerated = true;
            }

            if (mapNotGenerated)
            {
                ResetMap();
            }
        } while (mapNotGenerated);

        ActivatePlanets();

        MakeNeighbor(_planets);

        foreach (Planet planet in _planets)
        {
            foreach (KeyValuePair<Planet, float> keyValue in planet.Neighbors)
            {
                Debug.Log($"Planet {planet.name}: neighbor {keyValue.Key.name} distance {keyValue.Value}");
            }
        }

        return _planets;
    }

    private Vector3 GeneratePlanetPosition(PlanetParameter parameter)
    {
        _generatePositionAttemptsCount = 0;

        bool generaåtionSuccessful;

        Vector3 planetPosition = Vector3.zero;

        do
        {
            generaåtionSuccessful = true;

            float x = Random.Range(-_rules.MapWidth + parameter.SearchRadius, _rules.MapWidth - parameter.SearchRadius);
            float z = Random.Range(-_rules.MapHeight + parameter.SearchRadius, _rules.MapHeight - parameter.SearchRadius);
            float y = 2.5f;

            planetPosition = new Vector3(x, y, z);

            foreach (Planet planet in _planets)
            {
                float distance = Vector3.Distance(planetPosition, planet.Position);

                if (distance < parameter.SearchRadius + planet.SearchRadius)
                {
                    generaåtionSuccessful = false;
                    break;
                }
            }

            bool isInMaxDistance = false;

            if (_planets.Count == 0)
            {
                isInMaxDistance = true;
            }

            foreach (Planet planet in _planets)
            {
                float distance = Vector3.Distance(planetPosition, planet.Position);

                if (distance < _rules.MaxDistanceBetweenPlanet)
                {
                    isInMaxDistance = true;
                    break;
                }
            }

            if (!isInMaxDistance)
            {
                generaåtionSuccessful = false;
            }

            _generatePositionAttemptsCount++;

            if (_generatePositionAttemptsCount > _maxGeneratePositionAttempts)
            {
                break;
            }
        }
        while (!generaåtionSuccessful);

        return planetPosition;
    }

    private Planet CreatePlanet(PlanetBuilder builder, PlanetParameter parameter, Vector3 position)
    {
        int indexPlanetModel = Random.Range(0, _rules.PlanetPrefabs.Length);

        GameObject planetGameObject = _rules.PlanetPrefabs[indexPlanetModel];

        Planet planet = builder.Model(planetGameObject)
            .Position(position)
            .Rotation(planetGameObject.transform.rotation)
            .Size(parameter.Size)
            .ShipForCaptured(Random.Range(parameter.MinShipToCaptured, parameter.MaxShipToCaptured))
            .ResourceProductionPerSecond(parameter.ResourcePerSecond)
            .MaxShip(parameter.MaxShip)
            .ShipPrefab(_rules.ShipPrefab)
            .Build();

        planet.gameObject.SetActive(false);

        return planet;
    }

    private void MakeNeighbor(IEnumerable<Planet> planets)
    {
        foreach (Planet planet in planets)
        {
            foreach (Planet otherPlanet in planets.Where(p => p != planet))
            {
                float distance = Vector3.Distance(planet.Position, otherPlanet.Position);

                if (distance <= _rules.MaxDistanceBetweenPlanet)
                {
                    if (Physics.Linecast(planet.Position, otherPlanet.Position, out RaycastHit hit) &&
                        hit.collider.TryGetComponent<Planet>(out Planet hitPlanet))
                    {
                        if (otherPlanet == hitPlanet)
                        {
                            planet.AddNeighbor(otherPlanet, distance);
                        }
                        else
                        {
                            Debug.Log($"Hit another planet {hitPlanet.name} shoot from {planet.name} to {otherPlanet.name}");
                        }
                    }
                }
            }
        }
    }

    private void ActivatePlanets()
    {
        foreach (Planet planet in _planets)
        {
            planet.gameObject.SetActive(true);
        }
    }

    private void ResetMap()
    {
        foreach(Planet planet in _planets)
        {
            Object.Destroy(planet.gameObject);
        }

        _planets.Clear();
    }
}