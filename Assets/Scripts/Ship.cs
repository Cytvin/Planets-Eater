using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Ship : MonoBehaviour
{
    public enum ShipState
    {
        Constructed,
        Holding,
        Fly,
        Fight
    }

    [SerializeField]
    private NavMeshAgent _agent;
    [SerializeField]
    private Collider _collider;
    private ShipState _state;
    private Owner _owner;
    private Planet _currentPlanet;
    private bool _justCreated = true;
    private float _holdingRadius;
    private float _speed = 5f;
    private float _angle;

    public ShipState State => _state;
    public Owner Owner => _owner;
    public Planet TargetPlanet => _currentPlanet;

    public void Init(Planet currentPlanet, Owner owner)
    {
        _state = ShipState.Constructed;
        _owner = owner;
        _currentPlanet = currentPlanet;
    }

    private void Update()
    {
        if (_state == ShipState.Constructed)
        {
            if (_justCreated) 
            {
                Vector3 rotationAngle = new Vector3(0, Random.Range(0.0f, 360.0f), 0);
                transform.Rotate(rotationAngle);

                _angle = transform.rotation.eulerAngles.y * Mathf.Deg2Rad;

                _holdingRadius = Random.Range(4f, 8f);
                _justCreated = false;
            }

            if (Vector3.Distance(transform.position, _currentPlanet.transform.position) >= _holdingRadius)
            {
                _agent.enabled = true;
                _collider.enabled = true;
                _state = ShipState.Holding;
            }

            transform.Translate(transform.forward * _speed * Time.deltaTime);
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
            if (Vector3.Distance(transform.position, _currentPlanet.transform.position) < _holdingRadius)
            {
                _state = ShipState.Holding;
            }
        }
        else if (_state == ShipState.Fight)
        {

        }
    }

    public void FlyToPlanet(Planet planet)
    {
        _state = ShipState.Fly;
        _currentPlanet = planet;
        _agent.SetDestination(_currentPlanet.transform.position);
    }
}
