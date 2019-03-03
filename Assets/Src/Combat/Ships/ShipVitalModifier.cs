using UnityEngine;

[System.Serializable]
public class ShipVitalModifier : Modifier
{
    [SerializeField]VitalType _type;

    public VitalType type { get { return _type; } }

    public ShipVitalModifier(string semantic, float value, ModifierMode mode, VitalType type) : base(semantic, value, mode)
    {
        _type = type;
    }

    public override string ToString()
    {
        return base.GetValueFormatted() + " " + ShipVital.Format(_type);
    }
}
