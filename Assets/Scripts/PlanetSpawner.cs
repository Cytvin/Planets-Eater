using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class PlanetSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject _planetsPrefab;
    [SerializeField]
    private PlanetConnector _planetConnectorPrefab;
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
    private List<PlanetConnector> _connectors = new List<PlanetConnector>();

    public void Start()
    {
        _deltaO = (2 * Mathf.PI) / _planetCount;
        SpawnPlanet();
        ConnectPlanets();
    }

    public void SpawnPlanet()
    {
        for (int i = 0; i < _planetCount; i++)
        {
            Vector3 planetPosition = GeneratePlanetCoordinate();

            Planet newPlanet = Instantiate(_planetsPrefab, planetPosition, Quaternion.identity).GetComponent<Planet>();
            newPlanet.name = $"Planet {i + 1}";

            TextMeshProUGUI shipCountText = newPlanet.GetComponentInChildren<TextMeshProUGUI>();
            PlanetView planetView = new PlanetView(shipCountText);
            PlanetPresenter planetPresenter = new PlanetPresenter(newPlanet, planetView);

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

    private void ConnectPlanets()
    {
        foreach (Planet planet in _planets)
        {
            Collider[] nearPlanets = FindNearPlanet(planet, _maxConnection);

            foreach (Collider collider in nearPlanets) 
            {
                RaycastHit hit;
                Vector3 rayDirection = collider.transform.position - planet.Coordinate;

                Physics.Raycast(planet.Coordinate, rayDirection, out hit);

                if (hit.collider == collider && !collider.CompareTag("GalaxyCenter") && IntersectCheck(planet.Coordinate, collider.transform.position) == false)
                {
                    Planet otherPlanet = collider.gameObject.GetComponent<Planet>();

                    //if (!otherPlanet.IsConnect(planet))
                    //{
                    //    PlanetConnector connector = Instantiate(_planetConnectorPrefab, planet.transform);
                    //    connector.Connect(planet, otherPlanet);
                    //    _connectors.Add(connector);
                    //}  
                }
            }
        }
    }

    private bool IntersectCheck(Vector3 start, Vector3 end)
    {
        bool intersect = false;

        foreach(PlanetConnector connector in _connectors)
        {
            intersect = CheckBoundingBox(start.x, end.x, connector.Start.x, connector.End.x)
                    && CheckBoundingBox(start.z, end.z, connector.Start.z, connector.End.z)
                    && Area(start, end, connector.Start) * Area(start, end, connector.End) <= 0
                    && Area(connector.Start, connector.End, start) * Area(connector.Start, connector.End, end) <= 0;

            if (intersect)
            {
                return intersect;
            }
        }

        return intersect;
    }

    private bool CheckBoundingBox(float a, float b, float c, float d)
    {
        if (a > b)
        {
            (a, b) = (b, a);
        }
        if (c > d)
        {
            (c, d) = (d, c);
        }
        return Mathf.Max(a, c) <= Mathf.Min(b, d);
    }

    private float Area(Vector3 a, Vector3 b, Vector3 c)
    {
        return (b.x - a.x) * (c.z - a.z) - (b.z - a.z) * (c.x - a.x);
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
