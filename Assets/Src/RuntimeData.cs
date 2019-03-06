using UnityEngine;

using Random = System.Random;
using System.Collections.Generic;
using System.Linq;

public static class RuntimeData
{
    static Save _save;
    static StarSystem _system;
    //use to hold data between scenes
    static LocalMapData _location;

    public static Save save { get { return _save; } }
    public static StarSystem system { get { return _system; } }
    public static LocalMapData localMapData { get { return _location; } }

    public static void GenerateNewSystem(int seed = -1)
    {
        if (seed == -1)
            seed = (int)System.DateTime.Now.Ticks;

        _system = new StarSystem(seed);
        _system.Generate();

        OnNewSystem();
    }
    public static void SetSystem(StarSystem s)
    {
        _system = s;

        OnNewSystem();
    }

    public static void SetLocalMapData(LocalMapData l)
    {
        _location = l;
    }
    public static void SetSave(Save save)
    {
        _save = save;
    }

    static void OnNewSystem()
    {
        if (PlayerData.fleet == null)
            PlayerData.Initialize(_system.grid.GetRandom());

        _location = null;
    }

    public static void OnPlayerDefeat()
    {
        ProgressData.Reset();

        _system = null;
    }
}
public static class PlayerData
{
    static Fleet _fleet;
    static List<ShipComponent> _inventory;
    static List<Officer> _officers;

    public static Fleet fleet { get { return _fleet; } }
    public static List<ShipComponent> inventory { get { return _inventory; } }
    public static List<Officer> officers { get { return _officers; } }

    public static void Initialize(Cell cell)
    {
        _fleet = new Fleet("<color=purple>You</color>", cell, new Random(RuntimeData.system.seed), 0, new List<Ship>());
        _inventory = new List<ShipComponent>();
        _officers = new List<Officer>();

        var ships = Resources.LoadAll<ShipData>("Data/Start Fleets/" + RuntimeData.save.data.difficulty.ToString());

        for (int i = 0; i < ships.Length; i++)
            _fleet.AddShip(new Ship(ships[i]));

        ProgressData.Reset();

#if UNITY_EDITOR
        DebugGenerateOfficers();
        DebugGenerateInventory();
#endif
    }

    public static void OnPlayerRetreat()
    {

    }
    public static void OnPlayerDefeat()
    {
        _fleet = null;
        _inventory = null;
        _officers = null;
    }

    static void DebugGenerateOfficers()
    {
        for (int i = 0; i < 5; i++)
            _officers.Add(new Officer());
    }
    static void DebugGenerateInventory()
    {
        var choices = Resources.LoadAll<ShipComponentData>("Data/Parts/");

        for (int i = 0; i < 10; i++)
            _inventory.Add(choices.RandomItem().Instantiate());
    }
}