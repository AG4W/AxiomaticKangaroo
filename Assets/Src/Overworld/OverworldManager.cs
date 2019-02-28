using UnityEngine;

using Random = System.Random;
using System.Collections.Generic;
using System.Linq;

public static class OverworldManager
{
    static OverworldUIManager _uiManager;
    static OverworldCameraManager _cameraManager;

    static Random _random;

    static int _turnIndex = -1;
    static int _turnCount = 0;

    static float _spawnChance;

    static bool _isPlayerTurn = false;

    public static int turnCount { get { return _turnCount; } }
    public static bool isPlayerTurn { get { return _isPlayerTurn; } }

    public static void Initialize()
    {
        CleanUp();

        if (RuntimeData.system == null)
            RuntimeData.GenerateNewSystem();

        RuntimeData.system.Instantiate();

        if (!RuntimeData.system.pointsOfInterest.Contains(PlayerData.fleet))
            RuntimeData.system.AddPointOfInterest(PlayerData.fleet);

        _random = new Random(RuntimeData.system.seed);

        //add slot for player
        //this was fucking retarded

        _uiManager = OverworldUIManager.getInstance;
        _uiManager.Initialize();

        _cameraManager = OverworldCameraManager.getInstance;
        _cameraManager.JumpTo(PlayerData.fleet.location);

        ToolbarManager.getInstance.Initialize();

        //loading complete callback
        if (!ProgressData.Evaluate(ProgressPoint.ReceivedStartEvent))
            DialogueUIManager.getInstance.DisplayDialogueEvent(EventDB.GetStart());

        UpdateSpawnChance();
        EndCurrentTurn();
    }

    public static void EndCurrentTurn()
    {
        //before turn ends, apply player fleet changes
        if (_isPlayerTurn)
            PlayerData.fleet.OnTurnEnd();

        //turn has ended, increment turn index
        if (_turnIndex == RuntimeData.system.aiEntities.Count)
            _turnIndex = 0;
        else
            _turnIndex++;

        _isPlayerTurn = _turnIndex == 0;

        //roll for hostile fleet spawn
        if (_isPlayerTurn)
        {
            _turnCount++;

            if (_spawnChance > 0 && _random.Next(0, 101) <= _spawnChance)
                GenerateEnemyFleet();

            UpdateSpawnChance();
        }
        else
            RuntimeData.system.aiEntities[_turnIndex - 1].Execute();
    }
    static void UpdateSpawnChance()
    {
        _spawnChance = 0;
        //5% base
        //can be mitigated perhaps?
        //_spawnChance += 5f;
        //+1% per turn after 10 turns
        if (_turnCount - 10 > 0)
            _spawnChance += _turnCount - 10;

        //add some other modifiers here

        //+25% per hostile fleet already in system
        if (RuntimeData.system.aiEntities.Count > 0)
            _spawnChance += RuntimeData.system.aiEntities[0].fleetCount * (RuntimeData.system.aiEntities[0].knowsAboutPlayer ? 20 : -20);

        _spawnChance = Mathf.Clamp(_spawnChance, 0, 100);
    }
    static void GenerateEnemyFleet()
    {
        AIPlanner ap;

        if(RuntimeData.system.aiEntities.Count == 0)
        {
            ap = new AIPlanner();
            RuntimeData.system.aiEntities.Add(ap);
        }
        else
            ap = RuntimeData.system.aiEntities[0];

        //generate fleet
        List<Ship> ships = new List<Ship>();

        int shipCount = _random.Next(0, ap.fleetCount) + 1;
        var choices = Resources.LoadAll<ShipData>("Data/Premade Ships/")
            .Where(sd => (int)sd.hull.size <= shipCount)
            .ToArray();

        for (int i = 0; i < shipCount; i++)
            ships.Add(new Ship(choices.RandomItem()));

        string efn = RuntimeData.save.data.enemy.name + " " + (shipCount < 4 ? (shipCount < 2 ? "Scout" : "Vanguard") : "Invasion Fleet");

        Fleet ef = new Fleet(
            "<color=red>" + efn + "</color>",
            RuntimeData.system.GetRandomLocation(),
            new Random(RuntimeData.system.seed),
            1,
            ships);

        RuntimeData.system.AddPointOfInterest(ef);
        ap.AddFleet(ef);

        LogManager.getInstance.AddEntry("A " + efn + " has entered the system.", 15f, EntryType.Combat);
    }

    static void CleanUp()
    {
        _turnIndex = 0;
        _turnCount = 0;
        _isPlayerTurn = false;
        _random = null;
    }
}