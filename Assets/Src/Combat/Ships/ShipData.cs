using UnityEngine;

using System.Linq;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Data/Premade Ship")]
public class ShipData : ScriptableObject
{
    [Header("Leave empty to randomize.")]
    [SerializeField]string _name;
    [SerializeField]HullData _hull;

    [SerializeField]WeaponData[] _weapons;
    [SerializeField]UtilityData[] _utilities;

    public new string name { get { return _name; } }
    public HullData hull { get { return _hull; } }

    public WeaponData[] weapons { get { return _weapons; } }
    public UtilityData[] utilities { get { return _utilities; } }
}
public class Ship
{
    string _name;

    float[] _vitals;

    Weapon[] _weapons;
    Utility[] _utilities;
    ShipComponent[] _components;
    ResourceScanner _scanner;

    List<FleetVitalModifier> _modifiers = new List<FleetVitalModifier>();

    //components
    Hull _hull;
    HullClass _size;

    //captain
    Officer _officer;
    ShipAllegiance _allegiance;

    //misc
    GameObject _model;
    GameObject _destroyedVFX;

    ShipLog _log = new ShipLog();

    public string name { get { return _name; } }

    public Weapon[] weapons { get { return _weapons; } }
    public Utility[] utilities { get { return _utilities; } }
    public ShipComponent[] components { get { return _components; } }
    public ResourceScanner scanner { get { return _scanner; } }

    public List<FleetVitalModifier> modifiers { get { return _modifiers; } }

    public HullClass size { get { return _size; } }

    public Officer officer { get { return _officer; } }
    public ShipAllegiance allegiance { get { return _allegiance; } }

    public GameObject model { get { return _model; } }
    public GameObject destroyedVFX { get { return _destroyedVFX; } }

    public ShipLog log { get { return _log; } }

    public Ship(ShipData sd)
    {
        _name = sd.name.Length == 0 ? NameGenerator.GetShipName() : sd.name;
        _hull = sd.hull.CreateInstance() as Hull;
        _size = _hull.size;

        _weapons = new Weapon[_hull.weaponSlots];
        _utilities = new Utility[_hull.utilitySlots];
        _components = new ShipComponent[_hull.weaponSlots + _hull.utilitySlots];

        _modifiers.AddRange(_hull.maintenanceCosts);

        _model = _hull.model;
        _destroyedVFX = _hull.destroyedVFX;

        //add preconfigured utilities
        if(sd.weapons != null)
            for (int i = 0; i < sd.weapons.Length; i++)
                if (sd.weapons[i] != null)
                    _weapons[i] = sd.weapons[i].Instantiate() as Weapon;

        if (sd.utilities != null)
            for (int i = 0; i < sd.utilities.Length; i++)
                if (sd.utilities[i] != null)
                    _utilities[i] = sd.utilities[i].Instantiate() as Utility;

        //autofill component slots with components for debug purposes
        var weapons =
                Resources.LoadAll<WeaponData>("Data/Parts/Weapons")
                .Where(w => (int)w.minimumSize <= (int)_hull.size)
                .ToArray();
        var utilities =
                Resources.LoadAll<UtilityData>("Data/Parts/Utilities")
                .Where(u => (int)u.minimumSize <= (int)_hull.size)
                .ToArray();

        for (int i = 0; i < _weapons.Length; i++)
        {
            if (_weapons[i] == null)
                _weapons[i] = weapons.RandomItem().Instantiate() as Weapon;

            _components[i] = _weapons[i];
        }
        for (int i = 0; i < _utilities.Length; i++)
        {
            if (_utilities[i] == null)
                _utilities[i] = utilities.RandomItem().Instantiate() as Utility;

            _components[_weapons.Length + i] = _utilities[i];
        }

        UpdateStats();
        UpdateResourceScanner();
    }

    void UpdateStats()
    {
        //initialize base values
        float movementSpeed = _hull.baseMovementSpeed;
        float rotationSpeed = _hull.baseRotationSpeed;

        //base scanRadius, basically optical range
        float scanRadius = 200;
        float scanRate = 5f;

        float hullpoints = _hull.hullpoints;
        float shieldpoints = 0f;

        float gasStorage = _hull.gasStorage;
        float oreStorage = _hull.oreStorage;
        float fuelStorage = _hull.fuelStorage;
        float goodsStorage = _hull.goodsStorage;

        _vitals = new float[] 
        {
            movementSpeed,
            rotationSpeed,
            scanRadius,
            scanRate,
            hullpoints,
            shieldpoints,
            gasStorage, 
            oreStorage,
            fuelStorage,
            goodsStorage
        };

        //apply additive modifiers
        for (int i = 0; i < _components.Length; i++)
        {
            for (int j = 0; j < _components[i].modifiers.Length; j++)
            {
                if(_components[i].modifiers[j].mode == ModifierMode.Additive)
                    _vitals[(int)_components[i].modifiers[j].type] += _components[i].modifiers[j].value;
            }
        }
        //apply percentage buffs
        for (int i = 0; i < _components.Length; i++)
        {
            for (int j = 0; j < _components[i].modifiers.Length; j++)
                if(_components[i].modifiers[j].mode == ModifierMode.Percentage)
                    _vitals[(int)_components[i].modifiers[j].type] *= _components[i].modifiers[j].value;
        }
        //sanity checking, this needs expanding
        for (int i = 0; i < _vitals.Length; i++)
        {
            if (_vitals[i] < 0f)
                _vitals[i] = 0f;
        }

        OnStatsUpdated?.Invoke();
    }
    void UpdateResourceScanner()
    {
        _scanner = null;

        for (int i = 0; i < _utilities.Length; i++)
        {
            if (_utilities[i] is ResourceScanner)
            {
                ResourceScanner scanner = (ResourceScanner)_utilities[i];

                if (_scanner == null || scanner.range > _scanner.range)
                    _scanner = scanner;
            }
        }
    }

    public ShipEntity Instantiate(Vector3 position, Quaternion rotation, int teamID, bool isDiscovered)
    {
        GameObject s = Object.Instantiate(_hull.model, position, rotation, null);
        ShipEntity entity = s.GetComponent<ShipEntity>();
        entity.Initialize(this, teamID, isDiscovered);

        return entity;
    }

    public void SetName(string newName)
    {
        _name = newName;
    }
    public void AddModifier(FleetVitalModifier fvm)
    {
        _modifiers.Add(fvm);

        LogManager.getInstance.AddEntry("<i><color=yellow>" + _name + "</color></i> has gained a modifier: [" + fvm.value + " " + FleetVital.Format(fvm.type) + " due to " + fvm.reason + " for " + fvm.duration + (fvm.isInfinite ? "∞" : " days") + ".]");
        OnModifierAdded?.Invoke(fvm);
    }
    public void RemoveModifier(FleetVitalModifier fvm)
    {
        _modifiers.Remove(fvm);

        LogManager.getInstance.AddEntry("<i><color=yellow>" + _name + "</color></i> has lost a modifier: [" + fvm.value + " " + FleetVital.Format(fvm.type) + " due to " + fvm.reason + ".]");
        OnModifierRemoved?.Invoke(fvm);
    }
    public void AssignOfficer(Officer o)
    {
        _officer = o;

        if (o != null)
            o.Assign(this);
    }

    public void TickModifiers()
    {
        for (int i = 0; i < _modifiers.Count; i++)
        {
            if (!_modifiers[i].isInfinite)
            {
                _modifiers[i].Tick();

                if (_modifiers[i].isComplete)
                    RemoveModifier(_modifiers[i]);
            }
        }
    }
    public void TickConverters()
    {
        for (int i = 0; i < _utilities.Length; i++)
            if(_utilities[i] is ResourceConversionUtility)
                ((ResourceConversionUtility)_utilities[i]).Tick();
    }
    public float GetVital(VitalType vt)
    {
        return _vitals[(int)vt];
    }

    public string GetClass()
    {
        return _hull.name + "-class " + _size.ToString();
    }
    public string GetDescription()
    {
        return _hull.description;
    }
    public string ListModifiers(FleetVitalType type)
    {
        string s = "";

        s += "[<i><color=yellow>" + _name + "</color></i>]:\n";

        for (int i = 0; i < _modifiers.Count; i++)
        {
            float v = _modifiers[i].value;

            if(_modifiers[i].type == type)
                s += "<color=" + (v == 0 ? "yellow" : v > 0 ? "green" : "red") + ">" + v.ToString("0.##") + "</color>" + (_modifiers[i].setting == ModifierSetting.Repeating ? " daily" : "") + ", due to: " + _modifiers[i].reason + ", " + (_modifiers[i].isInfinite ? "∞\n" : _modifiers[i].duration + " days.\n");
        }
        for (int i = 0; i < _utilities.Length; i++)
        {
            if(_utilities[i] is ResourceConversionUtility)
            {
                ResourceConversionUtility rcu = ((ResourceConversionUtility)_utilities[i]);

                for (int j = 0; j < rcu.conversions.Length; j++)
                {
                    if(rcu.conversions[j].start == type)
                        s += "<color=red>-" + rcu.conversions[j].maxConversionPerTurn.ToString("0.##") + "</color> daily (if possible), due to: [<color=yellow>" + rcu.name + "</color>].\n";
                    else if(rcu.conversions[j].end == type)
                        s += "<color=green>+" + (rcu.conversions[j].maxConversionPerTurn * rcu.conversions[j].rate).ToString("0.##") + "</color> daily (if possible), due to: [<color=yellow>" + rcu.name + "</color>].\n";
                }
            }
        }

        return s;
    }

    public delegate void StatsChangedEvent();
    public StatsChangedEvent OnStatsUpdated;

    public delegate void ModifierEvent(FleetVitalModifier fvm);
    public ModifierEvent OnModifierAdded;
    public ModifierEvent OnModifierRemoved;
}
public class ShipLog
{
    int _kills = 0;
    List<ShipLogEntry> _entries = new List<ShipLogEntry>();

    public int kills { get { return _kills; } }
    public List<ShipLogEntry> entries { get { return _entries; } }

    public void AddKill()
    {
        _kills++;
    }
    public void AddEntry(string title, string date, string body)
    {
        _entries.Add(new ShipLogEntry(title, date, body));
    }
}
public struct ShipLogEntry
{
    public readonly string title;
    public readonly string date;
    public readonly string body;

    public ShipLogEntry(string title, string date, string body)
    {
        this.title = title;
        this.date = date;
        this.body = body;
    }
}
public enum ShipAllegiance
{
    Military,
    Civilian
}