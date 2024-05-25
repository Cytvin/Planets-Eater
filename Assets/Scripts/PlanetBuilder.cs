using UnityEngine;

public class PlanetBuilder
{
    private PlanetView _planetViewPrefab;
    private Transform _parent;
    private GameObject _planetModel;
    private Vector3 _position;
    private Quaternion _rotation;
    private int _size;
    private int _shipForCaptured;
    private float _resourceProductionPerSecond;
    private int _maxShip;

    public PlanetBuilder(PlanetView planetViewPrefab, Transform parent)
    {
        _planetViewPrefab = planetViewPrefab;
        _parent = parent;
    }

    public PlanetBuilder Model(GameObject model)
    {
        _planetModel = model;
        return this;
    }

    public PlanetBuilder Position(Vector3 position)
    {
        _position = position;
        return this;
    }

    public PlanetBuilder Rotation(Quaternion rotation) 
    {
        _rotation = rotation;
        return this;
    }

    public PlanetBuilder Size(int size)
    {
        _size = size;
        return this;
    }

    public PlanetBuilder ShipForCaptured(int shipForCaptured)
    {
        _shipForCaptured = shipForCaptured;
        return this;
    }

    public PlanetBuilder ResourceProductionPerSecond(float resourceProductionPerSecond) 
    {
        _resourceProductionPerSecond = resourceProductionPerSecond;
        return this;
    }

    public PlanetBuilder MaxShip(int maxShip)
    {
        _maxShip = maxShip;
        return this;
    }

    public Planet Build() 
    {
        GameObject planetGameObject = Object.Instantiate(_planetModel, _position, _rotation, _parent);
        Planet planet = planetGameObject.AddComponent<Planet>();

        Vector3 planetScale = new Vector3(_size, _size, _size);
        planetGameObject.transform.localScale = planetScale;

        planetGameObject.AddComponent<SphereCollider>();

        planet.Init(_shipForCaptured, _resourceProductionPerSecond, _maxShip);

        PlanetView planetView = Object.Instantiate(_planetViewPrefab, planetGameObject.transform);
        PlanetPresenter planetPresenter = new PlanetPresenter(planet, planetView);

        Reset();

        return planet;
    }

    private void Reset()
    {
        _planetModel = null;
        _position = Vector3.zero;
        _rotation = Quaternion.identity;
        _size = 0;
        _shipForCaptured = 0;
        _resourceProductionPerSecond = 0;
        _maxShip = 0;
    }
}
