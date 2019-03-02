using UnityEngine;

[CreateAssetMenu(menuName = "Data/Parts/Utilities/Utility")]
public class UtilityData : ShipComponentData
{
    [SerializeField]bool _canBeActivated = false;
    public bool canBeActivated { get { return _canBeActivated; } }

    public override ShipComponent Instantiate()
    {
        return new Utility(this);
    }
}
public class Utility : ShipComponent
{
    bool _canBeActivated;

    public Utility(UtilityData ud) : base(ud)
    {
        base.type = ShipComponentType.Utility;

        _canBeActivated = ud.canBeActivated;
    }

    public virtual void AttemptActivate()
    {
        if (!base.hasCooldown && _canBeActivated)
            OnActivation();
    }
    protected virtual void OnActivation()
    {
        base.StartCooldown();
    }

    public override string GetSummary()
    {
        string s = base.GetSummary();

        s += "\n\n";
        s += "Mode: " + (_canBeActivated ? "Active." : "Passive.");

        if(_canBeActivated)
            s += "\nCooldown: " + base.cooldown.ToString("0.##");

        return s;
    }
}