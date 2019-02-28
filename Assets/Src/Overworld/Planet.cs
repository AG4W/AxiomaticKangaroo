using UnityEngine;

using Random = System.Random;

public class Planet : Celestial
{
    float _waterLevel;
    PlanetType _planetType;
    //moons
    //rings?

    //useless fluff
        //biome
        //etc, etc
        //use to procedurally generate texture
            //later :)

    //color
    Palette _palette;

    public float waterLevel { get { return _waterLevel; } }
    public PlanetType planetType { get { return _planetType; } }
    public Palette palette { get { return _palette; } }

    public bool isTerrestrial { get { return _planetType == PlanetType.Terrestrial; } }

    public Planet(string name, Vector3 location, Random random, PlanetType planetType) : base(name, location, random)
    {
        base.type = PointOfInterestType.Planet;
        base.model = ModelDB.GetPlanet(planetType, random);

        _waterLevel = random.Next(0, 100) * .01f;
        _planetType = planetType;
        _palette = PaletteDB.Get(_planetType).Instantiate();
    }

    public override GameObject Instantiate()
    {
        base.Instantiate();
        base.prefab.GetComponent<PlanetEntity>().Initialize(_palette, _waterLevel, isTerrestrial);
        base.prefab.transform.eulerAngles = 
            new Vector3(
                base.random.Next(0, 360), 
                base.random.Next(0, 360), 
                base.random.Next(0, 360));

        return base.prefab;
    }

    public override string GetTooltip()
    {
        string s = "";

        s += "Type: " + Format(_planetType) + "\n";

        if (isTerrestrial)
            s += "Water: " + (_waterLevel * 100f).ToString("0.##") + "%\n\n";

        s += base.GetResourceString();
        s += "\n" + base.GetDistanceTooltip();

        return s;
    }

    static string Format(PlanetType pt)
    {
        switch (pt)
        {
            case PlanetType.Gas:
                return "Gas Giant";
            case PlanetType.Barren:
                return "Barren";
            case PlanetType.Terrestrial:
                return "Terrestrial";
            default:
                return "INVALID PLANETTYPE";
        }
    }
}
public enum PlanetType
{
    Gas,
    Barren,
    Terrestrial
}