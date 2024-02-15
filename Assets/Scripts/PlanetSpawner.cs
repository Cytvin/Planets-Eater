using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlanetSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject _planetsPrefab;
    [SerializeField]
    private PlanetConnector _planetConnector;
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
    [SerializeField]
    private int _maxConnection;

    private float _deltaO;
    private float _currentO = 0;

    private List<Planet> _planets = new List<Planet>();
    private Dictionary<Planet, List<Planet>> _map;

    public void Start()
    {
        _deltaO = (2 * Mathf.PI) / _planetCount;
        SpawnPlanet();
        //_map = CreateMap(_planets, _maxDistanceToConnect);
        ConnectPlanets();
    }

    public void SpawnPlanet()
    {
        for (int i = 0; i < _planetCount; i++)
        {
            Vector3 planetPosition = GeneratePlanetCoordinate();

            Planet newPlanet = Instantiate(_planetsPrefab, planetPosition, Quaternion.identity).GetComponent<Planet>();
            newPlanet.name = $"Planet {i + 1}";
            _planets.Add(newPlanet);
        }
    }

    private Vector3 GeneratePlanetCoordinate()
    {
        Vector3 coordinate = new Vector3();

        bool isFarEnough = true;

        _currentO += _deltaO;
        int tryCount = 0;

        do
        {
            float radius = Random.Range(_innerRadius, _outerRadius);

            float x = radius * Mathf.Cos(_currentO);
            float z = radius * Mathf.Sin(_currentO);

            coordinate.x = x;
            coordinate.y = 1;
            coordinate.z = z;

            foreach(Planet planet in _planets)
            {
                float distance = Vector3.Distance(planet.Coordinate, coordinate);

                if (distance < _minDistance)
                {
                    isFarEnough = false;
                    break;
                }
                else
                {
                    isFarEnough = true;
                }
            }

            tryCount++;
        }
        while (isFarEnough == false && tryCount < 100);

        return coordinate;
    }

    public Dictionary<Planet, List<Planet>> CreateMap(List<Planet> planets, int maxDistance)
    {
        Dictionary<Planet, List<Planet>> map = new Dictionary<Planet, List<Planet>>();

        for (int i = 0; i < planets.Count; i++)
        {
            for (int j = i + 1; j < planets.Count; j++)
            {
                float dx = planets[j].Coordinate.x - planets[i].Coordinate.x;
                float dz = planets[j].Coordinate.z - planets[i].Coordinate.z;
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

                    PlanetConnector connector = Instantiate(_planetConnector, planets[i].transform);
                    connector.Init(planets[i].Coordinate, planets[j].Coordinate);

                    map[planets[i]].Add(planets[j]);
                    map[planets[j]].Add(planets[i]);
                }
            }
        }

        return map;
    }

    private void ConnectPlanets()
    {
        foreach (Planet planet in _planets)
        {
            Collider[] nearPlanets = FindNearPlanet(planet, _maxConnection);

            foreach (Collider collider in nearPlanets) 
            {
                if (planet.transform.childCount == _maxConnection)
                {
                    break;
                }

                RaycastHit hit;
                Vector3 rayDirection = collider.transform.position - planet.Coordinate;

                Physics.Raycast(planet.Coordinate, rayDirection, out hit);

                if (hit.collider == collider && !collider.CompareTag("GalaxyCenter"))
                {
                    PlanetConnector connector1 = Instantiate(_planetConnector, planet.transform);
                    connector1.Init(planet.Coordinate, collider.transform.position);

                    //planet.AddNearPlanet(collider.gameObject.GetComponent<Planet>());
                }
            }
        }
    }

    private Collider[] FindNearPlanet(Planet planet, int count)
    {
        Collider currentPlanetCollider = planet.GetComponent<Collider>();

        Collider[] nearPlanets = Physics.OverlapSphere(planet.Coordinate, _maxDistanceToConnect);

        List<Collider> planets = nearPlanets.ToList();
        planets.Remove(currentPlanetCollider);

        planets.Sort((a, b) => Vector3.Distance(planet.Coordinate, a.transform.position)
                .CompareTo(Vector3.Distance(planet.Coordinate, b.transform.position)));

        count = Mathf.Min(count, planets.Count);

        return planets.GetRange(0, count).ToArray();
    }
}
