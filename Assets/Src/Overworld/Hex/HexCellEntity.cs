using UnityEngine;

public class HexCellEntity : MonoBehaviour
{
    Renderer _renderer;

    Color _defaultColor;
    Color _fleetOccupyColor = Color.red;

    Cell _cell;

    void Start()
    {
        this.GetComponent<HexMesh>().Triangulate();

        _renderer = this.GetComponent<Renderer>();

        _defaultColor = _renderer.material.color;
        //_fleetOccupyColor.a *= .05f;
    }

    public void Initialize(Cell cell)
    {
        _cell = cell;
        _cell.OnStatusChanged += OnStatusChanged;
    }

    public void OnMouseEnter()
    {
        _defaultColor = _renderer.material.color;

        Color c = _defaultColor;
        c.a *= 2;

        _renderer.material.color = c;
    }
    public void OnMouseDown()
    {
        PlayerData.fleet.Move(_cell);
    }
    public void OnMouseExit()
    {
        if (_cell.isOccupied)
            _renderer.material.color = _fleetOccupyColor;
        else
            _renderer.material.color = _defaultColor;
    }

    void OnStatusChanged(bool status)
    {
        if (status)
            _renderer.material.color = _fleetOccupyColor;
        else
            _renderer.material.color = _defaultColor;
    }
}
