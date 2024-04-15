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
        _view.SetDamageUpgradeAction(planet.UpgradeShipDamage);
        _view.SetShipDamageLevel(planet.ShipDamageLevel);
        _view.SetProductionSpeedUpgradeAction(planet.UpgradeFactorySpeed);
        _view.SetProductionSpeedLevel(planet.ProdutionSpeedLevel);

        planet.ShipDamageLevelChanged += OnShipDamageLevelChanged;
        planet.ProductionLevelChanged += OnProductionLevelChanged;
    }

    private void OnShipDamageLevelChanged(int level)
    {
        _view.SetShipDamageLevel(level);
    }

    private void OnProductionLevelChanged(int level)
    {
        _view.SetProductionSpeedLevel(level);
    }

    private void OnPlanetDeselected(Planet planet)
    {
        _view.Disable();
        planet.ShipDamageLevelChanged -= OnShipDamageLevelChanged;
        planet.ProductionLevelChanged -= OnProductionLevelChanged;
    }
}
