public class PlanetSelectorPresenter
{
    private PlanetSelectorView _view;
    private PlanetSelector _planetSelector;

    public PlanetSelectorPresenter(PlanetSelectorView view, PlanetSelector planetSelector)
    {
        _view = view;
        _planetSelector = planetSelector;

        _planetSelector.PlanetSelected += OnPlanetSelected;
        _planetSelector.PlanetDeselected += OnPlanetDeselected;
    }

    private void OnPlanetSelected(Planet planet)
    {
        _view.Enable();

        _view.SetShipUpgradeAction(planet.Factory.UpgradeShipDamage);
        _view.SetShipUpgradeInfo(planet.Factory.ShipDamageLevel, planet.Factory.ShipUpgradeCost);

        _view.SetProductionSpeedUpgradeAction(planet.Factory.UpgradeProductionSpeed);
        _view.SetProductionSpeedInfo(planet.Factory.ProdutionSpeedLevel, planet.Factory.ProductionUpgradeCost);

        planet.Factory.ShipDamageLevelChanged += OnShipDamageLevelChanged;
        planet.Factory.ProductionLevelChanged += OnProductionLevelChanged;
    }

    private void OnShipDamageLevelChanged(int level, float cost)
    {
        _view.SetShipUpgradeInfo(level, cost);
    }

    private void OnProductionLevelChanged(int level, float cost)
    {
        _view.SetProductionSpeedInfo(level, cost);
    }

    private void OnPlanetDeselected(Planet planet)
    {
        _view.Disable();
        planet.Factory.ShipDamageLevelChanged -= OnShipDamageLevelChanged;
        planet.Factory.ProductionLevelChanged -= OnProductionLevelChanged;
    }
}
