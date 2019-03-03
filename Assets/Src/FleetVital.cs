using UnityEngine;

public class FleetVital : Vital
{
    FleetVitalType _type;

    public FleetVitalType type { get { return _type; } }

    public FleetVital(float maxValue, FleetVitalType type) : base(maxValue)
    {
        _type = type;
    }

    public static string Format(FleetVitalType type)
    {
        string color = ColorUtility.ToHtmlStringRGB(Color(type));

        switch (type)
        {
            case FleetVitalType.ProcessedFuel:
                return "<color=#" + color + ">Processed Fuel</color>";
            case FleetVitalType.Ammunition:
                return "<color=#" + color + ">Ammunition</color>";
            case FleetVitalType.CivilianGoods:
                return "<color=#" + color + ">Civilian Goods</color>";
            case FleetVitalType.NebulaGas:
                return "<color=#" + color + ">Nebula Gas</color>";
            case FleetVitalType.Tritanite:
                return "<color=#" + color + ">Tritanite</color>";
            case FleetVitalType.Veldspar:
                return "<color=#" + color + ">Veldspar</color>";
            case FleetVitalType.Range:
                return "<color=#" + color + ">Range</color>";
            default:
                return "Invalid text";
        }
    }
    public static Color Color(FleetVitalType type)
    {
        switch (type)
        {
            case FleetVitalType.ProcessedFuel:
                return new Color(.5f, 0f, 1f);
            case FleetVitalType.Ammunition:
                return new Color(1f, .5f, 0f);
            case FleetVitalType.CivilianGoods:
                return new Color(.5f, 1f, 0f);
            case FleetVitalType.NebulaGas:
                return new Color(0f, 1f, 1f);
            case FleetVitalType.Tritanite:
                return new Color(1f, 0f, 1f);
            case FleetVitalType.Veldspar:
                return new Color(1f, 0f, 0f);
            case FleetVitalType.Range:
                return new Color(.5f, 0f, 1f);
            default:
                return new Color(0f, 0f, 0f);
        }
    }
}
public enum FleetVitalType
{
    ProcessedFuel,
    Ammunition,
    CivilianGoods,
    NebulaGas,
    Tritanite,
    Veldspar,
    Range
}
