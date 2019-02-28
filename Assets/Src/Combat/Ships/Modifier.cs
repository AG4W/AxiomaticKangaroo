using UnityEngine;

[System.Serializable]
public class Modifier
{
    [SerializeField]float _value;

    [SerializeField]ModifierMode _mode;
    [SerializeField]ModifierTarget _target;

    public float value { get { return _value; } }

    public ModifierMode mode { get { return _mode; } }
    public ModifierTarget target { get { return _target; } }

    public Modifier(float value, ModifierMode mode, ModifierTarget target)
    {
        _value = value;

        _mode = mode;
        _target = target;
    }
}
public enum ModifierMode
{
    Additive,
    Percentage
}
public enum ModifierTarget
{
    Current,
    Max,
}
public enum ModifierSetting
{
    Repeating,
    Static
}
