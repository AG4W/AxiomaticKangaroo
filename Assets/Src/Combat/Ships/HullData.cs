using UnityEngine;

[CreateAssetMenu(menuName = "Data/Parts/Hull")]
public class HullData : ScriptableObject
{
    [TextArea(3, 10)][SerializeField]string _description;

    [Header("Vital Specs")]
    [SerializeField]int _weaponSlots;
    [SerializeField]int _utilitySlots;

    [SerializeField]float _hitpoints;
    [SerializeField]float _movementSpeed;
    [SerializeField]float _rotationSpeed;

    [Header("Storage")]
    [SerializeField]float _gasStorage;
    [SerializeField]float _oreStorage;
    [SerializeField]float _fuelStorage;
    [SerializeField]float _goodsStorage;

    [SerializeField]FleetVitalModifier[] _maintenanceCosts;

    [SerializeField]HullClass _size;

    [Header("Visuals")]
    [SerializeField]GameObject _model;
    [SerializeField]GameObject _destroyedVFX;

    [SerializeField]Sprite _icon;

    public string description { get { return _description; } }

    public int weaponSlots { get { return _weaponSlots; } }
    public int utilitySlots { get { return _utilitySlots; } }

    public float hitpoints { get { return _hitpoints; } }
    public float movementSpeed { get { return _movementSpeed; } }
    public float rotationSpeed { get { return _rotationSpeed; } }

    public float gasStorage { get { return _gasStorage; } }
    public float oreStorage { get { return _oreStorage; } }
    public float fuelStorage { get { return _fuelStorage; } }
    public float goodsStorage { get { return _goodsStorage; } }

    public FleetVitalModifier[] maintenanceCosts { get { return _maintenanceCosts; } }

    public HullClass size { get { return _size; } }

    public GameObject model { get { return _model; } }
    public GameObject destroyedVFX { get { return _destroyedVFX; } }

    public Sprite icon { get { return _icon; } }

    public Hull CreateInstance()
    {
        return new Hull(this);
    }
}
public class Hull
{
    string _name;
    string _description;

    int _weaponSlots;
    int _utilitySlots;

    float _hullpoints;
    float _baseMovementSpeed;
    float _baseRotationSpeed;

    float _gasStorage;
    float _oreStorage;
    float _fuelStorage;
    float _goodsStorage;

    FleetVitalModifier[] _maintenanceCosts;

    HullClass _size;

    GameObject _model;
    GameObject _destroyedVFX;
    Sprite _icon;

    public string name { get { return _name; } }
    public string description { get { return _description; } }

    public int weaponSlots { get { return _weaponSlots; } }
    public int utilitySlots { get { return _utilitySlots; } }
    
    public float hullpoints { get { return _hullpoints; } }
    public float baseMovementSpeed { get { return _baseMovementSpeed; } }
    public float baseRotationSpeed { get { return _baseRotationSpeed; } }

    public float gasStorage { get { return _gasStorage; } }
    public float oreStorage { get { return _oreStorage; } }
    public float fuelStorage { get { return _fuelStorage; } }
    public float goodsStorage { get { return _goodsStorage; } }

    public FleetVitalModifier[] maintenanceCosts { get { return _maintenanceCosts; } }

    public HullClass size { get { return _size; } }

    public GameObject model { get { return _model; } }
    public GameObject destroyedVFX { get { return _destroyedVFX; } }
    public Sprite icon { get { return _icon; } }

    public Hull(HullData hd)
    {
        _name = hd.name;
        _description = hd.description;

        _weaponSlots = hd.weaponSlots;
        _utilitySlots = hd.utilitySlots;

        _hullpoints = hd.hitpoints;
        _baseMovementSpeed = hd.movementSpeed;
        _baseRotationSpeed = hd.rotationSpeed;

        _gasStorage = hd.gasStorage;
        _oreStorage = hd.oreStorage;
        _fuelStorage = hd.fuelStorage;
        _goodsStorage = hd.goodsStorage;

        _maintenanceCosts = hd.maintenanceCosts;

        _size = hd.size;

        _model = hd.model;
        _destroyedVFX = hd.destroyedVFX;
        _icon = hd.icon;
    }
}
public enum HullClass
{
    Frigate,
    Destroyer,
    Cruiser,
    Battleship,
    Tanker,
    FactoryShip,
    Carrier,
    Titan
}