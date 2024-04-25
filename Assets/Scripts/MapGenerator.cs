using System.Collections.Generic;
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

        foreach (PlanetParameter parameter in _planetParameters) 
        {
            Vector3 planetPosition = GeneratePlanetPosition(parameter);

            int indexPlanetModel = Random.Range(0, _rules.PlanetPrefabs.Length);

            GameObject planetGameObject = _rules.PlanetPrefabs[indexPlanetModel];

            Planet planet = planetBuilder.Model(planetGameObject)
                .Position(planetPosition)
                .Rotation(planetGameObject.transform.rotation)
                .Size(parameter.Size)
                .ShipForCaptured(Random.Range(parameter.MinShipToCaptured, parameter.MaxShipToCaptured))
                .ResourceProductionPerSecond(parameter.ResourcePerSecond)
                .MaxShip(parameter.MaxShip)
                .ShipPrefab(_rules.ShipPrefab)
                .Build();

            _planets.Add(planet);
        }

        return _planets;
    }

    private Vector3 GeneratePlanetPosition(PlanetParameter parameter)
    {
        bool generaåtionSuccessful;

        Vector3 planetPosition = Vector3.zero;

        do
        {
            //TODO: Ğåøèòü ïğîáëåìó ñ ïîïàäàíèåì â áåñêîíå÷íûé öèêë

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
        }
        while (!generaåtionSuccessful);

        return planetPosition;
    }
}
