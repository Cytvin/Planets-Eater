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
    [SerializeField]
    private AI _ai;

    private void Awake()
    {
        _mapGeneratorRules.MapHeight = (_mapBoundary.transform.localScale.z / 2) * 10;
        _mapGeneratorRules.MapWidth = (_mapBoundary.transform.localScale.x / 2) * 10;

        MapGenerator generator = new MapGenerator(_mapGeneratorRules, _planetHolder);
        List<Planet> map = generator.GenerateMap().ToList();

        _map.BuildNavMesh();

        Player player = new Player(Color.blue);
        Player ai = new Player(Color.red);

        PlayerPresenter playerPresenter = new PlayerPresenter(_playerView, player);
        PlanetSelectorPresenter planetSelectorPresenter = new PlanetSelectorPresenter(_planetSelectorView, _planetSelector);

        _ai.Init(ai, map);

        PlacePlayers(map, player, ai);
        player.AddResource(1000000);
        ai.AddResource(1000000000);
    }

    private void PlacePlayers(List<Planet> planets, Player player, Player ai)
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

        left.ChangeOwner(player);
        right.ChangeOwner(ai);
    }
}
