public class Trait
{
    public string title { get; private set; }
    public string description { get; private set; }

    public ShipVitalModifier[] modifiers { get; private set; }

    public Trait(string title, string description, ShipVitalModifier[] modifiers)
    {
        this.title = title;
        this.description = description;

        this.modifiers = modifiers;
    }

    public override string ToString()
    {
        string s = "";

        s += "<color=yellow>" + title + "</color>\n";
        s += "<color=grey><i>" + description + "</i></color>\n\n";

        for (int i = 0; i < modifiers.Length; i++)
            s += modifiers[i].ToString() + (i == modifiers.Length - 1 ? "" : "\n");

        return s;
    }
}
