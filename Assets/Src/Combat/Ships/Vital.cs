public class Vital
{
    float _maxValue;
    float _currentValue;

    VitalType _type;

    public float max { get { return _maxValue; } }
    public float current { get { return _currentValue; } }
    public float inPercent { get { return _currentValue / _maxValue; } }

    public VitalType type { get { return _type; } }

    public Vital(float maxValue, VitalType type)
    {
        _maxValue = maxValue;
        _currentValue = maxValue;
        _type = type;
    }

    public void Update(float amount)
    {
        _currentValue += amount;

        OnCurrentChanged();
    }
    public void Set(float value)
    {
        _currentValue = value;

        OnCurrentChanged();
    }
    void OnCurrentChanged()
    {
        if (_currentValue < 0)
            _currentValue = 0;
        else if (_currentValue > _maxValue)
            _currentValue = _maxValue;

        if (OnVitalChanged != null)
            OnVitalChanged(_type, inPercent);
    }

    public static string Format(VitalType type)
    {
        switch (type)
        {
            case VitalType.MovementSpeed:
                return "Movement Speed";
            case VitalType.RotationSpeed:
                return "Rotational Speed";
            case VitalType.ScanRadius:
                return "Scan Range";
            case VitalType.ScanRate:
                return "Scan Rate";
            case VitalType.HullPoints:
                return "Hull Hitpoints";
            case VitalType.ShieldPoints:
                return "Shield Hitpoints";
            case VitalType.GasStorage:
                return "Gas Storage";
            case VitalType.OreStorage:
                return "Ore Storage";
            case VitalType.FuelStorage:
                return "Fuel Storage";
            case VitalType.GoodsStorage:
                return "Goods Storage";
            default:
                return "No format specified for " + type.ToString();
        }
    }
    public static string FormatTooltip(VitalType type)
    {
        switch (type)
        {
            case VitalType.MovementSpeed:
                return "Forward movement speed.";
            case VitalType.RotationSpeed:
                return "Rotational movement speed.";
            case VitalType.ScanRadius:
                return "Range of proximity scan.";
            case VitalType.ScanRate:
                return "Update rate for proximity scan.";
            case VitalType.HullPoints:
                return "Hull hitpoints, only damaged after shield reaches zero.";
            case VitalType.ShieldPoints:
                return "Shield hitpoints.";
            case VitalType.GasStorage:
                return "Contributes to the fleet maximum cap of " + FleetVital.Format(FleetVitalType.NebulaGas) + ".";
            case VitalType.OreStorage:
                return "Contributes to the fleet maximum cap of  " + FleetVital.Format(FleetVitalType.Veldspar) + " and " + FleetVital.Format(FleetVitalType.Tritanite) + ".";
            case VitalType.FuelStorage:
                return "Contributes to the fleet maximum cap of  " + FleetVital.Format(FleetVitalType.ProcessedFuel) + ".";
            case VitalType.GoodsStorage:
                return "Contributes to the fleet maximum cap of  " + FleetVital.Format(FleetVitalType.Ammunition) + " and " + FleetVital.Format(FleetVitalType.CivilianSupplies) + ".";
            default:
                return "No tooltipformat for " + type.ToString();
        }
    }

    public delegate void VitalChangedEvent(VitalType vt, float inPercent);
    public event VitalChangedEvent OnVitalChanged;
}
public enum VitalType
{
    MovementSpeed,
    RotationSpeed,
    ScanRadius,
    ScanRate,
    HullPoints,
    ShieldPoints,
    GasStorage,
    OreStorage, //veldspar & tritanite
    FuelStorage,
    GoodsStorage, //ammo & supplies
}
