using UnityEngine;

public class Visualizer : MonoBehaviour
{
    static Visualizer _instance;
    public static Visualizer getInstance { get { return _instance; } }

    LineRenderer _lineRenderer;
    GameObject _target;

    bool _ignoreXZRotation;

    void Awake()
    {
        _instance = this;
        _lineRenderer = this.GetComponent<LineRenderer>();
        _lineRenderer.enabled = false;
    }

    void Update()
    {
        if(_target != null)
        {
            _lineRenderer.transform.position = _target.transform.position;
            _lineRenderer.transform.eulerAngles = _ignoreXZRotation ? new Vector3(0, _target.transform.eulerAngles.y, 0) : _target.transform.eulerAngles;
        }
    }

    public void DrawLine(GameObject target, Vector3[] positions, Color color, bool ignoreXZRotation)
    {
        _target = target;
        _ignoreXZRotation = ignoreXZRotation;
        _lineRenderer.positionCount = positions.Length;
        _lineRenderer.SetPositions(positions);

        _lineRenderer.startColor = color;
        _lineRenderer.endColor = color;

        _lineRenderer.enabled = true;
    }
    public void Hide()
    {
        _target = null;
        _lineRenderer.enabled = false;
    }
}
