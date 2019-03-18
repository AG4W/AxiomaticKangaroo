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

    int _size;
    int _seed;

    Random _random;
    HexGrid _grid;

    Star _star;

    List<Nebula> _nebulas = new List<Nebula>();
    List<Stargate> _stargates = new List<Stargate>();
    List<Structure> _structures = new List<Structure>();
    List<Wormhole> _wormholes = new List<Wormhole>();
    List<Planet> _planets = new List<Planet>();
    List<Celestial> _celestials = new List<Celestial>();

    List<PointOfInterest> _pointsOfInterest = new List<PointOfInterest>();
    List<AIPlanner> _aiPlanners = new List<AIPlanner>();

    public int seed { get { return _seed; } }
    public int size { get { return _size; } }

    public HexGrid grid { get { return _grid; } }
    public Star star { get { return _star; } }

    public List<Stargate> stargates { get { return _stargates; } }
    public List<Structure> structures { get { return _structures; } }
    public List<Wormhole> wormholes { get { return _wormholes; } }
    public List<Planet> planets { get { return _planets; } }
    public List<Celestial> celestials { get { return _celestials; } }

    public List<PointOfInterest> pointsOfInterest { get { return _pointsOfInterest; } }
    public List<AIPlanner> aiEntities { get { return _aiPlanners; } }

    public StarSystem(int seed)
    {
        _random = new Random(seed);

        _seed = seed;

        while (_size == 0 || _size % 2 == 0)
            _size = _random.Next(7, 15);
    }

    public void Generate()
    {
        _grid = new HexGrid(_size, _random);

        GenerateStar();
        GenerateWormholes();
        GenerateNebulas();
        GenerateStargates();
        GeneratePlanets();
        GenerateStructures();
    }
    void GenerateStar()
    {
        //generate star, always at 0,random?, 0 
        _star = new Star(
            NameGenerator.GetPOIName(PointOfInterestType.Star),
           _grid.Get(_size / 2, _size / 2),
            _random);

        _celestials.Add(_star);
        _pointsOfInterest.Add(_star);
    }
    void GenerateStargates()
    {
        int sgcount = _random.Next(_wormholes.Count > 0 ? 0 : 1, MAX_STARGATES);

        for (int i = 0; i < sgcount; i++)
        {
            Stargate s = new Stargate(
                "Stargate",
                _grid.GetRandom(),
                _random);

            _stargates.Add(s);
            _pointsOfInterest.Add(s);
        }
    }
    void GenerateNebulas()
    {
        int ncount = _random.Next(0, MAX_NEBULAS);

        for (int i = 0; i < ncount; i++)
        {
            Nebula n = new Nebula(
                "Gas Clouds",
                _grid.GetRandom(),
                _random);

            _nebulas.Add(n);
            _pointsOfInterest.Add(n);
        }
    }
    void GenerateWormholes()
    {
        int whcount = _random.Next(0, MAX_CHARTED_WORMHOLES);

        for (int i = 0; i < whcount; i++)
        {
            Wormhole w = new Wormhole(
                "Uncharted Wormhole",
                _grid.GetRandom(),
                _random);

            _wormholes.Add(w);
            _pointsOfInterest.Add(w);
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
                _star.name + " " + (i + 1).ToRomanNumeral(),
                _grid.GetRandom(),
                _random, 
                pt);

            _planets.Add(p);
            _celestials.Add(p);
            _pointsOfInterest.Add(p);
        }
    }
    void GenerateStructures()
    {
        if (_planets.Count == 0)
            return;

        int structureCount = _random.Next(0, MAX_STRUCTURES);

        for (int i = 0; i < structureCount; i++)
        {
            Module m = (Module)_random.Next(0, System.Enum.GetNames(typeof(Module)).Length);

            Structure os = new Structure(
                Structure.FormatName(m),
                _grid.GetRandom(),
                _random,
                m);

            _structures.Add(os);
            _pointsOfInterest.Add(os);
        }
    }

    public void Instantiate()
    {
        _grid.Instantiate();

        for (int i = 0; i < _pointsOfInterest.Count; i++)
            _pointsOfInterest[i].Instantiate();

        //CreateOrbitalLines();
    }
    void CreateOrbitalLines()
    {
        Material lrMat = Resources.Load<Material>("orbitalLineMat");

        for (int i = 0; i < _planets.Count; i++)
        {
            GameObject g = new GameObject();

            g.transform.position = Vector3.zero;

            int segments = 80;

            LineRenderer lr = g.AddComponent<LineRenderer>();
            lr.material = lrMat;
            lr.loop = true;
            lr.useWorldSpace = true;
            lr.positionCount = segments;
            lr.startWidth = 1f;
            lr.endWidth = 1f;

            float angle = 20f;
            float distance = Vector3.Distance(Vector3.zero, _planets[i].cell.location);

            for (int a = 0; a < segments; a++)
            {
                float x;
                float y = 0;
                float z ;

                x = Mathf.Sin(Mathf.Deg2Rad * angle) * distance;
                z = Mathf.Cos(Mathf.Deg2Rad * angle) * distance;

                lr.SetPosition(a, new Vector3(x, y, z));

                angle += (360f / segments);
            }
        }
    }

    public void AddAIEntity(AIPlanner ai)
    {
        _aiPlanners.Add(ai);
    }
    public void RemoveAIEntity(AIPlanner ai)
    {
        for (int i = 0; i < ai.fleetCount; i++)
            RemovePointOfInterest(ai.fleets[i]);

        _aiPlanners.Remove(ai);
    }

    public void AddPointOfInterest(PointOfInterest poi)
    {
        _pointsOfInterest.Add(poi);

        poi.Instantiate();

        OnPointOfInterestAdded?.Invoke(poi);
    }
    public void RemovePointOfInterest(PointOfInterest poi)
    {
        _pointsOfInterest.Remove(poi);

        poi.Deinstantiate();

        OnPointOfInterestRemoved?.Invoke(poi);
    }

    public void RemoveFleet(Fleet fleet)
    {
        for (int i = 0; i < _aiPlanners.Count; i++)
        {
            if(_aiPlanners[i].fleets.IndexOf(fleet) != -1)
            {
                if (_aiPlanners[i].fleetCount == 1)
                    RemoveAIEntity(_aiPlanners[i]);
                else
                    RemovePointOfInterest(fleet);

                break;
            }
        }
    }

    public Vector3 GetRandomLocation()
    {
        return GetLocation(_random.Next(-_size, _size));
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

        while (p == Vector3.zero || _celestials.Any(c => Vector3.Distance(c.cell.location, p) < minimumDistanceToOtherCelestial))
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