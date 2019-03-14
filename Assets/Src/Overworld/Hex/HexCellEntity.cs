using UnityEngine;

public class HexCellEntity : MonoBehaviour
{
    Renderer _renderer;

    Color _color;
    Color _occupyColor = new Color(1f, 0f, 0f, .2f);

    Cell _cell;

    void Start()
    {
        this.GetComponent<HexMesh>().Triangulate();

        _renderer = this.GetComponent<Renderer>();
        _color = _renderer.material.color;
    }

    public void Initialize(Cell cell)
    {
        _cell = cell;
        _cell.OnStatusChanged += OnStatusChanged;
    }

    public void OnMouseEnter()
    {
        Color c = _renderer.material.color;
        c.a = .2f;

        _renderer.material.color = c;
    }
    public void OnMouseDown()
    {
        PlayerData.fleet.Move(_cell);
    }
    public void OnMouseExit()
    {
        Color c = _renderer.material.color;
        c.a = .05f;

        _renderer.material.color = c;
    }

    void OnStatusChanged(bool isOccupied)
    {
        _renderer.material.color = isOccupied ? _occupyColor : _color;
    }
}
