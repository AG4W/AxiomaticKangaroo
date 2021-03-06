﻿using UnityEngine;

using System.Collections.Generic;
using System.Linq;

//localmapsize will be fixed size
public class LocalMapData
{
    const float WORLD_TO_LOCAL_SIZE_SCALE = 50f;
    const float WORLD_TO_LOCAL_DISTANCE_SCALE = 100f;

    const int LOCAL_MAP_SIZE = 3000;

    const int MAX_ASTEROID_COUNT = 25;
    const int MIN_DISTANCE_BETWEEN_ASTEROIDS = 25;

    string _name;
    float[] _resourceDensities;

    Vector3 _location;

    StarSystem _system;
    Celestial _orbital;

    List<LocalCelestial> _localCelestials = new List<LocalCelestial>();
    List<Fleet> _localFleets = new List<Fleet>();

    bool _playerKnowsAboutEnemy = false;

    public string name { get { return _name; } }
    public int size { get { return LOCAL_MAP_SIZE; } }

    public float[] resourceDensities { get { return _resourceDensities; } }

    public Vector3 location { get { return _location; } }

    public List<LocalCelestial> localCelestials { get { return _localCelestials; } }
    public List<Fleet> fleets { get { return _localFleets; } }
    
    public bool playerKnowsAboutEnemy { get { return _playerKnowsAboutEnemy; } }

    public LocalMapData(string name, float[] resourceDensities, Vector3 location, Celestial orbit = null)
    {
        _name = name;
        _resourceDensities = resourceDensities;
        _location = location;

        CacheCelestials(orbit);
    }

    public void Instantiate()
    {
        GenerateCelestials();
        GenerateResources();
    }
    public void SetPlayerKnowsAboutEnemy(bool status)
    {
        _playerKnowsAboutEnemy = status;
    }
    public void AddFleet(Fleet fleet)
    {
        _localFleets.Add(fleet);
    }

    void GenerateCelestials()
    {
        //instantiate localcelestials
        for (int i = 0; i < _localCelestials.Count; i++)
        {
            if (_localCelestials[i].celestial.model == null)
                continue;

            bool isStar = _localCelestials[i].celestial.type == PointOfInterestType.Star;
            bool isPlanet = _localCelestials[i].celestial.type == PointOfInterestType.Planet;

            //offset to keep stuff outside of actual map
            //need to increase offset so huge stars doesnt cover local map
            Vector3 offset = _localCelestials[i].vector.normalized * (LOCAL_MAP_SIZE * (isStar ? 5 : 2));
            Vector3 h = (Vector3.zero + _localCelestials[i].vector) * WORLD_TO_LOCAL_DISTANCE_SCALE;
            Vector3 pos = offset + h;

            GameObject g = Object.Instantiate(
                _localCelestials[i].celestial.model,
                pos,
                Random.rotation,
                null);

            //hacky fixes for light issues
            if(isStar)
            {
                ((Star)_localCelestials[i].celestial).ApplyColors(g);

                foreach (Light l in g.GetComponentsInChildren<Light>())
                    l.enabled = false;

                Light dl = g.AddComponent<Light>();
                dl.type = LightType.Point;
                dl.lightmapBakeType = LightmapBakeType.Mixed;
                dl.shadows = LightShadows.Hard;
                dl.range = 10000000;
                dl.intensity = 1000000000;
                //dl.transform.rotation = Quaternion.LookRotation(Vector3.zero - pos);
            }
            else if (isPlanet)
            {
                Planet p = ((Planet)_localCelestials[i].celestial);

                g.GetComponent<PlanetEntity>().Initialize(p.palette, p.waterLevel, p.isTerrestrial);
            }

            //increase size of the local celestial (last added to list)
            //to create parallax effect and false distance
            g.transform.localScale *= i == _localCelestials.Count - 1 ? WORLD_TO_LOCAL_SIZE_SCALE * 5 : WORLD_TO_LOCAL_SIZE_SCALE;
        }
    }
    void GenerateResources()
    {
        float bufferedMapSize = (LOCAL_MAP_SIZE * .9f);

        FleetVitalType[] resources = new FleetVitalType[] 
        {
            FleetVitalType.NebulaGas,
            FleetVitalType.Veldspar,
            FleetVitalType.Tritanite
        };

        var all = Resources.LoadAll<GameObject>("Combat/ExtractableResources/");

        for (int j = 0; j < resources.Length; j++)
        {
            int count = Mathf.RoundToInt(MAX_ASTEROID_COUNT * _resourceDensities[j]);
            var resourceModels = all.Where(re => re.GetComponent<ResourceEntity>().type == resources[j]).ToArray();
            List<GameObject> asteroids = new List<GameObject>();

            for (int i = 0; i < count; i++)
            {
                Vector3 pos = new Vector3(
                    Random.Range(-bufferedMapSize, bufferedMapSize),
                    Random.Range(-bufferedMapSize, bufferedMapSize),
                    Random.Range(-bufferedMapSize, bufferedMapSize));

                while (asteroids.Any(a => Vector3.Distance(a.transform.position, pos) < MIN_DISTANCE_BETWEEN_ASTEROIDS))
                {
                    pos = new Vector3(
                        Random.Range(-bufferedMapSize, bufferedMapSize),
                        Random.Range(-bufferedMapSize, bufferedMapSize),
                        Random.Range(-bufferedMapSize, bufferedMapSize));
                }

                asteroids.Add(
                    Object.Instantiate(
                        resourceModels.RandomItem(),
                        pos,
                        Random.rotation,
                        null));
            }
        }
    }

    void CacheCelestials(Celestial orbit)
    {
        for (int i = 0; i < RuntimeData.system.celestials.Count; i++)
        {
            if (RuntimeData.system.celestials[i] != orbit)
            {
                _localCelestials.Add(
                    new LocalCelestial(
                        RuntimeData.system.celestials[i],
                        RuntimeData.system.celestials[i].location - _location));
            }
        }

        Vector3[] cardinals = new Vector3[]
        {
            Vector3.up,
            Vector3.down,
            Vector3.right,
            Vector3.left,
            Vector3.forward,
            Vector3.back
        };

        if (orbit != null)
            _localCelestials.Add(new LocalCelestial(orbit, cardinals.RandomItem()));
    }
}
public struct LocalCelestial
{
    public readonly Celestial celestial;
    public readonly Vector3 vector;

    public LocalCelestial(Celestial celestial, Vector3 vector)
    {
        this.celestial = celestial;
        this.vector = vector;
    }
}