using UnityEngine;

[CreateAssetMenu(menuName = "Data/Parts/Utilities/Resource Converter")]
public class ResourceConversionUtilityData : UtilityData
{
    [SerializeField]ResourceConversion[] _conversions;
    public ResourceConversion[] conversions { get { return _conversions; } }

    public override ShipComponent Instantiate()
    {
        return new ResourceConversionUtility(this);
    }
}
public class ResourceConversionUtility : Utility
{
    ResourceConversion[] _conversions;
    public ResourceConversion[] conversions { get { return _conversions; } }

    public ResourceConversionUtility(ResourceConversionUtilityData rcud) : base(rcud)
    {
        _conversions = rcud.conversions;
    }

    public void Tick()
    {
        for (int i = 0; i < _conversions.Length; i++)
        {
            if (PlayerData.fleet.GetVital(_conversions[i].start).current > 0f)
            {
                float amount;

                if (_conversions[i].maxConversionPerTurn > PlayerData.fleet.GetVital(_conversions[i].start).current)
                    amount = PlayerData.fleet.GetVital(_conversions[i].start).current;
                else
                    amount = _conversions[i].maxConversionPerTurn;

                PlayerData.fleet.GetVital(_conversions[i].start).Update(-amount);
                PlayerData.fleet.GetVital(_conversions[i].end).Update(amount * _conversions[i].rate);
                LogManager.getInstance.AddEntry(amount.ToString("0.##") + " units of " + FleetVital.Format(_conversions[i].start) + " has been converted to " + (amount * _conversions[i].rate).ToString("0.##") + " units of " + FleetVital.Format(_conversions[i].end) + ".");
            }
        }
    }
}
[System.Serializable]
public struct ResourceConversion
{
    [SerializeField]float _maxConversionPerTurn;
    [SerializeField]float _rate;

    [SerializeField]FleetVitalType _start;
    [SerializeField]FleetVitalType _end;

    public float maxConversionPerTurn { get { return _maxConversionPerTurn; } }
    public float rate { get { return _rate; } }

    public FleetVitalType start { get { return _start; } }
    public FleetVitalType end { get { return _end; } }

    public ResourceConversion(float maxConversionPerTurn, float rate, FleetVitalType start, FleetVitalType end)
    {
        _maxConversionPerTurn = maxConversionPerTurn;
        _rate = rate;

        _start = start;
        _end = end;
    }
}