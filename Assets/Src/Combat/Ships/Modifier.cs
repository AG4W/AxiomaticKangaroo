using UnityEngine;

[System.Serializable]
public class Modifier
{
    [SerializeField]string _semantic;
    [SerializeField]float _value;

    [SerializeField]ModifierMode _mode;

    public string semantic { get { return _semantic; } }
    public float value { get { return _value; } }

    public ModifierMode mode { get { return _mode; } }

    public Modifier(string semantic, float value, ModifierMode mode)
    {
        _semantic = semantic;
        _value = value;

        _mode = mode;
    }

    public string GetValueFormatted()
    {
        string s = "";

        s += "<color=" + (_value == 0f ? "yellow" : _value > 0f ? "green" : "red") + ">";
        s += (_mode == ModifierMode.Percentage ? (_value * 100f).ToString("+0.##;-0.##") : _value.ToString("+0.##;-0.##"));
        s += "</color>";

        if (_mode == ModifierMode.Percentage)
            s += "%";

        return s;
    }
}
public enum ModifierMode
{
    Additive,
    Percentage
}