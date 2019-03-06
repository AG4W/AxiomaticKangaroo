using UnityEngine;

using Random = System.Random;

public class HexGrid
{
    int _size;
    GameObject _cellPrefab;
    Random _random;

    Cell[,] _cells;

    public HexGrid(int size, Random random)
    {
        //add map border
        _size = size / HexMetrics.size;
        _cellPrefab = Resources.Load<GameObject>("cell");
        _cells = new Cell[(_size * 2) + 1, (_size * 2) + 1];

        _random = random;

        Create();
    }

    public void Instantiate()
    {
        for (int x = 0; x < _cells.GetLength(0); x++)
            for (int y = 0; y < _cells.GetLength(1); y++)
                InstantiateCell(_cells[x, y]);
    }

    void Create()
    {
        for (int r = -_size; r <= _size; r++)
            for (int c = -_size; c <= _size; c++)
                CreateCell(c, r);
    }
    void CreateCell(int column, int row)
    {
        _cells[column + _size, row + _size] = new Cell(column, row);
    }
    void InstantiateCell(Cell cell)
    {
        GameObject g = Object.Instantiate(_cellPrefab, Cell.GridToWorld(cell), Quaternion.identity, null);
        g.name = "Cell [" + cell.column + ", " + cell.row + "]";
        g.GetComponent<HexCellEntity>().Initialize(cell);
    }

    public Cell Get(int column, int row)
    {
        return _cells[column + _size, row + _size];
    }
    public Cell GetRandom()
    {
        return _cells[_random.Next(0, _cells.GetLength(0)), _random.Next(0, _cells.GetLength(1))];
    }
}
