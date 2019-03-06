using UnityEngine;

using Random = System.Random;

public class Celestial : PointOfInterest
{
    public Celestial(string name, Cell cell, Random random) : base(name, cell, random)
    {
    }

    public override string GetTooltip()
    {
        string s = "";

        s += GetResourceString();
        s += "\n" + base.GetDistanceTooltip();

        return s;
    }
    protected string GetResourceString()
    {
        string s = "";

        s += "Local resources:\n\n";

        int turns = OverworldManager.turnCount;
        int threshold = DifficultyUtils.Multiply(RuntimeData.save.data.difficulty, 2);

        bool infoAvailable = turns > threshold;

        if (!infoAvailable)
            s += "Data will become available in " + (threshold - turns) + " days.\n\n";

        //s += FleetVital.Format(FleetVitalType.NebulaGas) + ": " + (infoAvailable ? (base.resourceDensities[0] * 100f).ToString("0.##") : "?") + "%\n";
        //s += FleetVital.Format(FleetVitalType.Veldspar) + ": " + (infoAvailable ? (base.resourceDensities[1] * 100f).ToString("0.##") : "?") + "%\n";
        //s += FleetVital.Format(FleetVitalType.Tritanite) + ": " + (infoAvailable ? (base.resourceDensities[2] * 100f).ToString("0.##") : "?") + "%\n";

        return s;
    }
}