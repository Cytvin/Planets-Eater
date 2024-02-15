using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PlanetConnector : MonoBehaviour
{
    private LineRenderer _lineRenderer;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.material.color = Color.white;
    }

    public void Init(Vector3 startPosition, Vector3 endPosition)
    {
        _lineRenderer.SetPosition(0, startPosition);
        _lineRenderer.SetPosition(1, endPosition);
    }

    public void Select()
    {
        _lineRenderer.material.color = Color.red;
    }

    public void UnSelect()
    {
        _lineRenderer.material.color = Color.white;
    }
}
