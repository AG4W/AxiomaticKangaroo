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

        HexGrid.OnEnter += OnEnter;
    }

    public void OnMouseEnter()
    {
        OverworldUIManager.getInstance.UpdateCurrentSector(_cell);

        Color c = _renderer.material.color;
        c.a = .2f;

        _renderer.material.color = c;
    }
    public void OnMouseDown()
    {
        TooltipManager.getInstance.CloseTooltip();

        if(PlayerData.fleet.cell == _cell)
            _cell.Enter();
        else
            PlayerData.fleet.Move(_cell);
    }
    public void OnMouseExit()
    {
        OverworldUIManager.getInstance.UpdateCurrentSector(null);

        Color c = _renderer.material.color;
        c.a = .05f;

        _renderer.material.color = c;
    }

    void OnStatusChanged(bool isOccupied)
    {
        _renderer.material.color = isOccupied ? _occupyColor : _color;
    }
    void OnEnter()
    {
        _cell.OnStatusChanged -= OnStatusChanged;

        HexGrid.OnEnter -= OnEnter;
    }
}
