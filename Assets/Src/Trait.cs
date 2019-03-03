public class Trait
{
    string _title;
    string _description;

    ShipVitalModifier[] _modifiers;

    public string title { get { return _title; } }

    public ShipVitalModifier[] modifiers { get { return _modifiers; } }

    public Trait(string title, string description, ShipVitalModifier[] modifiers)
    {
        _title = title;
        _description = description;

        _modifiers = modifiers;
    }

    public override string ToString()
    {
        string s = "";

        s += _title + "\n\n";
        s += _description + "\n\n";

        for (int i = 0; i < _modifiers.Length; i++)
            s += _modifiers[i].value + " " + _modifiers[i].type + (i == _modifiers.Length - 1 ? "" : "\n");

        return s;
    }
}
