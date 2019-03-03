using UnityEngine;
using UnityEngine.SceneManagement;

using Random = System.Random;
using System.Collections;

public class PointOfInterest
{
    string _name;

    float[] _resourceDensities;

    Vector3 _location;
    GameObject _model;
    GameObject _prefab;
    PointOfInterestType _type;

    Random _random;
    bool _isBusy = false;

    public string name { get { return _name; } protected set { _name = value; } }

    public float[] resourceDensities { get { return _resourceDensities; } protected set { _resourceDensities = value; } }

    public Vector3 location { get { return _location; } }
    public GameObject model { get { return _model; } protected set { _model = value; } }
    public GameObject prefab { get { return _prefab; } }
    public PointOfInterestType type { get { return _type; } protected set { _type = value; } }

    protected Random random { get { return _random; } }
    public bool isBusy { get { return _isBusy; } }

    public PointOfInterest(string name, Vector3 location, Random random)
    {
        _name = name;
        _location = location;
        _random = random;

        _type = PointOfInterestType.Default;
    }

    public virtual GameObject Instantiate()
    {
        SetResourceDensities();

        if (_model != null)
            _prefab = Object.Instantiate(_model, _location, Quaternion.identity, null);

        return _prefab;
    }
    public virtual void Deinstantiate()
    {
        if (_prefab != null)
            Object.Destroy(_prefab);
    }

    public virtual void Move(Vector3 location, float moveTime = 1f)
    {
        //ayyy lmao
        OverworldUIManager.getInstance.StartCoroutine(MoveAsync(location, moveTime));
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
        LocalMapData lmd = GenerateLocalMapData();
        lmd.AddFleet(PlayerData.fleet);

        RuntimeData.SetLocalMapData(lmd);
        SceneManager.LoadSceneAsync("LocalMap");
    }
    protected virtual LocalMapData GenerateLocalMapData()
    {
        return new LocalMapData(
            _name,
            _resourceDensities,
            _location);
    }

    public virtual string GetTooltip()
    {
        return GetDistanceTooltip();
    }
    protected string GetDistanceTooltip()
    {
        if (this == PlayerData.fleet)
            return "";

        float daysAway = Vector3.Distance(_location, PlayerData.fleet.location) / PlayerData.fleet.GetVital(FleetVitalType.Range).current;
        return (daysAway > 1 ? (daysAway.ToString("0.##") + " days away.") : "In range.");
    }

    void SetResourceDensities()
    {
        int maxNebulaGas = 0;
        int maxVeldspar = 0;
        int maxTritanite = 0;

        switch (_type)
        {
            case PointOfInterestType.Planet:
                maxNebulaGas = 10;
                maxVeldspar = 50;
                maxTritanite = 30;
                break;
            case PointOfInterestType.Star:
                maxNebulaGas = 60;
                maxVeldspar = 20;
                maxTritanite = 10;
                break;
            case PointOfInterestType.Nebula:
                maxNebulaGas = 100;
                maxVeldspar = 0;
                maxTritanite = 0;
                break;
            default:
                maxNebulaGas = 0;
                maxVeldspar = 0;
                maxTritanite = 0;
                break;
        }

        _resourceDensities = new float[]
        {
            _random.Next(0, maxNebulaGas) * .01f,
            _random.Next(0, maxVeldspar) * .01f,
            _random.Next(0, maxTritanite) * .01f
        };
    }

    IEnumerator MoveAsync(Vector3 location, float moveTime)
    {
        _isBusy = true;

        Vector3 o = _location;
        float t = 0f;

        while (t <= moveTime)
        {
            t += Time.deltaTime;

            Vector3 p = Vector3.Lerp(o, location,Mathf.SmoothStep(0f, 1f, t / moveTime));

            _prefab.transform.position = p;
            _location = p;
            yield return null;
        }

        _location = location;
        _isBusy = false;
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
