using System.Linq;

public static class TraitDB
{
    static Trait[] _traits;

    static bool _hasInitialized = false;

    public static void Initialize()
    {
        if (_hasInitialized)
            return;

        _traits = new Trait[]
        {
            new Trait("Veteran Engineer Staff", "This officer has a veteran engineer retinue assisting them aboard.",
                new ShipVitalModifier("Veteran Engineer Staff", .05f, ModifierMode.Percentage, VitalType.MovementSpeed),
                new ShipVitalModifier("Veteran Engineer Staff", .05f, ModifierMode.Percentage, VitalType.RotationSpeed)),

            new Trait("Expert Scanner", "This person is a highly adept scan technician.",
                new ShipVitalModifier("Expert Scanner", .1f, ModifierMode.Percentage, VitalType.ScanRadius),
                new ShipVitalModifier("Expert Scanner", .25f, ModifierMode.Percentage, VitalType.ScanRate)),

            new Trait("Fortune Favours the Bold", "This commander charges headlong into action, without a shred of doubt.",
                new ShipVitalModifier("Fortune Favours the Bold", .05f, ModifierMode.Percentage, VitalType.MovementSpeed),
                new ShipVitalModifier("Fortune Favours the Bold", .05f, ModifierMode.Percentage, VitalType.MovementSpeed),
                new ShipVitalModifier("Fortune Favours the Bold", -.1f, ModifierMode.Percentage, VitalType.ShieldPoints)),

            new Trait("No Unknown Variables", "This commander prefers a cautious approach, leaving nothing to chance.",
                new ShipVitalModifier("No Unknown Variables", -.05f, ModifierMode.Percentage, VitalType.MovementSpeed),
                new ShipVitalModifier("No Unknown Variables", -.05f, ModifierMode.Percentage, VitalType.MovementSpeed),
                new ShipVitalModifier("No Unknown Variables", .1f, ModifierMode.Percentage, VitalType.ShieldPoints)),
        };
        _hasInitialized = true;
    }

    public static Trait Get(string name)
    {
        return _traits.FirstOrDefault(t => t.title == name);
    }
    public static Trait GetRandom()
    {
        return _traits.RandomItem();
    }
}
