using UnityEngine;

using Random = System.Random;

public class PointOfInterest
{
    string _name;

    Cell _cell;
    Vector3 _location;

    GameObject _model;
    GameObject _prefab;
    PointOfInterestType _type;

    Random _random;
    bool _isBusy = false;

    public string name { get { return _name; } protected set { _name = value; } }

    public Cell cell { get { return _cell; } }
    public Vector3 location { get { return _location; } }

    public GameObject model { get { return _model; } protected set { _model = value; } }
    public GameObject prefab { get { return _prefab; } }
    public PointOfInterestType type { get { return _type; } protected set { _type = value; } }

    protected Random random { get { return _random; } }
    public bool isBusy { get { return _isBusy; } }

    public PointOfInterest(string name, Cell cell, Random random)
    {
        _name = name;
        _cell = cell;

        _random = random;

        _type = PointOfInterestType.Default;
    }

    public virtual GameObject Instantiate()
    {
        _cell.AddPointOfInterest(this);

        float boundaries = (HexMetrics.size / 2) * .8f;

        if (_type == PointOfInterestType.Star || _type == PointOfInterestType.Fleet)
            _location = _cell.location;
        else
            _location = _cell.location + new Vector3(
                _random.NextFloat(-boundaries, boundaries),
                0f,
                _random.NextFloat(-boundaries, boundaries));

        if (_model != null)
            _prefab = Object.Instantiate(_model, _location, Quaternion.identity, null);

        return _prefab;
    }
    public virtual void Deinstantiate()
    {
        if (_prefab != null)
            Object.Destroy(_prefab);
    }

    public virtual void Move(Cell cell, float moveTime = 1f)
    {
        //old cell
        _cell.RemovePointOfInterest(this);
        //new
        _cell = cell;
        _cell.AddPointOfInterest(this);

        _location = cell.location;
        _prefab.transform.position = _location;
    }

    public virtual void OnMouseEnter()
    {
        TooltipManager.getInstance.OpenTooltip(GetTooltip(), Input.mousePosition);
        OverworldInputManager.getInstance.HandlePoIMouseCallback(CallbackType.MouseEnter, this);
    }
    public virtual void OnLeftClick()
    {
        OverworldInputManager.getInstance.HandlePoIMouseCallback(CallbackType.LeftDown, this);
    }
    public virtual void OnScrollClick()
    {
        OverworldInputManager.getInstance.HandlePoIMouseCallback(CallbackType.ScrollDown, this);
    }
    public virtual void OnRightClick()
    {
        OverworldInputManager.getInstance.HandlePoIMouseCallback(CallbackType.RightDown, this);
    }
    public virtual void OnMouseExit()
    {
        TooltipManager.getInstance.CloseTooltip();
        OverworldInputManager.getInstance.HandlePoIMouseCallback(CallbackType.MouseExit, this);
    }

    public virtual void Interact()
    {
        Debug.Log("Interacting with " + _name);
    }

    public virtual string GetTooltip()
    {
        return "";
    }
}
public enum PointOfInterestType
{
    Fleet,
    Planet,
    Star,
    Nebula,
    Wormhole,
    Structure,
    Default
}
