using System;
using System.Collections.Generic;

public static class Event
{
    static List<Action>[] _actionEvents;

    public static void Initialize()
    {
        _actionEvents = new List<Action>[Enum.GetNames(typeof(ActionEvent)).Length];

        for (int i = 0; i < _actionEvents.Length; i++)
            _actionEvents[i] = new List<Action>();
    }

    public static void Subscribe(ActionEvent e, Action a)
    {
#if UNITY_EDITOR
        if (a == null)
            UnityEngine.Debug.LogWarning(e.ToString() + " IS BEING SUBSCRIBED TO WITH NULL ACTION.");
#endif
        _actionEvents[(int)e].Add(a);
    }
    public static void Unsubscribe(ActionEvent e, Action a)
    {
        _actionEvents[(int)e].Remove(a);
    }

    public static void Raise(ActionEvent e)
    {
        for (int i = 0; i < _actionEvents[(int)e].Count; i++)
            _actionEvents[(int)e][i]?.Invoke();
    }
}
public enum ActionEvent
{
    LeaveLocalMap,

    IncreaseSimulationSpeed,
    DecreaseSimulationSpeed,
    ToggleSimulation,

    ToggleAlignmentPlane,
}