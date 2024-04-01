using TMPro;
using UnityEngine;

public class Main : MonoBehaviour
{
    [SerializeField]
    private GameObject _planetHolder;
    [SerializeField]
    private Planet _player1Planet;
    [SerializeField]
    private Planet _player2Planet;

    private void Awake()
    {
        Planet[] planets = _planetHolder.GetComponentsInChildren<Planet>();

        foreach (Planet planet in planets)
        {
            TextMeshProUGUI shipCountText = planet.GetComponentInChildren<TextMeshProUGUI>();
            PlanetView planetView = new PlanetView(shipCountText);
            PlanetPresenter planetPresenter = new PlanetPresenter(planet, planetView);
            planet.Init(null);
        }

        Player player1 = new Player(Color.blue);
        Player player2 = new Player(Color.red);
        _player1Planet.Init(player1);
        _player2Planet.Init(player2);
    }
}
