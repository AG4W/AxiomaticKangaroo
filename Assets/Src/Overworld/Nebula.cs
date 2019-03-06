using UnityEngine;

using Random = System.Random;

public class Nebula : Celestial
{
    public Nebula(string name, Cell cell, Random random) : base(name, cell, random)
    {
        base.type = PointOfInterestType.Nebula;
    }
}
