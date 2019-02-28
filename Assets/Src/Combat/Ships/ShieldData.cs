using UnityEngine;

using System.Collections;

[CreateAssetMenu(menuName = "Data/Parts/Utilities/Shield")]
public class ShieldData : UtilityData
{
    [SerializeField]float _amount;
    [SerializeField]float _rate;
    [SerializeField]float _duration;

    public float amount { get { return _amount; } }
    public float rate { get { return _rate; } }
    public float duration { get { return _duration; } }

    public override ShipComponent Instantiate()
    {
        return new Shield(this);
    }
}
public class Shield : Utility
{
    float _amount;
    float _rate;
    float _duration;

    public Shield(ShieldData sd) : base(sd)
    {
        _amount = sd.amount;
        _rate = sd.rate;
        _duration = sd.duration;
    }
    protected override void OnActivation()
    {
        base.owner.StartCoroutine(Regenerate());
        //starts cooldown
        base.OnActivation();
    }

    IEnumerator Regenerate()
    {
        float t = 0f;

        while (t <= _duration)
        {
            t += _rate;
            base.owner.GetVital(VitalType.ShieldPoints).Update(_amount);
            yield return new WaitForSeconds(_rate);
        }
    }

    public override string GetSummary()
    {
        string s = base.GetSummary();

        s += "\n\n";
        s += "Regeneration Amount: " + _amount + "\n";
        s += "Regeneration Rate: " + _rate + "\n";
        s += "Duration: " + _duration;

        return s;
    }
}
