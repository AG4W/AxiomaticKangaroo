using UnityEngine;

public static class PaletteDB
{
    static PaletteData[] _palettes;

    static PaletteData[] _terrestrials;
    static PaletteData[] _gas;
    static PaletteData[] _barren;

    static bool _hasInitialized = false;

    public static void Initialize()
    {
        if (_hasInitialized)
            return;

        _palettes = Resources.LoadAll<PaletteData>("Palettes/");

        _terrestrials = Resources.LoadAll<PaletteData>("Palettes/Terrestrial/");
        _gas = Resources.LoadAll<PaletteData>("Palettes/Gas/");
        _barren = Resources.LoadAll<PaletteData>("Palettes/Barren/");

        _hasInitialized = true;
    }

    public static PaletteData Get(PlanetType pt)
    {
        switch (pt)
        {
            case PlanetType.Gas:
                return _gas.RandomItem();
            case PlanetType.Barren:
                return _barren.RandomItem();
            case PlanetType.Terrestrial:
                return _terrestrials.RandomItem();
            default:
                return _palettes.RandomItem();
        }
    }
}
