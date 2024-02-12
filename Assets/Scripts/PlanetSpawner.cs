using System.Collections.Generic;
using UnityEngine;

public class PlanetSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject _planetsPrefab;
    [SerializeField]
    private int _planetCount;
    [SerializeField]
    private int _innerRadius;
    [SerializeField]
    private int _outerRadius;
    [SerializeField]
    private int _minDistance;
    [SerializeField]
    private int _maxDistanceToConnect;

    private Vector3 _ringCenter = new Vector3(0, 0, 0);
    private List<Planet> _planets = new List<Planet>();
    private Dictionary<Planet, List<Planet>> _map;

    public void Start()
    {
        SpawnPlanet();
        _map = CreateMap(_planets, _maxDistanceToConnect);
    }

    public void SpawnPlanet()
    {
        for(int i = 0; i < _planetCount; i++)
        {
            Vector3 planetPosition = GeneratePlanetCoordinate();

            Planet newPlanet = Instantiate(_planetsPrefab, planetPosition, Quaternion.identity).GetComponent<Planet>();
            _planets.Add(newPlanet);
        }
    }

    private Vector3 GeneratePlanetCoordinate()
    {
        Vector3 coordinate = new Vector3();
        bool isFarEnough;

        do
        {
            float angle = 2 * Mathf.PI * Random.Range(0.0f, 1.0f);
            float radius = Random.Range(_innerRadius, _outerRadius);

            Debug.Log($"Radius: {radius}, Angle: {angle}");

            float x = _ringCenter.x + (int)(radius * Mathf.Cos(angle));
            float z = _ringCenter.z + (int)(radius * Mathf.Sin(angle));

            coordinate.x = x;
            coordinate.y = 1;
            coordinate.z = z;
            isFarEnough = true;
            foreach(Planet planet in _planets)
            {
                float dx = planet.Coordinate.x - coordinate.x;
                float dy = planet.Coordinate.z - coordinate.z;
                if (Mathf.Sqrt(dx * dx + dy * dy) < _minDistance)
                {
                    isFarEnough = false;
                    break;
                }
            }
        }
        while (!isFarEnough);

        return coordinate;
    }

    public static Dictionary<Planet, List<Planet>> CreateMap(List<Planet> planets, int maxDistance)
    {
        Dictionary<Planet, List<Planet>> map = new Dictionary<Planet, List<Planet>>();

        for (int i = 0; i < planets.Count; i++)
        {
            for (int j = i + 1; j < planets.Count; j++)
            {
                float dx = planets[i].Coordinate.x - planets[j].Coordinate.x;
                float dz = planets[i].Coordinate.z - planets[j].Coordinate.z;
                double distance = Mathf.Sqrt(dx * dx + dz * dz);

                if (distance <= maxDistance)
                {
                    if (!map.ContainsKey(planets[i]))
                    {
                        map[planets[i]] = new List<Planet>();
                    }
                    if (!map.ContainsKey(planets[j]))
                    {
                        map[planets[j]] = new List<Planet>();
                    }

                    map[planets[i]].Add(planets[j]);
                    map[planets[j]].Add(planets[i]);
                }
            }
        }

        return map;
    }
}
