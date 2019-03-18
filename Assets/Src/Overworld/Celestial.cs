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

        return s;
    }
}