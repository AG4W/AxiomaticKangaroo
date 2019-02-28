using UnityEngine;
using UnityEngine.UI;

using Random = System.Random;

public static class ModelDB 
{
    static bool _hasInitialized = false;

    static Sprite[] _icons;
    static Sprite[] _resourceIcons;
    static Sprite[] _portraits;
    static Sprite _defaultPortrait;

    static GameObject _shieldDeflectionVFX;

    static GameObject[] _stars;
    static GameObject[][] _planets;

    static GameObject _fleetEntity;

    public static Sprite defaultPortrait { get { return _defaultPortrait; } }
    public static GameObject fleetEntity { get { return _fleetEntity; } }

    public static void Initialize()
    {
        if (_hasInitialized)
            return;

        _shieldDeflectionVFX = Resources.Load<GameObject>("shieldDeflection");

        LoadMisc();
        LoadStars();
        LoadPlanetModels();
        LoadPortraits();

        _hasInitialized = true;
    }

    static void LoadMisc()
    {
        int l = System.Enum.GetNames(typeof(PointOfInterestType)).Length;
        _icons = new Sprite[l];

        for (int i = 0; i < l; i++)
            _icons[i] = Resources.Load<Sprite>("Overworld/Icons/" + ((PointOfInterestType)i).ToString().ToLower() + "");

        l = System.Enum.GetNames(typeof(FleetVitalType)).Length;
        _resourceIcons = new Sprite[l];

        for (int i = 0; i < l; i++)
            _resourceIcons[i] = Resources.Load<Sprite>("Overworld/Resource Icons/" + ((FleetVitalType)i).ToString().ToLower() + "");

        _fleetEntity = Resources.Load<GameObject>("Overworld/fleetEntity");
    }
    static void LoadStars()
    {
        _stars = Resources.LoadAll<GameObject>("Celestials/Stars/");
    }
    static void LoadPlanetModels()
    {
        int l = System.Enum.GetNames(typeof(PlanetType)).Length;

        _planets = new GameObject[l][];

        for (int i = 0; i < l; i++)
            _planets[i] = Resources.LoadAll<GameObject>("Celestials/Planets/" + ((PlanetType)i).ToString() + "/");
    }
    static void LoadPortraits()
    {
        _portraits = Resources.LoadAll<Sprite>("Art/Portraits/Officers/");
        _defaultPortrait = Resources.Load<Sprite>("Art/Portraits/default");
    }

    public static GameObject GetShieldDeflectionVFX()
    {
        return _shieldDeflectionVFX;
    }

    public static GameObject GetStar(Random random)
    {
        return _stars[random.Next(0, _stars.Length)];
    }
    public static GameObject GetPlanet(PlanetType type, Random random)
    {
        return _planets[(int)type][random.Next(0, _planets[(int)type].Length)];
    }

    public static Sprite GetIcon(PointOfInterestType icon)
    {
        return _icons[(int)icon];
    }
    public static Sprite GetResourceIcon(FleetVitalType type)
    {
        return _resourceIcons[(int)type];
    }

    public static Sprite GetPortrait()
    {
        return _portraits.RandomItem();
    }
}
