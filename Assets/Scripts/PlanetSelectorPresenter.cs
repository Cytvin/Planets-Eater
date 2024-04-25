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

        _view.SetShipUpgradeAction(planet.UpgradeShipDamage);
        _view.SetShipUpgradeInfo(planet.ShipDamageLevel, planet.ShipUpgradeCost);

        _view.SetProductionSpeedUpgradeAction(planet.UpgradeProductionSpeed);
        _view.SetProductionSpeedInfo(planet.ProdutionSpeedLevel, planet.ProductionUpgradeCost);

        planet.ShipDamageLevelChanged += OnShipDamageLevelChanged;
        planet.ProductionLevelChanged += OnProductionLevelChanged;
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
        planet.ShipDamageLevelChanged -= OnShipDamageLevelChanged;
        planet.ProductionLevelChanged -= OnProductionLevelChanged;
    }
}
