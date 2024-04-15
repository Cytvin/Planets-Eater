public class PlayerPresenter
{
    private PlayerView _view;
    private Player _player;

    public PlayerPresenter(PlayerView view, Player player)
    {
        _view = view;
        _player = player;

        _player.ResourceCountChanged += OnRecourceCountChanged;
    }

    private void OnRecourceCountChanged(float count)
    {
        _view.SetResourceCount(count);
    }
}