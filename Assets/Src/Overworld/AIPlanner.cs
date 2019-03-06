using UnityEngine;

using Random = System.Random;
using System.Collections.Generic;
using System.Collections;

public class AIPlanner
{
    Random _random;
    List<Fleet> _fleets = new List<Fleet>();

    bool _knowsAboutPlayer = false;

    public int fleetCount { get { return _fleets.Count; } }

    public List<Fleet> fleets { get { return _fleets; } }
    public bool knowsAboutPlayer { get { return _knowsAboutPlayer; } }

    public AIPlanner()
    {
        _random = new Random(RuntimeData.system.seed);
    }

    public void Execute()
    {
        //Debug.Log("Executing AIPlanner");
        //assume only hostile fleets atm
        //and always have them move towards player
        OverworldUIManager.getInstance.StartCoroutine(ExecuteAsync());
    }

    public void AddFleet(Fleet fleet)
    {
        _fleets.Add(fleet);
    }
    public void RemoveFleet(Fleet fleet)
    {
        _fleets.Remove(fleet);
    }
    public void OnPlayerDiscovered()
    {
        _knowsAboutPlayer = true;
    }

    void Plan(Fleet fleet)
    {
        //scan at start and end of every move
        ScanForPlayer(fleet);
    }
    void ScanForPlayer(Fleet fleet)
    {
        if (_knowsAboutPlayer)
            return;

        _knowsAboutPlayer = true;

        LogManager.getInstance.AddEntry(fleet.name + " has spotted you!", 15f, EntryType.Combat);
    }

    IEnumerator ExecuteAsync()
    {
        //AI think delay
        yield return new WaitForSeconds(_random.Next(1, 5) * .5f);

        for (int i = 0; i < _fleets.Count; i++)
        {
            Plan(_fleets[i]);
            //for less static movement
            yield return new WaitForSeconds(_random.Next(1, 5) * .1f);
        }
       
        OverworldManager.EndCurrentTurn();
    }
}