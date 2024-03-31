using TMPro;
using UnityEngine;

public class Main : MonoBehaviour
{
    [SerializeField]
    private GameObject _planetHolder;

    private void Awake()
    {
        Planet[] planets = _planetHolder.GetComponentsInChildren<Planet>();

        foreach (Planet planet in planets)
        {
            TextMeshProUGUI shipCountText = planet.GetComponentInChildren<TextMeshProUGUI>();
            PlanetView planetView = new PlanetView(shipCountText);
            PlanetPresenter planetPresenter = new PlanetPresenter(planet, planetView);
        }
    }
}
