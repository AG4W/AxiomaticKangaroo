using UnityEngine;

[System.Serializable]
public class FleetVitalModifier : Modifier
{
    [SerializeField]FleetVitalType _type;

    public FleetVitalType type { get { return _type; } }

    public FleetVitalModifier(string semantic, float value, ModifierMode mode, FleetVitalType type) : base(semantic, value, mode)
    {
        _type = type;
    }
}
