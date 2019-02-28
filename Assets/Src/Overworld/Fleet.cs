using UnityEngine;
using UnityEngine.SceneManagement;

using Random = System.Random;
using System.Collections.Generic;
using System.Linq;

public class Fleet : PointOfInterest
{
    string _name;
    int _teamID;

    FleetVital[] _vitals;

    List<Ship> _ships;

    public int teamID { get { return _teamID; } }

    public List<Ship> ships { get { return _ships; } }

    public Fleet(string name, Vector3 location, Random random, int teamID, List<Ship> ships) : base(name, location, random)
    {
        _name = name;
        _teamID = teamID;
        _ships = ships;

        for (int i = 0; i < ships.Count; i++)
            ships[i].OnStatsUpdated += UpdateVitals;

        InitializeVitals();

        base.type = PointOfInterestType.Fleet;
        base.model = ModelDB.fleetEntity;
    }

    public override GameObject Instantiate()
    {
        base.Instantiate();
        UpdateDetectionAndMovement();
        return base.prefab;
    }

    public override void Move(Vector3 target, float moveTime = 1)
    {
        float d = Vector3.Distance(target, base.location);
        //start coroutine
        base.Move(base.location + (target - base.location).normalized * (d > GetVital(FleetVitalType.Movement).current ? GetVital(FleetVitalType.Movement).current : d), moveTime);

        if(_teamID != 0 && Vector3.Distance(base.location, PlayerData.fleet.location) <= GetVital(FleetVitalType.Detection).current)
            RuntimeData.system.aiEntities[_teamID - 1].OnPlayerDiscovered();
    }

    public override void OnLeftClick()
    {
        if (_teamID == 0)
            return;

        base.OnLeftClick();
    }

    public void Intercept(Fleet interceptingFleet)
    {
        LocalMapData lmd = base.GenerateLocalMapData();
        lmd.AddFleet(this);
        lmd.AddFleet(interceptingFleet);
        lmd.SetPlayerKnowsAboutEnemy(true);

        RuntimeData.SetLocalMapData(lmd);
        SceneManager.LoadSceneAsync("LocalMap");
    }

    public void AddShip(Ship s)
    {
        s.OnStatsUpdated += UpdateVitals;
        s.OnModifierAdded += OnModifierAdded;
        s.OnModifierRemoved += OnModifierRemoved;

        _ships.Add(s);

        for (int i = 0; i < s.modifiers.Count; i++)
            GetVital(s.modifiers[i].type).AddModifier(s.modifiers[i]);

        if(_teamID == 0)
        {
            if(s.officer != null)
                PlayerData.officers.Add(s.officer);

            if (LogManager.getInstance != null)
                LogManager.getInstance.AddEntry("<i><color=yellow>" + s.name + "</color></i> has joined the fleet.");
        }

        UpdateVitals();
    }
    public void RemoveShip(Ship s)
    {
        s.OnStatsUpdated -= UpdateVitals;
        s.OnModifierAdded -= OnModifierAdded;
        s.OnModifierRemoved -= OnModifierRemoved;

        _ships.Remove(s);

        for (int i = 0; i < s.modifiers.Count; i++)
            GetVital(s.modifiers[i].type).RemoveModifier(s.modifiers[i]);

        if(_teamID == 0)
        {
            PlayerData.officers.Remove(s.officer);

            if (LogManager.getInstance != null)
                LogManager.getInstance.AddEntry("<i><color=yellow>" + s.name + "</color></i> has left the fleet.");
        }

        UpdateVitals();
    }

    public void OnTurnEnd()
    {
        //update values with costs
        for (int i = 0; i < _vitals.Length; i++)
            _vitals[i].Update(_vitals[i].changePerTurn);

        //tick modifiers/converters
        for (int i = 0; i < _ships.Count; i++)
        {
            _ships[i].TickModifiers();
            _ships[i].TickConverters();
        }
    }

    public FleetVital GetVital(FleetVitalType type)
    {
        return _vitals[(int)type];
    }
    void InitializeVitals()
    {
        Ship slowestShip = _ships.OrderBy(s => s.GetVital(VitalType.MovementSpeed)).FirstOrDefault();

        _vitals = new FleetVital[]
        {
            new FleetVital(FleetVitalType.ProcessedFuel, _ships.Sum(s => s.GetVital(VitalType.FuelStorage))),
            new FleetVital(FleetVitalType.Ammunition, _ships.Sum(s => s.GetVital(VitalType.GoodsStorage))),
            new FleetVital(FleetVitalType.CivilianSupplies, _ships.Sum(s => s.GetVital(VitalType.GoodsStorage))),
            new FleetVital(FleetVitalType.NebulaGas, _ships.Sum(s => s.GetVital(VitalType.GasStorage))),
            new FleetVital(FleetVitalType.Veldspar, _ships.Sum(s => s.GetVital(VitalType.OreStorage))),
            new FleetVital(FleetVitalType.Tritanite, _ships.Sum(s => s.GetVital(VitalType.OreStorage))),
            new FleetVital(FleetVitalType.Movement, Mathf.Clamp(slowestShip == null ? 0f : 500f / (int)slowestShip.size, 0f, 400f)),
            new FleetVital(FleetVitalType.Detection, (200f - (ships.Count * 10f)) * 1.5f)
        };

        for (int i = 0; i < _vitals.Length; i++)
        {
            _vitals[i].OnCurrentChanged += delegate { OnStatsUpdated?.Invoke(this); };
            _vitals[i].OnMaxChanged += delegate { OnStatsUpdated?.Invoke(this); };
            _vitals[i].OnCurrentCritical += OnVitalCritical;
            _vitals[i].OnVitalUpdated += delegate { OnStatsUpdated?.Invoke(this); };
        }

        //add ship modifiers
        for (int i = 0; i < _ships.Count; i++)
            for (int j = 0; j < _ships[i].modifiers.Count; j++)
                GetVital(_ships[i].modifiers[j].type).AddModifier(_ships[i].modifiers[j]);
    }
    void UpdateVitals()
    {
        //recalculate fleet maximum values
        GetVital(FleetVitalType.ProcessedFuel).SetMax(_ships.Sum(s => s.GetVital(VitalType.FuelStorage)));
        GetVital(FleetVitalType.Ammunition).SetMax(_ships.Sum(s => s.GetVital(VitalType.GoodsStorage)));
        GetVital(FleetVitalType.CivilianSupplies).SetMax(_ships.Sum(s => s.GetVital(VitalType.GoodsStorage)));
        GetVital(FleetVitalType.NebulaGas).SetMax(_ships.Sum(s => s.GetVital(VitalType.GasStorage)));
        GetVital(FleetVitalType.Veldspar).SetMax(_ships.Sum(s => s.GetVital(VitalType.OreStorage)));
        GetVital(FleetVitalType.Tritanite).SetMax(_ships.Sum(s => s.GetVital(VitalType.OreStorage)));

        Ship slowestShip = _ships.OrderBy(s => s.GetVital(VitalType.MovementSpeed)).First();
        GetVital(FleetVitalType.Movement).SetMax(Mathf.Clamp(slowestShip == null ? 0f : 500f / (int)slowestShip.size, 0f, 400f), true);
        GetVital(FleetVitalType.Detection).SetMax((200f - (ships.Count * 10f)) * 1.5f);

        UpdateDetectionAndMovement();
        OnStatsUpdated?.Invoke(this);
    }
    void UpdateDetectionAndMovement()
    {
        if (base.prefab == null)
            return;

        UpdateVisualization(
            base.prefab.transform.Find("detectionRange").GetComponent<LineRenderer>(), 
            GetVital(FleetVitalType.Detection).current,
            FleetVital.Color(FleetVitalType.Detection),
            _teamID != 0);

        UpdateVisualization(
            base.prefab.transform.Find("movementRange").GetComponent<LineRenderer>(), 
            GetVital(FleetVitalType.Movement).max, 
            FleetVital.Color(FleetVitalType.Movement),
            true);
    }
    void UpdateVisualization(LineRenderer lr, float range, Color color, bool displayPlayer)
    {
        //create detection visualization
        lr.enabled = displayPlayer;

        if (!displayPlayer)
            return;

        int segments = 80;

        lr.positionCount = segments;
        lr.startWidth = 1f;
        lr.endWidth = 1f;
        lr.startColor = color;
        lr.endColor = color;

        float angle = 20f;
        float distance = range;

        for (int a = 0; a < segments; a++)
        {
            float x;
            float y = 0;
            float z;

            x = Mathf.Sin(Mathf.Deg2Rad * angle) * distance;
            z = Mathf.Cos(Mathf.Deg2Rad * angle) * distance;

            lr.SetPosition(a, new Vector3(x, y, z));

            angle += (360f / segments);
        }
    }

    void OnVitalCritical(FleetVital vital)
    {
        switch (vital.type)
        {
            case FleetVitalType.ProcessedFuel:
                break;
            case FleetVitalType.Ammunition:
                break;
            case FleetVitalType.CivilianSupplies:
                DialogueEvent disaster = EventDB.GetDisaster();

                if (base.random.Next(0, 100) * .01f <= disaster.probability)
                    DialogueUIManager.getInstance.DisplayDialogueEvent(disaster);
                break;
            case FleetVitalType.NebulaGas:
                break;
            case FleetVitalType.Veldspar:
                break;
            case FleetVitalType.Tritanite:
                break;
            case FleetVitalType.Movement:
                break;
            case FleetVitalType.Detection:
                break;
            default:
                break;
        }
    }
    void OnModifierAdded(FleetVitalModifier fvm)
    {
        GetVital(fvm.type).AddModifier(fvm);
        UpdateVitals();
    }
    void OnModifierRemoved(FleetVitalModifier fvm)
    {
        GetVital(fvm.type).RemoveModifier(fvm);
        UpdateVitals();
    }

    public override string GetTooltip()
    {
        string s = "";

        s += _name + "\n\n";

        for (int i = 0; i < _ships.Count; i++)
            s += "<i><color=yellow>" + _ships[i].name + "</color></i>, " + _ships[i].GetClass() + (i < _ships.Count - 1 ? ",\n" : ".\n\n");

        s += "Military Power: <color=red>" + _ships.Sum(sh => sh.weapons.Sum(w => w.dps)).ToString("#.##") + "</color>.\n";
        s += "\n" + base.GetTooltip();

        return s;
    }

    public delegate void FleetStatsEvent(Fleet fleet);
    public event FleetStatsEvent OnStatsUpdated;
}
