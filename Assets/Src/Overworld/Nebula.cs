using UnityEngine;

using Random = System.Random;

public class Nebula : Celestial
{
    public Nebula(string name, Vector3 location, Random random) : base(name, location, random)
    {
        base.type = PointOfInterestType.Nebula;
    }
}
