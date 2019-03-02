using UnityEngine;

public class ShipComponentData : ScriptableObject
{
    [TextArea(3, 10)][SerializeField]string _description;

    [SerializeField]float _cooldown = 0f;

    [SerializeField]VitalModifier[] _modifiers;

    [SerializeField]ShipComponentRarity _rarity;
    [SerializeField]HullClass _minimumSize = HullClass.Frigate;
    [SerializeField]Sprite _icon;

    [SerializeField]bool _autoActivateEnabledByDefault = false;

    public string description { get { return _description; } }

    public float cooldown { get { return _cooldown; } }

    public VitalModifier[] modifiers { get { return _modifiers; } }

    public ShipComponentRarity rarity { get { return _rarity; } }
    public HullClass minimumSize { get { return _minimumSize; } }
    public Sprite icon { get { return _icon; } }

    public bool autoActivateEnabledByDefault { get { return _autoActivateEnabledByDefault; } }

    public virtual ShipComponent Instantiate()
    {
        return new ShipComponent(this);
    }
    public virtual ShipComponent Instantiate(ShipComponentRarity rarity)
    {
        return new ShipComponent(this, rarity);
    }
}
public class ShipComponent
{
    string _name;
    string _description;

    float _cooldown;
    float _cooldownTimestamp;

    VitalModifier[] _modifiers;

    ShipComponentType _type;
    ShipComponentRarity _rarity;

    HullClass _minimumSize;
    Sprite _icon;

    ShipEntity _owner;
    Visualizer _visualizer;

    bool _autoActivate;
    bool _hasCooldown;

    public string name { get { return _name; } }
    public string description { get { return _description; } }

    public float cooldown { get { return _cooldown; } }
    public float cooldownRemaining { get { return _hasCooldown ? Mathf.Abs(Time.time - _cooldownTimestamp) : 0f; } }
    public float cooldownRemainingInPercent { get { return _hasCooldown ? (1 - (Mathf.Abs(Time.time - _cooldownTimestamp) / _cooldown)) : 0f; } }

    public VitalModifier[] modifiers { get { return _modifiers; } }

    public ShipComponentType type { get { return _type; } protected set { _type = value; } }
    public ShipComponentRarity rarity { get { return _rarity; } }

    public HullClass minimumSize { get { return _minimumSize; } }
    public Sprite icon { get { return _icon; } }

    protected ShipEntity owner { get { return _owner; } }

    public bool autoActivate { get { return _autoActivate; } }
    public bool hasCooldown { get { return _hasCooldown; } }

    public ShipComponent(ShipComponentData scd)
    {
        _name = scd.name;
        _description = scd.description;

        _cooldown = scd.cooldown;

        _modifiers = scd.modifiers;

        _rarity = scd.rarity;
        _minimumSize = scd.minimumSize;
        _icon = scd.icon;

        _autoActivate = scd.autoActivateEnabledByDefault;
    }
    public ShipComponent(ShipComponentData scd, ShipComponentRarity rarity)
    {
        _name = scd.name;
        _description = scd.description;

        _cooldown = scd.cooldown;

        _modifiers = scd.modifiers;

        _rarity = rarity;
        _minimumSize = scd.minimumSize;
        _icon = scd.icon;

        _autoActivate = scd.autoActivateEnabledByDefault;
    }

    public void Initialize(ShipEntity owner)
    {
        _owner = owner;
        _visualizer = owner.GetComponentInChildren<Visualizer>();

        StartCooldown();
    }
    public void ToggleAutoActivate()
    {
        _autoActivate = !_autoActivate;
    }

    protected void StartCooldown()
    {
        _hasCooldown = true;
        //save start time
        _cooldownTimestamp = Time.time;
        //start timer
        MasterTimer.getInstance.Request(_cooldown, OnCooldownComplete);
    }
    void OnCooldownComplete()
    {
        _cooldownTimestamp = 0f;
        _hasCooldown = false;
    }

    public virtual void OnTooltipEnter()
    {

    }
    public virtual void OnTooltipExit()
    {

    }

    public virtual string GetSummary()
    {
        string s = "";

        s += "<color=#" + ColorUtility.ToHtmlStringRGB(ColorByRarity()) + ">(" + _rarity + " " + _type + ")</color>\n\n";
        s += _description;

        return s;
    }
    public virtual void DrawVisualization()
    {
        
    }

    public string Format()
    {
        return "<color=#" + ColorUtility.ToHtmlStringRGB(ColorByRarity()) + ">" + _name + "</color>";
    }
    public Color ColorByRarity()
    {
        switch (_rarity)
        {
            case ShipComponentRarity.Common:
                return new Color(1f, 1f, 1f);
            case ShipComponentRarity.Rare:
                return new Color(0f, 0f, 1f);
            case ShipComponentRarity.Artifact:
                return new Color(1f, 0f, 1f);
            case ShipComponentRarity.Forerunner:
                return new Color(1f, .5f, 0f);
            default:
                Debug.Log("Used invalid rarity!");
                return UnityEngine.Color.cyan;
        }
    }
}
public enum ShipComponentType
{
    Weapon,
    Utility,
}
public enum ShipComponentRarity
{
    Common,
    Rare,
    Artifact,
    Forerunner,
}