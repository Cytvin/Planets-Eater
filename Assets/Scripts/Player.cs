using UnityEngine;

public class Player
{
    private Color _color;
    private float _resource;

    public event System.Action<float> ResourceCountChanged;
    public Color Color => _color;
    public float RecourceCount => _resource;

    public Player(Color color)
    {
        _color = color;
    }

    public void AddResource(float count)
    {
        if (count < 0) 
        {
            throw new System.ArgumentOutOfRangeException(nameof(count), "Количество должно быть больше 0");
        }

        _resource += count;
        ResourceCountChanged?.Invoke(_resource);
    }

    public bool TryPay(float cost)
    {
        if (cost < 0)
        {
            throw new System.ArgumentOutOfRangeException(nameof(cost), "Цена не может быть меньше нуля");
        }

        if (_resource < cost)
        {
            return false;
        }

        return true;
    }

    public void Pay(float cost)
    {
        if (cost < 0)
        {
            throw new System.ArgumentOutOfRangeException(nameof(cost), "Цена не может быть меньше нуля");
        }

        _resource -= cost;
    }
}