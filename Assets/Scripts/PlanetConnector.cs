using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PlanetConnector : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    private Planet _planetA;
    private Planet _planetB;

    public Vector3 Start => _lineRenderer.GetPosition(0);
    public Vector3 End => _lineRenderer.GetPosition(1);

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.material.color = Color.white;
    }

    public void Connect(Planet planetFrom, Planet PlanetTo)
    {
        if (planetFrom == null || PlanetTo == null)
        {
            throw new System.ArgumentNullException(nameof(planetFrom), $"planetFrom or planetTo is null");
        }

        _planetA = planetFrom;
        _planetB = PlanetTo;

        _planetA.AddConnector(this);
        _planetB.AddConnector(this);
        _planetA.Connect(_planetB);
        _planetB.Connect(_planetA);

        SetStartAndEnd(_planetA.Coordinate, _planetB.Coordinate);
    }

    public void Select()
    {
        _lineRenderer.material.color = Color.red;
    }

    public void UnSelect()
    {
        _lineRenderer.material.color = Color.white;
    }

    private void SetStartAndEnd(Vector3 start, Vector3 end)
    {
        float distance = 2.5f;

        Vector3 directionStartToEnd = (end - start).normalized;
        Vector3 directionEndToStart = (start - end).normalized;

        start += directionStartToEnd * distance;
        end += directionEndToStart * distance;

        _lineRenderer.SetPosition(0, start);
        _lineRenderer.SetPosition(1, end);
    }
}
