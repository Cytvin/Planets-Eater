public class PlanetPresenter
{
    private Planet _planet;
    private PlanetView _planetView;

    public PlanetPresenter(Planet planet, PlanetView planetView)
    {
        _planet = planet;
        _planetView = planetView;
        if (_planet.State == Planet.PlanetState.NotCaptured)
        {
            _planetView.SetText(planet.ShipToCaptured.ToString());
        }
        else
        {
            _planetView.SetText(planet.ShipCount.ToString());
        }

        _planet.ShipsCountChanged += DisplayShipsCount;
    }

    private void DisplayShipsCount(int shipsCount)
    {
        _planetView.SetText(shipsCount.ToString());
    }
}
