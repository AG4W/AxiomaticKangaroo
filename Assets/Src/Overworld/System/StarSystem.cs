using UnityEngine;

using Random = System.Random;
using System.Collections.Generic;
using System.Linq;

public class StarSystem
{
    const int MAX_STARGATES = 4;
    const int MAX_CHARTED_WORMHOLES = 4;
    const int MAX_UNCHARTED_WORMHOLES = 1;
    const int MAX_NEBULAS = 3;

    const int MAX_PLANETS = 8;
    const int MAX_STRUCTURES = 2;

    Random _random;
    List<Nebula> _nebulas = new List<Nebula>();

    public int seed { get; }
    public int size { get; }

    public Star star { get; private set; }

    public List<Stargate> stargates { get; } = new List<Stargate>();
    public List<Wormhole> wormholes { get; } = new List<Wormhole>();
    public List<Planet> planets { get; } = new List<Planet>();
    public List<Celestial> celestials { get; } = new List<Celestial>();

    public List<PointOfInterest> pointsOfInterest { get; } = new List<PointOfInterest>();
    public List<AIPlanner> aiEntities { get; } = new List<AIPlanner>();

    public StarSystem(int seed)
    {
        _random = new Random(seed);

        this.seed = seed;
        this.size = _random.Next(500, 1000);
    }

    public void Generate()
    {
        GenerateStar();
        GenerateWormholes();
        GenerateNebulas();
        GenerateStargates();
        GeneratePlanets();
    }
    void GenerateStar()
    {
        //generate star, always at 0,random?, 0 
        star = new Star(
            NameGenerator.GetPOIName(PointOfInterestType.Star),
            new Vector3(size / 2, 0f, size / 2),
            _random);

        celestials.Add(star);
        pointsOfInterest.Add(star);
    }
    void GenerateStargates()
    {
        int sgcount = _random.Next(wormholes.Count > 0 ? 0 : 1, MAX_STARGATES);

        for (int i = 0; i < sgcount; i++)
        {
            Stargate s = new Stargate(
                "Stargate",
                GetRandomLocation(),
                _random);

            stargates.Add(s);
            pointsOfInterest.Add(s);
        }
    }
    void GenerateNebulas()
    {
        int ncount = _random.Next(0, MAX_NEBULAS);

        for (int i = 0; i < ncount; i++)
        {
            Nebula n = new Nebula(
                "Nebula",
                GetRandomLocation(),
                _random);

            _nebulas.Add(n);
            pointsOfInterest.Add(n);
        }
    }
    void GenerateWormholes()
    {
        int whcount = _random.Next(0, MAX_CHARTED_WORMHOLES);

        for (int i = 0; i < whcount; i++)
        {
            Wormhole w = new Wormhole(
                "Wormhole",
                GetRandomLocation(),
                _random);

            wormholes.Add(w);
            pointsOfInterest.Add(w);
        }
    }
    void GeneratePlanets()
    {
        int planetCount = _random.Next(0, MAX_PLANETS);
        int starBuffer = 150;
        //in 0..100
        //int stepVariation = 25;

        for (int i = 0; i < planetCount; i++)
        {
            //skip to create some empty spaces or something else
            if (_random.Next(0, 1) == 1)
                continue;

            int step = _random.Next(50, 250);

            //get a random direction, and multiply by distance.
            Vector3 pos = GetLocation(starBuffer + (i * step));
            //introduce some randomness in vertical plane
            //pos.y = _random.Next(-25, 25);

            PlanetType pt = (PlanetType)_random.Next(0, System.Enum.GetNames(typeof(PlanetType)).Length);

            Planet p = new Planet(
                star.name + "_" + (i + 1).ToRomanNumeral(),
                GetRandomLocation(),
                _random, 
                pt);

            planets.Add(p);
            celestials.Add(p);
            pointsOfInterest.Add(p);
        }
    }

    public void Instantiate()
    {
        for (int i = 0; i < pointsOfInterest.Count; i++)
            pointsOfInterest[i].Instantiate();
    }

    public void AddAIEntity(AIPlanner ai)
    {
        aiEntities.Add(ai);
    }
    public void RemoveAIEntity(AIPlanner ai)
    {
        for (int i = 0; i < ai.fleetCount; i++)
            RemovePointOfInterest(ai.fleets[i]);

        aiEntities.Remove(ai);
    }

    public void AddPointOfInterest(PointOfInterest poi)
    {
        pointsOfInterest.Add(poi);

        poi.Instantiate();

        OnPointOfInterestAdded?.Invoke(poi);
    }
    public void RemovePointOfInterest(PointOfInterest poi)
    {
        pointsOfInterest.Remove(poi);

        poi.Deinstantiate();

        OnPointOfInterestRemoved?.Invoke(poi);
    }

    public void RemoveFleet(Fleet fleet)
    {
        for (int i = 0; i < aiEntities.Count; i++)
        {
            if(aiEntities[i].fleets.IndexOf(fleet) != -1)
            {
                if (aiEntities[i].fleetCount == 1)
                    RemoveAIEntity(aiEntities[i]);
                else
                    RemovePointOfInterest(fleet);

                break;
            }
        }
    }

    public Vector3 GetRandomLocation()
    {
        return GetLocation(_random.Next(-size, size));
    }
    /// <summary>
    /// Very high minimumDistanceToOtherCelestial might cause infinite or near infinite loops in generation! Use at own peril!
    /// </summary>
    /// <param name="minimumDistanceToOtherCelestial"></param>
    /// <param name="excludeBufferZone"></param>
    /// <returns></returns>
    public Vector3 GetRandomLocation(float minimumDistanceToOtherCelestial)
    {
        Vector3 p = Vector3.zero;

        while (p == Vector3.zero || celestials.Any(c => Vector3.Distance(c.position, p) < minimumDistanceToOtherCelestial))
            p = GetRandomLocation();

        return p;
    }

    public Vector3 GetLocation(int radius)
    {
        int x = Mathf.RoundToInt(radius * Mathf.Cos(_random.Next(0, 360)));
        int z = Mathf.RoundToInt(radius * Mathf.Sin(_random.Next(0, 360)));

        return new Vector3(x, 0, z);
    }
    public Vector3 GetLocation(int radius, float angle)
    {
        angle = Mathf.Clamp(angle, 0, 360);

        int x = Mathf.RoundToInt(radius * Mathf.Cos(angle));
        int z = Mathf.RoundToInt(radius * Mathf.Sin(angle));

        return new Vector3(x, 0, z);
    }
    public Vector3 GetLocation(Vector3 center, float radius)
    {
        return center + GetLocation((int)radius);
    }

    public delegate void PointOfInterestEvent(PointOfInterest poi);
    public PointOfInterestEvent OnPointOfInterestAdded;
    public PointOfInterestEvent OnPointOfInterestRemoved;
}