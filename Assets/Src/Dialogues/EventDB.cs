using UnityEngine;

using Random = UnityEngine.Random;
using System.Linq;
using System;

public static class EventDB
{
    static DialogueEvent[] _starts;
    static DialogueEvent[] _onNewSystem;
    static DialogueEvent[] _disasters;
    static DialogueEvent[] _all;

    static bool _hasInitialized = false;

    public static void Initialize()
    {
        if (_hasInitialized)
            return;

        _starts = Resources.LoadAll<DialogueEvent>("Data/Dialogue Events/Start/");
        _onNewSystem = Resources.LoadAll<DialogueEvent>("Data/Dialogue Events/New System/");
        _disasters = Resources.LoadAll<DialogueEvent>("Data/Dialogue Events/Disasters/");
        _all = Resources.LoadAll<DialogueEvent>("Data/Dialogue Events");
    }

    public static DialogueEvent GetStart()
    {
        return _starts[Random.Range(0, _starts.Length)];
    }
    public static DialogueEvent GetNewSystem()
    {
        return _onNewSystem[Random.Range(0, _onNewSystem.Length)];
    }
    public static DialogueEvent GetDisaster()
    {
        return _disasters.RandomItem();
    }

    public static DialogueEvent GetByID(int id)
    {
        return _all.FirstOrDefault(e => e.id == id);
    }
    public static DialogueEvent GetByName(string name)
    {
        return _all.FirstOrDefault(e => e.name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
    }
}
