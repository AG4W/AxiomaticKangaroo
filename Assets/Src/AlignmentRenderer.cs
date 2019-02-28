using UnityEngine;

public class AlignmentRenderer : MonoBehaviour
{
    Transform _alignmentPlane;
    LineRenderer _lineRenderer;

    Vector3 _alignmentPos;

    void Start()
    {
        _alignmentPlane = this.transform.Find("alignmentPlane");
        _alignmentPos = new Vector3(this.transform.position.x, AlignmentPlane.y, this.transform.position.z);

        _lineRenderer = this.GetComponent<LineRenderer>();
        _lineRenderer.loop = false;
        _lineRenderer.useWorldSpace = true;
        _lineRenderer.positionCount = 2;
        _lineRenderer.startWidth = .1f;
        _lineRenderer.endWidth = .1f;
        _lineRenderer.SetPositions(new Vector3[] { this.transform.position, _alignmentPos });
        _alignmentPlane.transform.position = _alignmentPos;
    }
    void Update()
    {
        _alignmentPos.y = AlignmentPlane.y;
        _lineRenderer.SetPosition(1, _alignmentPos);
        _alignmentPlane.transform.position = _alignmentPos;
    }
}
