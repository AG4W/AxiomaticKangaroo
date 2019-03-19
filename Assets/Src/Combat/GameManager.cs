using UnityEngine;
using UnityEngine.SceneManagement;

using System.Collections.Generic;
using System.Collections;
using System.Linq;

public static class GameManager
{
    static float _aiTickRate = 3f;
    static float _turnTimeInMinutes = 2f;
    static float _incursionChance = 15f;

    static SimulationSpeed _lastSpeed;
    static SimulationSpeed _simulationSpeed;

    static List<ResourceEntity> _resources;
    static List<ShipEntity> _ships;
    static List<Fleet> _fleets;

    public static List<ResourceEntity> resources { get { return _resources; } }
    public static List<ShipEntity> ships { get { return _ships; } }

    public static void Initialize()
    {
        //setup location
        RuntimeData.localMapData.Instantiate();

        _fleets = new List<Fleet>();
        _ships = new List<ShipEntity>();

        //collect all entities
        _resources = Object.FindObjectsOfType<ResourceEntity>().ToList();
        _lastSpeed = SimulationSpeed.Normal;

        //setup ui
        CommandMapper.Initialize();

        //setup eventual fleets
        for (int i = 0; i < RuntimeData.localMapData.fleets.Count; i++)
            AddFleet(RuntimeData.localMapData.fleets[i]);

        //enable/disable leave status
        OnGameStatusUpdated?.Invoke(_ships.All(s => s.teamID == 0));

        CameraManager.getInstance.JumpTo(_ships[0].transform.position, true);
        CoroutineSurrogate.getInstance.StartCoroutine(TickAICommanders());

        SetSimulationSpeed(SimulationSpeed.Stopped);
    }

    public static void IncrementSimulationSpeed()
    {
        if (_simulationSpeed == SimulationSpeed.Fastest)
            return;

        SetSimulationSpeed(_simulationSpeed + 1);
    }
    public static void DecrementSimulationSpeed()
    {
        if (_simulationSpeed == SimulationSpeed.Slow)
            return;

        SetSimulationSpeed(_simulationSpeed - 1);
    }
    public static void ToggleSimulation()
    {
        if (_simulationSpeed == SimulationSpeed.Stopped)
        {
            SetSimulationSpeed(_lastSpeed);
            _lastSpeed = SimulationSpeed.Stopped;
        }
        else
        {
            _lastSpeed = _simulationSpeed;
            SetSimulationSpeed(SimulationSpeed.Stopped);
        }
    }
    public static void SetSimulationSpeed(SimulationSpeed speed)
    {
        float t = 0;

        switch (speed)
        {
            case SimulationSpeed.Stopped:
                t = 0f;
                break;
            case SimulationSpeed.Slow:
                t = .2f;
                break;
            case SimulationSpeed.Normal:
                t = 1f;
                break;
            case SimulationSpeed.Fast:
                t = 2f;
                break;
            case SimulationSpeed.Fastest:
                t = 4f;
                break;
            default:
                t = 1f;
                break;
        }

        _simulationSpeed = speed;

        Time.timeScale = t;
        OnSimulationSpeedChanged?.Invoke(_simulationSpeed);
    }

    public static void AddFleet(Fleet fleet)
    {
        List<ShipEntity> ships = new List<ShipEntity>();
        //pick a random spawn location, use map siz with a 25% bufferzone to avoid spawning at edge of map
        Vector3 o = Random.insideUnitSphere * (RuntimeData.localMapData.size * .75f);

        for (int i = 0; i < fleet.ships.Count; i++)
        {
            ShipEntity se = fleet.ships[i].Instantiate(o + (Vector3.right * (i * 10)), Quaternion.identity, fleet.teamID, fleet.teamID == 0 ? true : RuntimeData.localMapData.playerKnowsAboutEnemy);
            se.OnDestroyed += OnDestroyed;

            ships.Add(se);
        }

        _ships.AddRange(ships);
        _fleets.Add(fleet);

        WorldUIManager.getInstance.CreateContactItem(
                "[FTL Wake]",
                "The wake of particles created by a fleet exiting super-FTL travel.",
                o,
                60f);
    }
    static void GenerateIncursion()
    {
        List<ShipEntity> ships = new List<ShipEntity>();

        var choices = Resources.LoadAll<ShipData>("");
        int count = Random.Range(2, 5);

        Vector3 o = Random.insideUnitSphere * (RuntimeData.localMapData.size * .75f);

        for (int i = 0; i < count; i++)
        {
            ShipEntity se = new Ship(choices.RandomItem()).Instantiate(o, Quaternion.identity, 1, false);
            se.OnDestroyed += OnDestroyed;

            ships.Add(se);
        }

        _ships.AddRange(ships);

        LogManager.getInstance.AddEntry("[<color=yellow>" + PlayerData.fleet.ships.RandomItem().name + "</color>]: A new unknown signal has been detected, Admiral.");
        WorldUIManager.getInstance.CreateContactItem(
                "[FTL Wake]",
                "The wake of particles created by a fleet exiting super-FTL travel.",
                o,
                60f);

        UpdateGameStatus();
    }

    public static ShipEntity[] GetSpecific(int teamID)
    {
        return _ships
            .Where(s => s.teamID == teamID)
            .ToArray();
    }
    public static ShipEntity[] GetExcluding(int excludingTeamID, bool discovered)
    {
        return _ships
            .Where(s => s.teamID != excludingTeamID && s.isDiscovered == discovered)
            .ToArray();
    }
    public static ShipEntity[] GetAllAI()
    {
        return _ships
            .Where(s => s.teamID != 0)
            .ToArray();
    }

    static void OnDestroyed(ShipEntity s)
    {
        OnShipDestroyed?.Invoke(s);

        _ships.Remove(s);

        LogManager.getInstance.AddEntry(s.teamID == 0 ? "The <i><color=yellow>" + s.name + "</color></i> has been lost with all souls, Admiral." : "Hostile vessel, <i><color=yellow>" + s.name + "</color></i> has been destroyed.");
        UpdateGameStatus();
    }
    static void UpdateGameStatus()
    {
        //player lost, gg no reEEEEEEEEEEEEEEEEEEEE
        if(_ships.All(s => s.teamID != 0))
        {
            RuntimeData.OnPlayerDefeat();
            PlayerData.OnPlayerDefeat();

            SetSimulationSpeed(SimulationSpeed.Normal);
            SceneManager.LoadSceneAsync("Main");
        }
        else if(_ships.All(s => s.teamID == 0))
        {
            for (int i = 0; i < _fleets.Count; i++)
                if (_fleets[i].teamID != 0)
                    RuntimeData.system.RemoveFleet(_fleets[i]);

            OnGameStatusUpdated?.Invoke(_ships.All(s => s.teamID == 0));
            LogManager.getInstance.AddEntry("Clear of hostile vessels, we can leave at anytime, Admiral.");
        }

        OnGameStatusUpdated?.Invoke(_ships.All(s => s.teamID == 0));
    }

    public static void Leave()
    {
        SetSimulationSpeed(SimulationSpeed.Normal);
        OnLeave?.Invoke();
        SceneManager.LoadSceneAsync("Overworld");
    }

    public delegate void GameStatusChangedEvent(bool safeToLeave);
    public static GameStatusChangedEvent OnGameStatusUpdated;
    public delegate void OnLeaveEvent();
    public static OnLeaveEvent OnLeave;

    public delegate void ShipDestroyedEvent(ShipEntity s);
    public static ShipDestroyedEvent OnShipDestroyed;

    public delegate void SimulationSpeedChangedEvent(SimulationSpeed currentSpeed);
    public static SimulationSpeedChangedEvent OnSimulationSpeedChanged;

    static IEnumerator TickAICommanders()
    {
        while (true)
        {
            ShipEntity[] ships = GetAllAI();
            ShipEntity[] player = GetSpecific(0);

            if (ships.Length == 0)
                yield return null;

            for (int i = 0; i < ships.Length; i++)
            {
                //pick a target
                ShipEntity target = ships[i].target == null ? player.RandomItem() : ships[i].target;
                ships[i].UpdateTarget(target);

                float d = Vector3.Distance(ships[i].transform.position, target.transform.position);

                Vector3 heading = ships[i].transform.position - target.transform.position;
                heading = heading.normalized;

                if (d <= ships[i].optimalTargetDistance)
                    ships[i].AddMove(new Move((target.transform.position + (heading * ships[i].optimalTargetDistance).Perpendicular(ships[i].transform.up)), ships[i]), true);
                else
                    ships[i].AddMove(new Move((target.transform.position + (heading * ships[i].optimalTargetDistance)), ships[i]), true);
            }

            yield return new WaitForSeconds(_aiTickRate);
        }
    }
}
public enum SimulationSpeed
{
    Stopped,
    Slow,
    Normal,
    Fast,
    Fastest
}