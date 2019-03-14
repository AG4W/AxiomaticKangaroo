using UnityEngine;

using System.Linq;

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

    float _acceleration;

    Weapon[] _weapons;
    Utility[] _utilities;
    ShipComponent[] _components;

    //components
    Hull _hull;
    HullClass _size;

    //captain
    Officer _officer;

    //misc
    GameObject _model;
    GameObject _destroyedVFX;

    public string name { get { return _name; } }

    public float acceleration { get { return _acceleration; } }

    public Weapon[] weapons { get { return _weapons; } }
    public Utility[] utilities { get { return _utilities; } }
    public ShipComponent[] components { get { return _components; } }

    public HullClass size { get { return _size; } }

    public Officer officer { get { return _officer; } }

    public GameObject model { get { return _model; } }
    public GameObject destroyedVFX { get { return _destroyedVFX; } }

    public Ship(ShipData sd)
    {
        _name = sd.name.Length == 0 ? NameGenerator.GetShipName(sd.hull.size) : sd.name;
        _hull = sd.hull.CreateInstance() as Hull;
        _size = _hull.size;

        _acceleration = _hull.acceleration;

        _weapons = new Weapon[_hull.weaponSlots];
        _utilities = new Utility[_hull.utilitySlots];
        _components = new ShipComponent[_hull.weaponSlots + _hull.utilitySlots];

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

        if(_officer != null)
        {
            //additive
            for (int i = 0; i < _officer.traits.Count; i++)
            {
                for (int j = 0; j < _officer.traits[i].modifiers.Length; j++)
                {
                    if (_officer.traits[i].modifiers[j].mode == ModifierMode.Additive)
                        _vitals[(int)_officer.traits[i].modifiers[j].type] += _officer.traits[i].modifiers[j].value;
                }
            }
            //percentage
            for (int i = 0; i < _officer.traits.Count; i++)
            {
                for (int j = 0; j < _officer.traits[i].modifiers.Length; j++)
                    if (_officer.traits[i].modifiers[j].mode == ModifierMode.Percentage)
                        _vitals[(int)_officer.traits[i].modifiers[j].type] *= _officer.traits[i].modifiers[j].value;
            }
        }

        //sanity checking, this needs expanding
        for (int i = 0; i < _vitals.Length; i++)
        {
            if (_vitals[i] < 0f)
                _vitals[i] = 0f;
        }

        OnStatsUpdated?.Invoke();
    }

    public ShipEntity Instantiate(Vector3 position, Quaternion rotation, int teamID, bool isDiscovered)
    {
        GameObject s = Object.Instantiate(_hull.model, position, rotation, null);
        ShipEntity entity = s.GetComponent<ShipEntity>();
        entity.Initialize(this, teamID, isDiscovered);

        return entity;
    }

    public void SetName(string n)
    {
        _name = n;
    }
    public void AssignOfficer(Officer o)
    {
        _officer = o;

        if (o != null)
            o.Assign(this);

        UpdateStats();
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

    public delegate void StatEvent();
    public StatEvent OnStatsUpdated;

    public delegate void ModifierEvent(FleetVitalModifier fvm);
    public ModifierEvent OnModifierAdded;
    public ModifierEvent OnModifierRemoved;
}