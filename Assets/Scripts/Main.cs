using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;

public class Main : MonoBehaviour
{
    [SerializeField]
    private MapGeneratorRules _mapGeneratorRules;
    [SerializeField]
    private NavMeshSurface _map;
    [SerializeField]
    private Collider _mapBoundary;
    [SerializeField]
    private Transform _planetHolder;
    [SerializeField]
    private PlayerView _playerView;
    [SerializeField]
    private PlanetSelector _planetSelector;
    [SerializeField]
    private PlanetSelectorView _planetSelectorView;


    private void Awake()
    {
        _mapGeneratorRules.MapHeight = (_mapBoundary.transform.localScale.z / 2) * 10;
        _mapGeneratorRules.MapWidth = (_mapBoundary.transform.localScale.x / 2) * 10;

        MapGenerator generator = new MapGenerator(_mapGeneratorRules, _planetHolder);
        List<Planet> planets = generator.GenerateMap().ToList();

        _map.BuildNavMesh();

        Player player1 = new Player(Color.blue);
        Player player2 = new Player(Color.red);

        PlayerPresenter playerPresenter = new PlayerPresenter(_playerView, player1);
        PlanetSelectorPresenter planetSelectorPresenter = new PlanetSelectorPresenter(_planetSelectorView, _planetSelector);

        PlacePlayers(planets, player1, player2);
    }

    private void PlacePlayers(List<Planet> planets, Player one, Player two)
    {
        Planet left = planets[0];
        Planet right = planets[1];

        foreach(Planet planet in planets) 
        {
            if (planet.Position.x < left.Position.x)
            {
                left = planet;
            }

            if (planet.Position.x > right.Position.x) 
            {
                right = planet;
            }
        }

        left.ChangeOwner(one);
        right.ChangeOwner(two);
    }
}
