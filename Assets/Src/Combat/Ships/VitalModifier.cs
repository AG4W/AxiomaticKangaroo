using UnityEngine;

[System.Serializable]
public class VitalModifier : Modifier
{
    [SerializeField]VitalType _type;
    public VitalType type { get { return _type; } }

    public VitalModifier(VitalType type, float value, ModifierMode mode, ModifierTarget target) : base(value, mode, target)
    {
        _type = type;
    }
}
