using UnityEngine;

using Random = System.Random;
using System.Collections.Generic;
using System.Linq;

public static class RuntimeData
{

    public static Save save { get; private set; }
    public static StarSystem system { get; private set; }
    public static LocalMapData localMapData { get; private set; }

    public static void GenerateNewSystem(int seed = -1)
    {
        if (seed == -1)
            seed = (int)System.DateTime.Now.Ticks;

        system = new StarSystem(seed);
        system.Generate();

        OnNewSystem();
    }
    public static void SetSystem(StarSystem system)
    {
        RuntimeData.system = system;

        OnNewSystem();
    }

    public static void SetLocalMapData(LocalMapData lmd)
    {
        localMapData = lmd;
    }
    public static void SetSave(Save save)
    {
        RuntimeData.save = save;
    }

    static void OnNewSystem()
    {
        if (PlayerData.fleet == null)
            PlayerData.Initialize(system.GetRandomLocation());

        localMapData = null;
    }

    public static void OnPlayerDefeat()
    {
        ProgressData.Reset();

        system = null;
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

    public static void Initialize(Vector3 position)
    {
        _fleet = new Fleet("<color=purple>You</color>", position, new Random(RuntimeData.system.seed), 0, new List<Ship>());
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