public class PlanetPresenter
{
    private Planet _planet;
    private PlanetView _view;

    public PlanetPresenter(Planet planet, PlanetView planetView)
    {
        _planet = planet;
        _view = planetView;
        if (_planet.State == Planet.PlanetState.NotCaptured)
        {
            _view.SetText(planet.ShipToCaptured.ToString());
        }
        else
        {
            _view.SetText(planet.ShipCount.ToString());
        }

        _planet.ShipsCountChanged += OnShipCountChanged;
    }

    private void OnShipCountChanged(int shipsCount)
    {
        _view.SetText(shipsCount.ToString());
    }
}
