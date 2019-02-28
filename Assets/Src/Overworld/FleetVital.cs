using UnityEngine;

using System.Collections.Generic;
using System.Linq;

public class FleetVital
{
    float _max;
    float _current;

    float _staticCurrentModSum = 0f;
    float _staticMaxModSum = 0f;

    float _changePerTurn = 0f;

    FleetVitalType _type;

    List<FleetVitalModifier> _modifiers = new List<FleetVitalModifier>();

    public float max
    {
        get
        {
            float s = _max + _staticMaxModSum;

            if (s < 0)
                return 0f;
            else
                return s;
        }
    }
    public float current
    {
        get
        {
            float s = _current + _staticCurrentModSum;

            if (s < 0)
                return 0f;
            else if (s > max)
                return max;
            else
                return s;
        }
    }
    public float changePerTurn { get { return _changePerTurn; } }
    public float inPercent { get { return current / max; } }

    public FleetVitalType type { get { return _type; } }

    public FleetVital(FleetVitalType type, float max)
    {
        _max = max;
        _current = max;

        _changePerTurn = 0f;
        _staticCurrentModSum = 0f;
        _staticMaxModSum = 0f;

        _type = type;
    }
    public FleetVital(FleetVitalType type, float current, float max)
    {
        _max = max;

        if (current < 0)
            _current = 0;
        else if (current > max)
            _current = max;
        else
            _current = current;

        _changePerTurn = 0f;
        _type = type;
    }

    public void Update(float amount)
    {
        _current += amount;

        if (_current < 0)
            _current = 0;
        else if (_current > _max)
            _current = _max;

        UpdateModifiers();
        OnCurrentChanged?.Invoke(this);

        if (_current <= 0f)
            OnCurrentCritical?.Invoke(this);
    }
    public void SetMax(float value, bool setCurrent = false)
    {
        _max = value;

        if (_max < 0)
            _max = 0;

        if (setCurrent)
            _current = _max;

        UpdateModifiers();
        OnMaxChanged?.Invoke(this);
    }

    public void AddModifier(FleetVitalModifier modifier)
    {
        _modifiers.Add(modifier);

        UpdateModifiers();
    }
    public void RemoveModifier(FleetVitalModifier modifier)
    {
        _modifiers.Remove(modifier);

        UpdateModifiers();
    }

    void UpdateModifiers()
    {
        _changePerTurn = 0f;
        _staticCurrentModSum = 0f;
        _staticMaxModSum = 0f;

        //additive first
        FleetVitalModifier[] additive = _modifiers.Where(m => m.mode == ModifierMode.Additive).ToArray();
        for (int i = 0; i < additive.Length; i++)
        {
            if (additive[i].setting == ModifierSetting.Repeating)
                _changePerTurn += additive[i].value;
            else
            {
                if (additive[i].target == ModifierTarget.Current)
                    _staticCurrentModSum += additive[i].value;
                else
                    _staticMaxModSum += additive[i].value;
            }
        }
        FleetVitalModifier[] percentage = _modifiers.Where(m => m.mode == ModifierMode.Percentage).ToArray();
        for (int i = 0; i < percentage.Length; i++)
        {
            if (percentage[i].setting == ModifierSetting.Repeating)
                _changePerTurn += (_changePerTurn * percentage[i].value);
            else
            {
                if (percentage[i].target == ModifierTarget.Current)
                    _staticCurrentModSum += (_current * percentage[i].value);
                else
                    _staticMaxModSum += (_max * percentage[i].value);
            }
        }

        OnVitalUpdated?.Invoke(this);
    }
    
    public string GetTooltip()
    {
        return current.ToString("0.##") + "<color=white> / </color>" + max.ToString("0");
    }
    public string GetTooltipExtended()
    {
        string s = "";

        if(_modifiers.Count > 0)
        {
            s += GetTooltip() + "\n\n";

            for (int i = 0; i < _modifiers.Count; i++)
                s += _modifiers[i].GetTooltip(true) + (i == _modifiers.Count - 1 ? "." : ".\n");
        }
        
        return s;
    }

    public delegate void FleetVitalEvent(FleetVital vital);
    public event FleetVitalEvent OnCurrentChanged;
    public event FleetVitalEvent OnMaxChanged;
    public event FleetVitalEvent OnCurrentCritical;
    public event FleetVitalEvent OnVitalUpdated;

    public static string Format(FleetVitalType type)
    {
        string f = "<color=#" + ColorUtility.ToHtmlStringRGB(Color(type)) + ">";

        switch (type)
        {
            case FleetVitalType.ProcessedFuel:
                f += "Processed Fuel";
                break;
            case FleetVitalType.Ammunition:
                f += "Ammunition";
                break;
            case FleetVitalType.CivilianSupplies:
                f += "Civilian Supplies";
                break;
            case FleetVitalType.NebulaGas:
                f += "Nebula Gas";
                break;
            case FleetVitalType.Veldspar:
                f += "Veldspar";
                break;
            case FleetVitalType.Tritanite:
                f += "Tritanite";
                break;
            case FleetVitalType.Movement:
                f += "Movement Range";
                break;
            case FleetVitalType.Detection:
                f += "Detection Range";
                break;
            default:
                break;
        }

        f += "</color>";

        return f;
    }
    public static Color Color(FleetVitalType type)
    {
        switch (type)
        {
            case FleetVitalType.ProcessedFuel:
                return new Color(.5f, 0f, 1f);
            case FleetVitalType.Ammunition:
                return new Color(1f, .5f, 0f);
            case FleetVitalType.CivilianSupplies:
                return new Color(.5f, .5f, 0f);
            case FleetVitalType.NebulaGas:
                return new Color(0f, 1f, 1f);
            case FleetVitalType.Veldspar:
                return new Color(1f, 0f, 0f);
            case FleetVitalType.Tritanite:
                return new Color(1f, 0f, 1f);
            case FleetVitalType.Movement:
                return new Color(.5f, .5f, 1f);
            case FleetVitalType.Detection:
                return new Color(1f, 0f, 0f);
            default:
                return new Color(1f, 1f, 1f);
        }
    }
}
public enum FleetVitalType
{
    ProcessedFuel,
    Ammunition,
    CivilianSupplies,
    NebulaGas,
    Veldspar,
    Tritanite,
    Movement,
    Detection
}
