using UnityEngine;

using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour
{
    Mesh _mesh;

    List<Vector3> _vertices = new List<Vector3>();
    List<int> _triangles = new List<int>();

    void Awake()
    {
        _mesh = new Mesh();

        this.GetComponent<MeshFilter>().mesh = _mesh;
    }

    public void Triangulate(HexCellEntity cell)
    {
        _mesh.Clear();

        _vertices.Clear();
        _triangles.Clear();

        Vector3 c = Vector3.zero;

        for (int i = 0; i < 6; i++)
            AddTriangle(c, c + HexMetrics.corners[i] * HexMetrics.visiblePercentage, c + HexMetrics.corners[i + 1] * HexMetrics.visiblePercentage);

        _mesh.vertices = _vertices.ToArray();
        _mesh.triangles = _triangles.ToArray();
        _mesh.RecalculateNormals();
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
