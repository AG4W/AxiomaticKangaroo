using UnityEngine;

using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour
{
    Mesh _mesh;
    MeshCollider _collider;

    List<Vector3> _vertices = new List<Vector3>();
    List<int> _triangles = new List<int>();

    void Awake()
    {
        _mesh = new Mesh();
        _collider = this.GetComponent<MeshCollider>();

        this.GetComponent<MeshFilter>().mesh = _mesh;
    }

    public void Triangulate()
    {
        _mesh.Clear();

        _vertices.Clear();
        _triangles.Clear();

        for (int i = 0; i < 6; i++)
            AddTriangle(Vector3.zero, HexMetrics.GetHexagonPoint(i) * HexMetrics.visiblePercentage, HexMetrics.GetHexagonPoint(i + 1) * HexMetrics.visiblePercentage);

        _mesh.vertices = _vertices.ToArray();
        _mesh.triangles = _triangles.ToArray();
        _mesh.RecalculateNormals();

        _collider.sharedMesh = null;
        _collider.sharedMesh = _mesh;
        _collider.convex = true;
        _collider.isTrigger = true;
    }
    void AddTriangle(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        int vertexIndex = _vertices.Count;

        _vertices.Add(p1);
        _vertices.Add(p2);
        _vertices.Add(p3);

        _triangles.Add(vertexIndex);
        _triangles.Add(vertexIndex + 1);
        _triangles.Add(vertexIndex + 2);
    }
}
