using UnityEngine;

using System.Collections.Generic;
using Random = System.Random;

public static class HexGrid
{
    static int _size;

    static GameObject _cellPrefab;
    static Random _random;

    static Cell[,] _cells;
    static List<GameObject> _entities;

    public static void Initialize(int size, Random random)
    {
        //add map border
        _size = size;
        _cellPrefab = Resources.Load<GameObject>("cell");
        _cells = new Cell[_size, _size];
        _entities = new List<GameObject>();

        _random = random;

        Debug.Log(_size);

        Create();
    }

    static void Create()
    {
        for (int r = 0; r < _size; r++)
            for (int c = 0; c < _size; c++)
                CreateCell(c, r);
    }
    static void CreateCell(int column, int row)
    {
        _cells[column, row] = new Cell(column, row);
    }

    public static Cell Get(int column, int row)
    {
        return _cells[column, row];
    }
    public static Cell GetRandom()
    {
        return _cells[_random.Next(0, _cells.GetLength(0)), _random.Next(0, _cells.GetLength(1))];
    }

    public static Cell GetByCubic(Vector3 cubic)
    {
        int column = (int)cubic.x;
        int row = (int)(cubic.z + (cubic.x + ((int)cubic.x & 1)) / 2);

        return _cells[column, row];
    }

    public static void Instantiate()
    {
        for (int x = 0; x < _cells.GetLength(0); x++)
            for (int y = 0; y < _cells.GetLength(1); y++)
                InstantiateCell(_cells[x, y]);
    }
    static void InstantiateCell(Cell cell)
    {
        GameObject g = Object.Instantiate(_cellPrefab, cell.ToWorld(), Quaternion.identity, null);
        g.name = "Cell [" + cell.column + ", " + cell.row + "]";
        g.GetComponent<HexCellEntity>().Initialize(cell);

        _entities.Add(g);
    }

    public static void Enter()
    {
        for (int i = 0; i < _entities.Count; i++)
            Object.Destroy(_entities[i]);

        OnEnter?.Invoke();
        OnEnter = null;
    }

    public delegate void CleanUpEvent();
    public static event CleanUpEvent OnEnter;
}