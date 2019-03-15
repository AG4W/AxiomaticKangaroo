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
        _size = size;
        _cellPrefab = Resources.Load<GameObject>("cell");
        _cells = new Cell[_size, _size];

        _random = random;

        Create();
    }

    void Create()
    {
        for (int r = 0; r < _size; r++)
            for (int c = 0; c < _size; c++)
                CreateCell(c, r);
    }
    void CreateCell(int column, int row)
    {
        _cells[column, row] = new Cell(column, row);
    }

    public Cell Get(int column, int row)
    {
        return _cells[column, row];
    }
    public Cell GetRandom()
    {
        return _cells[_random.Next(0, _cells.GetLength(0)), _random.Next(0, _cells.GetLength(1))];
    }

    public Cell GetByCubic(Vector3 cubic)
    {
        int column = (int)cubic.x;
        int row = (int)(cubic.z + (cubic.x + ((int)cubic.x & 1)) / 2);

        return _cells[column, row];
    }

    public void Instantiate()
    {
        for (int x = 0; x < _cells.GetLength(0); x++)
            for (int y = 0; y < _cells.GetLength(1); y++)
                InstantiateCell(_cells[x, y]);
    }
    void InstantiateCell(Cell cell)
    {
        GameObject g = Object.Instantiate(_cellPrefab, cell.ToWorld(), Quaternion.identity, null);
        g.name = "Cell [" + cell.column + ", " + cell.row + "]";
        g.GetComponent<HexCellEntity>().Initialize(cell);
    }
}