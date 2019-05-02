using UnityEngine;

using Random = System.Random;

public class Star : Celestial
{
    Color _surface;
    Color _secondarySurface;

    Color _emission;

    public Color surface { get { return _surface; } }
    public Color secondarySurface { get { return _secondarySurface; } }

    public Color emission { get { return _emission; } }

    public Star(string name, Cell cell, Random random) : base(name, cell, random)
    {
        base.type = PointOfInterestType.Star;
        base.model = ModelDB.GetStar(random);
    }

    public override GameObject Instantiate()
    {
        base.Instantiate();
        ApplyColors(base.prefab);

        return base.prefab;
    }

    public void ApplyColors(GameObject model)
    {
        float index = base.random.Next(0, 100) * .01f;
        StarColorSettings starColorSettings = Resources.Load<StarColorSettings>("StarColorSettings");

        _surface = starColorSettings.GetSurface(index);
        _secondarySurface = starColorSettings.GetSurface(index * .8f);

        _emission = starColorSettings.GetReflection(index);

        _surface.a = 1f;
        _secondarySurface.a = 1f;
        _emission.a = 1f;

        Material m = model.GetComponentInChildren<Renderer>().material;

        m.SetColor("_surfaceColor", _surface);
        m.SetColor("_secondarySurfaceColor", _secondarySurface);
        m.SetColor("_surfaceEmissionColor", _emission * 4f);

        GameObject light = new GameObject();
        light.transform.SetParent(model.transform);
        light.transform.position = model.transform.position;

        Light l = light.AddComponent<Light>();
        l.type = LightType.Point;
        l.range = 10000;
        l.intensity = 100000f;
        l.bounceIntensity = 1f;
        //l.lightmapBakeType = LightmapBakeType.Mixed;
        l.shadows = LightShadows.None;
        //model.GetComponentInChildren<Light>().color = _emission;
    }

    public override string GetTooltip()
    {
        return "The local star in this system.";
    }
}
