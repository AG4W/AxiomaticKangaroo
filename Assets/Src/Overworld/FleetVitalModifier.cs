using UnityEngine;

[System.Serializable]
public class FleetVitalModifier : Modifier
{
    [SerializeField]string _reason;
    [SerializeField]int _duration;

    [SerializeField]FleetVitalType _type;
    [SerializeField]ModifierSetting _setting;

    public string reason { get { return _reason; } }
    public int duration { get { return _duration; } }

    public FleetVitalType type { get { return _type; } }
    public ModifierSetting setting { get { return _setting; } }

    public bool isInfinite { get; private set; }
    public bool isComplete { get { return _duration == 0; } }

    public FleetVitalModifier(string reason, int duration, float value, FleetVitalType type, ModifierSetting setting, ModifierMode mode, ModifierTarget target) : base(value, mode, target)
    {
        _reason = reason;
        _duration = duration;

        _type = type;
        _setting = setting;

        isInfinite = duration < 0;
    }

    public void Tick()
    {
        if(!isComplete)
            _duration--;
    }

    public string GetTooltip(bool includeReason = true)
    {
        string s = "";

        s += "<color=" + (base.value > 0 ? "green" : "red") + ">" + (base.mode == ModifierMode.Percentage ? base.value * 100f : base.value).ToString("+0.##;-0.##") + "</color>";
        s += (base.mode == ModifierMode.Percentage ? "%" : "") + " ";
        s += FleetVital.Format(_type) + (_setting == ModifierSetting.Repeating ? " daily" : "");

        if (includeReason)
            s += " due to " + _reason;

        return s;
    }
}

