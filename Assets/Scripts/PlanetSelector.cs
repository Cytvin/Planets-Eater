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

    private Player _player;

    public event System.Action<Planet> PlanetSelected;
    public event System.Action<Planet> PlanetDeselected;

    public void Init(Player player)
    {
        _player = player;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SelectPlanet();
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (_from == null)
            {
                return;
            }

            Refresh();
        }

        if (_from != null)
        {
            Vector3 fromPosition;
            Vector3 endLinePosition;
            (fromPosition, endLinePosition) = GetStartAndEndLinePosition();

            DrawLine(fromPosition, endLinePosition);

            if (_to != null)
            {
                if (!_from.IsNeighbor(_to) /*|| _from.GetDistanceToNeighbor(_to) > _maxDistanceBetwenPlanet*/)
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

    private void SelectPlanet()
    {
        Planet selectedPlanet = SearchPlanetByRayCast();

        if (selectedPlanet == null)
        {
            return;
        }

        if (_from == null)
        {
            if (selectedPlanet.Owner == _player)
            {
                _from = selectedPlanet;
                PlanetSelected?.Invoke(_from);
            }
        }
        else
        {
            if (selectedPlanet != _from)
            {
                _to = selectedPlanet;
            }
        }
    }

    private Planet SearchPlanetByRayCast()
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

    private (Vector3, Vector3) GetStartAndEndLinePosition()
    {
        Vector3 fromPosition = _from.transform.position;
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

        return (fromPosition, endLinePosition);
    }

    private void DrawLine(Vector3 startLinePosition, Vector3 endLinePosition)
    {
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

        shipAmount = Mathf.Min(to.GetMaxShipToReceiveByPlayer(from.Owner), from.MaxShipToSend);

        Debug.Log($"Будет отправлено {shipAmount} кораблей");
        from.SendShipsByAmount(to, shipAmount);
    }
}