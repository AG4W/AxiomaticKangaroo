using UnityEngine;
using UnityEngine.SceneManagement;

using Random = System.Random;

public class PointOfInterest
{
    string _name;

    Cell _cell;

    GameObject _model;
    GameObject _prefab;
    PointOfInterestType _type;

    Random _random;
    bool _isBusy = false;

    public string name { get { return _name; } protected set { _name = value; } }

    public Cell cell { get { return _cell; } }
    public GameObject model { get { return _model; } protected set { _model = value; } }
    public GameObject prefab { get { return _prefab; } }
    public PointOfInterestType type { get { return _type; } protected set { _type = value; } }

    protected Random random { get { return _random; } }
    public bool isBusy { get { return _isBusy; } }

    public PointOfInterest(string name, Cell location, Random random)
    {
        _name = name;
        _cell = location;
        _random = random;

        _type = PointOfInterestType.Default;
    }

    public virtual GameObject Instantiate()
    {
        if (_model != null)
            _prefab = Object.Instantiate(_model, _cell.location, Quaternion.identity, null);

        return _prefab;
    }
    public virtual void Deinstantiate()
    {
        if (_prefab != null)
            Object.Destroy(_prefab);
    }

    public virtual void Move(Cell cell, float moveTime = 1f)
    {
        _cell = cell;
        _prefab.transform.position = cell.location;
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
        return GetDistanceTooltip();
    }
    protected string GetDistanceTooltip()
    {
        if (this == PlayerData.fleet)
            return "";

        //float daysAway = Vector3.Distance(_location, PlayerData.fleet.location) / PlayerData.fleet.GetVital(FleetVitalType.Range).current;
        //return (daysAway > 1 ? (daysAway.ToString("0.##") + " days away.") : "In range.");
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
