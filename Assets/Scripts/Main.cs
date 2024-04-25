using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.AI.Navigation;
using UnityEngine;

public class Main : MonoBehaviour
{
    //[SerializeField]
    //private Planet _player1Planet;
    //[SerializeField]
    //private Planet _player2Planet;
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
        //Player player2 = new Player(Color.red);
        //_player1Planet.Init(player1);
        //_player2Planet.Init(player2);

        planets[planets.Count - 1].PlanetCaptured(player1);

        PlayerPresenter playerPresenter = new PlayerPresenter(_playerView, player1);

        PlanetSelectorPresenter planetSelectorPresenter = new PlanetSelectorPresenter(_planetSelectorView, _planetSelector);
    }
}
