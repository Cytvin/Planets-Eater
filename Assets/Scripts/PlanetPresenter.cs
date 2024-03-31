public class PlanetPresenter
{
    private Planet _planet;
    private PlanetView _planetView;

    public PlanetPresenter(Planet planet, PlanetView planetView)
    {
        _planet = planet;
        _planetView = planetView;
        _planetView.SetText(planet.ShipCount.ToString());

        _planet.ShipsCountChanged += DisplayShipsCount;
    }

    private void DisplayShipsCount(int shipsCount)
    {
        _planetView.SetText(shipsCount.ToString());
    }
}
