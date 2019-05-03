using System;
using System.Collections.Generic;

public static class Event
{
    static List<Action<object[]>>[] _actionEvents;

    public static void Initialize()
    {
        _actionEvents = new List<Action<object[]>>[Enum.GetNames(typeof(ActionEvent)).Length];

        for (int i = 0; i < _actionEvents.Length; i++)
            _actionEvents[i] = new List<Action<object[]>>();
    }

    public static void Subscribe(ActionEvent e, Action<object[]> a)
    {
#if UNITY_EDITOR
        if (a == null)
            UnityEngine.Debug.LogWarning(e.ToString() + " IS BEING SUBSCRIBED TO WITH NULL ACTION.");
#endif
        _actionEvents[(int)e].Add(a);
    }
    public static void Unsubscribe(ActionEvent e, Action<object[]> a)
    {
        _actionEvents[(int)e].Remove(a);
    }

    public static void Raise(ActionEvent e, params object[] args)
    {
        for (int i = 0; i < _actionEvents[(int)e].Count; i++)
            _actionEvents[(int)e][i]?.Invoke(args);
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