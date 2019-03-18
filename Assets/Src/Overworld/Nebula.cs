using UnityEngine;

using Random = System.Random;

public class Nebula : Celestial
{
    public Nebula(string name, Cell cell, Random random) : base(name, cell, random)
    {
        base.type = PointOfInterestType.Nebula;
    }

    public override string GetTooltip()
    {
        string s = "";

        s += 
            "Nebula Clouds are tight clusters in space where high pressures produces " + FleetVital.Format(FleetVitalType.NebulaGas) + ".\n" + 
            "The most vital component required in the production of " + FleetVital.Format(FleetVitalType.ProcessedFuel) + ".";

        return s;
    }
}
