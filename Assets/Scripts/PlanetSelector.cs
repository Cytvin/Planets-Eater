using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PlanetSelector : MonoBehaviour
{
    [SerializeField]
    private float _maxDistanceBetwenPlanet = 25f;
    [SerializeField]
    private LineRenderer _line;
    private Planet _from;
    private Planet _to;

    public event System.Action<Planet> PlanetSelected;
    public event System.Action<Planet> PlanetDeselected;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Planet pickedPlanet = PickPLanet();

            if (pickedPlanet == null) 
            {
                return;
            }

            if (_from == null)
            {
                if (pickedPlanet.Owner != null) //Для теста, чтобы проверять поведение кораблей. Потом поменять на == Owner.Player
                {
                    _from = pickedPlanet;
                    PlanetSelected?.Invoke(_from);
                }
            }
            else
            {
                if (pickedPlanet != _from)
                {
                    _to = pickedPlanet;
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            Refresh();
        }

        if (_from != null)
        {
            Vector3 startLinePosition = _from.transform.position;
            Vector3 mousePosition = Input.mousePosition;
            Ray searchPlanetRay = Camera.main.ScreenPointToRay(mousePosition);
            mousePosition.z = Camera.main.transform.position.y - 2.5f;
            Vector3 endLinePosition = Camera.main.ScreenToWorldPoint(mousePosition);

            if (Physics.Raycast(searchPlanetRay, out RaycastHit hit))
            {
                if (hit.collider.TryGetComponent<Planet>(out _))
                {
                    endLinePosition = hit.transform.position;
                }
            }

            _line.positionCount = 2;
            _line.SetPosition(0, startLinePosition);
            _line.SetPosition(1, endLinePosition);

            if (Vector3.Distance(startLinePosition, endLinePosition) > _maxDistanceBetwenPlanet)
            {
                _line.material.color = Color.red;
            }
            else
            {
                _line.material.color = Color.blue;
            }

            if (_to != null)
            {
                if (Vector3.Distance(startLinePosition, _to.transform.position) > _maxDistanceBetwenPlanet)
                {
                    Debug.Log("Расстояние между планетами больше максимального");
                    Refresh();
                    return;
                }

                Transfer(_from, _to);
                Refresh();
            }
        }
    }

    public Planet PickPLanet()
    {
        Vector3 screenPoint = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(screenPoint);
        if (Physics.Raycast(ray, out RaycastHit raycastHit))
        {
            if (raycastHit.collider.TryGetComponent<Planet>(out Planet planet))
            {
                return planet;
            }
        }

        return null;
    }

    private void Refresh()
    {
        PlanetDeselected?.Invoke(_from);
        _from = null;
        _to = null;

        _line.positionCount = 0;
    }

    private void Transfer(Planet from, Planet to)
    {
        if (from.ShipCount == 0)
        {
            Debug.Log("Нет кораблей для отправки");
            return;
        }

        int shipAmount;

        if (from.Owner == to.Owner)
        {
            shipAmount = Mathf.Min(to.MaxOwnerShipToReceive, from.MaxShipToSend);
        }
        else
        {
            shipAmount = Mathf.Min(to.GetMaxEnemyShipToReceive(from.Owner), from.MaxShipToSend);
        }

        Debug.Log($"Будет отправлено {shipAmount} кораблей");
        from.SendShipsByAmount(to, shipAmount);
    }
}