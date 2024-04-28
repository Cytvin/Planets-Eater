public class PlayerPresenter
{
    private PlayerView _view;
    private Player _player;

    public PlayerPresenter(PlayerView view, Player player)
    {
        _view = view;
        _player = player;

        _player.PlanetCountChanged += OnPlanetCountChanged;
        _player.ShipCountChanged += OnShipCountChanged;
        _player.ResourceCountChanged += OnRecourceCountChanged;
        _view.Enable();
    }

    private void OnPlanetCountChanged(int count)
    {
        _view.SetPlanetCount(count);
    }

    private void OnShipCountChanged(int count)
    {
        _view.SetShipCount(count);
    }

    private void OnRecourceCountChanged(float count)
    {
        _view.SetResourceCount(count);
    }
}