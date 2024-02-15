using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PlanetConnector : MonoBehaviour
{
    private LineRenderer _lineRenderer;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    public void Init(Vector3 startPosition, Vector3 endPosition)
    {
        _lineRenderer.SetPosition(0, startPosition);
        _lineRenderer.SetPosition(1, endPosition);
    }
}
