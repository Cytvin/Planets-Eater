using UnityEngine;

[CreateAssetMenu(fileName = "MapGeneratorRules", menuName = "Map/MapGeneratorRules")]
public class MapGeneratorRules : ScriptableObject
{
    public int MaxPlanet = 10;

    public int BigPlanetCount = 2;
    public int MiddlePlanetCount = 4;
    public int LittlePlanetCount = 4;

    public int BigPlanetSize = 8;
    public int MiddlePlanetSize = 5;
    public int LittlePlanetSize = 3;

    public int BigPlanetMaxShip = 75;
    public int MiddlePlanetMaxShip = 50;
    public int LittlePlanetMaxShip = 25;

    public int BigPlanetMinShipToCaptured = 30;
    public int BigPlanetMaxShipToCaptured = 50;
    public int MiddlePlanetMinShipToCaptured = 15;
    public int MiddlePlanetMaxShipToCaptured = 25;
    public int LittlePlanetMinShipToCaptured = 5;
    public int LittlePlanetMaxShipToCaptured = 10;

    public int BigPlanetResourcePerSecondProduction = 5;
    public int MiddlePlanetResourcePerSecondProduction = 3;
    public int LittlePlanetResourcePerSecondProduction = 1;

    public int MaxDistanceBetweenPlanet = 25;

    public float MapHeight = 0;
    public float MapWidth = 0;

    public GameObject[] PlanetPrefabs;
    public PlanetView PlanetView;
    public Ship ShipPrefab;
}
