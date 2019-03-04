using UnityEngine;

public class HexGrid
{
    int _size;
    GameObject _cellPrefab;

    public HexGrid(int size)
    {
        //add map border
        _size = size / HexMetrics.outerRadius;
        _cellPrefab = Resources.Load<GameObject>("cell");

        Create();
    }

    void Create()
    {
        for (int z = -_size; z <= _size; z++)
        {
            for (int x = -_size; x <= _size; x++)
            {
                if (z == 0 && x == 0)
                    continue;

                CreateCellObject(x, z);
            }
        }
            
    }
    void CreateCellObject(int x, int z)
    {
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
        position.y = 0;
        position.z = z * (HexMetrics.outerRadius * 1.5f);

        GameObject g = Object.Instantiate(_cellPrefab, Vector3.zero, Quaternion.identity, null);
        g.transform.position = position;
    }
}
