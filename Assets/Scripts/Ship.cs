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
    private bool _justCreated = true;
    private float _holdingRadius;
    private float _speed = 5f;
    private float _angle;
    private float _searchRadius = 15f;
    private Ship _currentEnemy;

    private float _health = 10f;
    private float _damage = 2f;

    private float _timeToAttack = 0.5f;
    private float _timeFromLastAttack = 0f;

    public ShipState State => _state;
    public Player Owner => _owner;
    public Planet TargetPlanet => _currentPlanet;

    public void Init(Planet currentPlanet, Player owner)
    {
        _state = ShipState.Takeoff;
        _owner = owner;
        _currentPlanet = currentPlanet;
    }

    private void Update()
    {
        if (_state == ShipState.Takeoff)
        {
            if (_justCreated) 
            {
                Vector3 rotationAngle = new Vector3(0, Random.Range(0.0f, 360.0f), 0);
                transform.Rotate(rotationAngle);

                _angle = transform.rotation.eulerAngles.y * Mathf.Deg2Rad;

                _holdingRadius = Random.Range(4f, 8f);
                _justCreated = false;
            }

            if (Vector3.Distance(transform.position, _currentPlanet.transform.position) >= _holdingRadius) //ѕодумать о другом переходе в состо€ние Holding, чтобы кораль мог раньше начать воевать
            {
                _agent.enabled = true;
                _collider.enabled = true;
                _state = ShipState.Holding;
            }
             
            transform.Translate(transform.forward * _speed * Time.deltaTime, Space.World);
        }
        else if (_state == ShipState.Holding)
        {
            _angle += Time.deltaTime;

            float x = Mathf.Cos(_angle * 0.5f) * _holdingRadius;
            float z = Mathf.Sin(_angle * 0.5f) * _holdingRadius;

            Vector3 nextPoint = new Vector3(x, 0, z) + _currentPlanet.transform.position;

            _agent.SetDestination(nextPoint);
        }
        else if (_state == ShipState.Fly)
        {
            _currentEnemy = SearchEnemy();

            if (_currentEnemy != null) 
            {
                _state = ShipState.Fight;
                return;
            }

            if (Vector3.Distance(transform.position, _currentPlanet.transform.position) > _holdingRadius)
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
            else if (_currentPlanet.Owner != _owner && _currentPlanet.Owner != null && SearchEnemy() != null) //ќшибка в изменении состо€ни€, нужно как-то провер€ть что на планете не осталось кораблей или делать Enum с состо€нием планет
            {
                _agent.enabled = true;
                _state = ShipState.Fight;
                return;
            }

            if (Vector3.Distance(transform.position, _currentPlanet.transform.position) < _currentPlanet.Radius)
            {
                _currentPlanet.TakeForLanding(this);
                Destroy(gameObject);
            }
            else
            {
                transform.LookAt(_currentPlanet.transform);
                transform.Translate(transform.forward * _speed * Time.deltaTime, Space.World);
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

            _agent.SetDestination(targetPosition);
            
            //TODO: Ќужно сделать атаку с учетом радиуса атаки, чтобы юниты не атаковали через пол карты
            if (_timeFromLastAttack > _timeToAttack)
            {
                _currentEnemy.ApplyDamage(_damage);
                _timeFromLastAttack = 0;
            }

            _timeFromLastAttack += Time.deltaTime;
        }
    }

    public void FlyToPlanet(Planet planet)
    {
        _state = ShipState.Fly;
        _currentPlanet = planet;
        _agent.enabled = true;
        _agent.SetDestination(_currentPlanet.transform.position);
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _searchRadius);
    }
}