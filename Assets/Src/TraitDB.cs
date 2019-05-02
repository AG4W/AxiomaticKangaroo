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
            new Trait("Veteran Engineer Staff", "This officer has a veteran engineer retinue.", new ShipVitalModifier[]
            {
                new ShipVitalModifier("Veteran Engineer Staff", .05f, ModifierMode.Percentage, VitalType.MovementSpeed),
                new ShipVitalModifier("Veteran Engineer Staff", .05f, ModifierMode.Percentage, VitalType.RotationSpeed)
            }),
            new Trait("Expert Scanner", "This person is a highly adept scan technician.", new ShipVitalModifier[]
            {
                new ShipVitalModifier("Expert Scanner", .1f, ModifierMode.Percentage, VitalType.ScanRadius),
                new ShipVitalModifier("Expert Scanner", .25f, ModifierMode.Percentage, VitalType.ScanRate)
            })
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
