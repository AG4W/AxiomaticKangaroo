using UnityEngine;

using Random = System.Random;

public class Cell
{
    int _column;
    int _row;

    float[] _resourceDensities;

    LocalMapData _localMapData;
    Vector3 _location;

    Random _random;

    bool _isOccupied;

    public int column { get { return _column; } }
    public int row { get { return _row; } }

    public LocalMapData localMapData { get { return _localMapData; } }
    public Vector3 location { get { return _location; } }

    public bool isOccupied { get { return _isOccupied; } }

    public Cell(int column, int row)
    {
        _column = column;
        _row = row;

        _random = new Random(column * row);

        _location = GridToWorld(this);
        _isOccupied = false;

        SetResourceDensities();

        _localMapData = new LocalMapData("Sector [" + _column + ", " + _row + "]", _resourceDensities, _location);
    }

    public void Enter()
    {
        _isOccupied = true;

        OnStatusChanged?.Invoke(_isOccupied);
    }
    public void Exit()
    {
        _isOccupied = false;

        OnStatusChanged?.Invoke(_isOccupied);
    }

    void SetResourceDensities()
    {
        int maxNebulaGas = 0;
        int maxVeldspar = 0;
        int maxTritanite = 0;

        _resourceDensities = new float[]
        {
            _random.Next(0, maxNebulaGas) * .01f,
            _random.Next(0, maxVeldspar) * .01f,
            _random.Next(0, maxTritanite) * .01f
        };
    }

    public delegate void CellStatusEvent(bool status);
    public event CellStatusEvent OnStatusChanged;

    public static Vector3 GridToWorld(Cell cell)
    {
        Vector3 position;
        position.x = cell.column * (HexMetrics.width * .75f);
        position.y = 0f;
        position.z = cell.row * HexMetrics.height;

        if (Mathf.Abs(cell.column) % 2 != 0)
            position.z += HexMetrics.height / 2;

        return position;
    }
}
