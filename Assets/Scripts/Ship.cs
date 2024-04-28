using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Ship : MonoBehaviour
{
    [SerializeField]
    private NavMeshAgent _agent;
    [SerializeField]
    private LineRenderer _laser;

    private IShipState _shipState;
    private Player _owner;
    private Planet _currentPlanet;
    private ShipWeapon _weapon;
    private float _holdingRadius;
    private float _speed = 5f;
    private float _searchRadius = 15f;
    private float _health = 10f;

    public event System.Action<Ship> Dead;

    public bool IsHolding => _shipState is HoldingState;
    public Player Owner => _owner;
    public Planet CurrentPlanet => _currentPlanet;
    public ShipWeapon Weapon => _weapon;
    public NavMeshAgent Agent => _agent;
    public float Speed => _speed;
    public float HoldingRadius => _holdingRadius;

    public void Init(Planet currentPlanet, Player owner, float damage)
    {
        _owner = owner;

        _shipState = new TakeOffState();
        _weapon = new ShipWeapon(this, _laser, damage);
        _laser.material.color = owner.Color;

        Vector3 rotationAngle = new Vector3(0, Random.Range(0.0f, 360.0f), 0);
        transform.Rotate(rotationAngle);

        ChangeCurrentPlanet(currentPlanet);
    }

    private void Update()
    {
        _shipState = _shipState.Update(this);
    }

    private void OnDestroy()
    {
        Dead?.Invoke(this);
    }

    public void FlyToPlanet(Planet planet)
    {
        ChangeCurrentPlanet(planet);

        _currentPlanet.AddShip(this);

        _shipState = new FlyState(this, planet);
    }

    public void ApplyDamage(float damage)
    {
        if (damage < 0)
        {
            throw new System.ArgumentOutOfRangeException(nameof(damage), "Урон не может быть отрицательным");
        }

        _health -= damage;

        if (_health <= 0)
        {
            Destroy(gameObject);
            Destroy(this);
        }
    }

    public Ship SearchEnemy()
    {
        Ship enemy = null;

        float distanceToClosestEnemy = _searchRadius;
        Vector3 currentPosition = transform.position;

        Collider[] searchResult = Physics.OverlapSphere(currentPosition, _searchRadius);
        IEnumerable<Ship> ships = GetAllShipsFromColliders(searchResult);

        foreach (Ship ship in ships)
        {
            if (ship.CurrentPlanet == _currentPlanet && IsItEnemyShip(ship))
            {
                float distance = Vector3.Distance(currentPosition, ship.transform.position);

                if (distance < distanceToClosestEnemy)
                {
                    distanceToClosestEnemy = distance;
                    enemy = ship;
                }
            }
        }

        return enemy;
    }

    private IEnumerable<Ship> GetAllShipsFromColliders(Collider[] colliders)
    {
        List<Ship> ships = new List<Ship>();

        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent(out Ship ship))
            {
                ships.Add(ship);
            }
        }

        return ships;
    }

    private bool IsItEnemyShip(Ship ship)
    {
        return ship != this && ship.Owner != _owner;
    }

    private void ChangeCurrentPlanet(Planet planet)
    {
        _currentPlanet = planet;
        _holdingRadius = Random.Range(_currentPlanet.Radius + 2, _currentPlanet.SearchRadius - 1);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _searchRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, Weapon.AttackRange);
    }
}