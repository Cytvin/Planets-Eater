using UnityEngine;

public class Player
{
    private Color _color;

    public Color Color => _color;

    public Player(Color color)
    {
        _color = color;
    }
}
