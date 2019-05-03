using UnityEngine;

using Random = System.Random;

public class Celestial : PointOfInterest
{
    public Celestial(string name, Vector3 position, Random random) : base(name, position, random)
    {
    }

    public override string GetTooltip()
    {
        string s = "";

        return s;
    }
}