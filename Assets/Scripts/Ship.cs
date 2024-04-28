using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Ship : MonoBehaviour
{
    public enum ShipState
    {
        Takeoff,
        Holding,
        Fly,
        Landing,
        Fight
    }

    [SerializeField]
    private NavMeshAgent _agent;
    [SerializeField]
    private Collider _collider;
    [SerializeField]
    private ShipState _state;
    private Player _owner;
    private Planet _currentPlanet;
    private float _holdingRadius;
    private float _speed = 5f;
    private float _angle;
    private float _searchRadius = 15f;
    private Ship _currentEnemy;

    private float _health = 10f;
    private float _damage = 2f;

    private float _timeToAttack = 3f;
    private float _timeFromLastAttack = 0f;
    private float _attackRange = 4f;

    [SerializeField]
    private LineRenderer _laser;

    public event System.Action<Ship> Dead;

    public ShipState State => _state;
    public Player Owner => _owner;
    public Planet TargetPlanet => _currentPlanet;

    public void Init(Planet currentPlanet, Player owner, float damge)
    {
        _state = ShipState.Takeoff;
        _owner = owner;
        _laser.material.color = owner.Color;
        _damage = damge;

        Vector3 rotationAngle = new Vector3(0, Random.Range(0.0f, 360.0f), 0);
        transform.Rotate(rotationAngle);

        _angle = transform.rotation.eulerAngles.y * Mathf.Deg2Rad;

        ChangeCurrentPlanet(currentPlanet);
    }

    private void Update()
    {
        if (_state == ShipState.Takeoff)
        {
            if (Vector3.Distance(transform.position, _currentPlanet.transform.position) > _currentPlanet.Radius + 1)
            {
                _agent.enabled = true;
                _collider.enabled = true;
                _state = ShipState.Holding;
            }
             
            transform.Translate(_speed * Time.deltaTime * transform.forward, Space.World);
        }
        else if (_state == ShipState.Holding)
        {
            _angle += Time.deltaTime;

            float x = Mathf.Cos(_angle * 0.5f) * _holdingRadius;
            float z = Mathf.Sin(_angle * 0.5f) * _holdingRadius;

            Vector3 nextPoint = new Vector3(x, 0, z) + _currentPlanet.Position;

            _agent.SetDestination(nextPoint);
        }
        else if (_state == ShipState.Fly)
        {
            if (Vector3.Distance(transform.position, _currentPlanet.Position) < _currentPlanet.SearchRadius)
            {
                _currentEnemy = SearchEnemy();
            }

            if (_currentEnemy != null) 
            {
                _state = ShipState.Fight;
                return;
            }

            if (Vector3.Distance(transform.position, _currentPlanet.Position) > _holdingRadius)
            {
                return;
            }

            if (_currentPlanet.Owner == _owner)
            {
                _state = ShipState.Holding;
            }
            else if (_currentPlanet.Owner == null)
            {
                _agent.enabled = false;
                _state = ShipState.Landing;
            }
            else
            {
                _state = ShipState.Fight;
            }
        }
        else if (_state == ShipState.Landing)
        {
            if (_currentPlanet.Owner == _owner)
            {
                _agent.enabled = true;
                _state = ShipState.Holding;
                return;
            }
            else if (_currentPlanet.Owner != _owner && _currentPlanet.Owner != null && SearchEnemy() != null)
            {
                _agent.enabled = true;
                _state = ShipState.Fight;
                return;
            }

            if (Vector3.Distance(transform.position, _currentPlanet.Position) < _currentPlanet.Radius)
            {
                _currentPlanet.TakeForLanding(this);
                Destroy(gameObject);
                Destroy(this);
            }
            else
            {
                transform.LookAt(_currentPlanet.transform);
                transform.Translate(_speed * Time.deltaTime * transform.forward, Space.World);
            }
        }
        else if (_state == ShipState.Fight)
        {
            if (_currentEnemy == null)
            {
                _currentEnemy = SearchEnemy();
            }

            if (_currentEnemy == null && _currentPlanet.Owner == _owner)
            {
                _state = ShipState.Holding;
                return;
            }
            else if (_currentEnemy == null && _currentPlanet.Owner != _owner)
            {
                _state = ShipState.Landing;
                _agent.enabled = false;
                return;
            }

            Vector3 targetPosition = _currentEnemy.transform.position;
            float distanceToEnemy = Vector3.Distance(transform.position, targetPosition);

            if (distanceToEnemy > _attackRange) 
            {
                _agent.SetDestination(targetPosition);
            }
            else
            {
                _agent.SetDestination(transform.position);
            }

            _timeFromLastAttack += Time.deltaTime;

            if (_timeFromLastAttack > _timeToAttack && distanceToEnemy <= _attackRange)
            {
                _currentEnemy.ApplyDamage(_damage);
                _timeFromLastAttack = 0;
                _laser.positionCount = 2;
                _laser.SetPosition(0, transform.position);
                _laser.SetPosition(1, targetPosition);
                StartCoroutine(RefreshLaser());
            }
        }
    }
    private void OnDestroy()
    {
        Dead?.Invoke(this);
    }

    public void FlyToPlanet(Planet planet)
    {
        _state = ShipState.Fly;
        _currentPlanet = planet;
        _agent.enabled = true;
        _agent.SetDestination(_currentPlanet.Position);

        _currentPlanet.AddShip(this);
        ChangeCurrentPlanet(planet);
    }

    public void DefendPlanet()
    {
        _state = ShipState.Fight;
    }

    public void ApplyDamage(float damage)
    {
        if (damage < 0)
        {
            return;
        }

        _health -= damage;

        if (_health <= 0)
        {
            Destroy(gameObject);
            Destroy(this);
        }
    }

    private void ChangeCurrentPlanet(Planet planet)
    {
        _currentPlanet = planet;
        _holdingRadius = Random.Range(_currentPlanet.Radius + 2, _currentPlanet.SearchRadius - 1);
    }

    private Ship SearchEnemy()
    {
        Ship enemy = null;

        Collider[] searchResult = Physics.OverlapSphere(transform.position, _searchRadius);

        float closestDistance = _searchRadius;
        Vector3 currentPosition = transform.position;

        foreach (Collider collider in searchResult)
        {
            if (collider.TryGetComponent<Ship>(out Ship ship) && ship.TargetPlanet == _currentPlanet && ship != this && ship.Owner != _owner)
            {
                float distance = Vector3.Distance(currentPosition, ship.transform.position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    enemy = ship;
                }
            }
        }

        return enemy;
    }

    private IEnumerator RefreshLaser()
    {
        yield return new WaitForSeconds(1f);
        _laser.positionCount = 0;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _searchRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }
}