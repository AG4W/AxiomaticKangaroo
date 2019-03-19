using UnityEngine;

using Random = System.Random;
using System.Collections.Generic;
using System.Linq;

public class Fleet : PointOfInterest
{
    string _name;

    int _teamID;

    List<Ship> _ships;
    FleetVital[] _vitals;

    public int teamID { get { return _teamID; } }

    public List<Ship> ships { get { return _ships; } }

    public Fleet(string name, Cell cell, Random random, int teamID, List<Ship> ships) : base(name, cell, random)
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
        return base.prefab;
    }

    public override void OnLeftClick()
    {
        if (_teamID == 0)
            return;

        base.OnLeftClick();
    }

    public override void Move(Cell cell, float moveTime = 1)
    {
        float distance = base.cell.Distance(cell);

        //not enough movepoints
        if(distance > GetVital(FleetVitalType.Range).current)
        {
            Debug.Log("Too far away!");
            return;
        }
        if (GetVital(FleetVitalType.ProcessedFuel).current <= cell.travelCosts[0])
        {
            Debug.Log("Not enough fuel!");
            return;
        }

        GetVital(FleetVitalType.Range).Update(-distance);
        GetVital(FleetVitalType.ProcessedFuel).Update(cell.travelCosts[0]);
        GetVital(FleetVitalType.CivilianGoods).Update(cell.travelCosts[1]);

        cell.Occupy(this);

        base.cell.Leave(this);
        base.Move(cell, moveTime);

        if (GetVital(FleetVitalType.Range).current <= 0)
            OverworldManager.EndCurrentTurn();
    }

    public void AddShip(Ship s)
    {
        s.OnStatsUpdated += UpdateVitals;

        _ships.Add(s);

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

        _ships.Remove(s);

        if(_teamID == 0)
        {
            PlayerData.officers.Remove(s.officer);

            if (LogManager.getInstance != null)
                LogManager.getInstance.AddEntry("<i><color=yellow>" + s.name + "</color></i> has left the fleet.");
        }

        UpdateVitals();
    }

    public void OnTurnStart()
    {
        //reset
        GetVital(FleetVitalType.Range).Set(GetVital(FleetVitalType.Range).max);

        //apply changes
        for (int i = 0; i < _vitals.Length; i++)
            _vitals[i].Update(_vitals[i].changePerTurn);
    }

    void InitializeVitals()
    {
        _vitals = new FleetVital[]
        {
            new FleetVital(0f, FleetVitalType.ProcessedFuel),
            new FleetVital(0f, FleetVitalType.Ammunition),
            new FleetVital(0f, FleetVitalType.CivilianGoods),
            new FleetVital(0f, FleetVitalType.NebulaGas),
            new FleetVital(0f, FleetVitalType.Tritanite),
            new FleetVital(0f, FleetVitalType.Veldspar),
            new FleetVital(0f, FleetVitalType.Range),
        };

        for (int i = 0; i < _vitals.Length; i++)
            _vitals[i].OnVitalChanged += OnVitalChange;
    }
    void UpdateVitals()
    {
        Ship slowestShip = _ships.OrderBy(s => s.GetVital(VitalType.MovementSpeed)).FirstOrDefault();

        GetVital(FleetVitalType.ProcessedFuel).SetMax(_ships.Sum(s => s.GetVital(VitalType.FuelStorage)), false);
        GetVital(FleetVitalType.Ammunition).SetMax(_ships.Sum(s => s.GetVital(VitalType.GoodsStorage)), false);
        GetVital(FleetVitalType.CivilianGoods).SetMax(_ships.Sum(s => s.GetVital(VitalType.GoodsStorage)), false);
        GetVital(FleetVitalType.NebulaGas).SetMax(_ships.Sum(s => s.GetVital(VitalType.GasStorage)), false);
        GetVital(FleetVitalType.Tritanite).SetMax(_ships.Sum(s => s.GetVital(VitalType.OreStorage)), false);
        GetVital(FleetVitalType.Veldspar).SetMax(_ships.Sum(s => s.GetVital(VitalType.OreStorage)), false);
        GetVital(FleetVitalType.Range).SetMax(2, false);
    }

    void OnVitalChange(Vital vital)
    {
        OnVitalChanged?.Invoke(vital);
        FleetVital v = vital as FleetVital;

        if (v.type == FleetVitalType.Range && v.current <= 0)
            OverworldManager.EndCurrentTurn();
    }
    void OnVitalCritical(FleetVital vital)
    {
        switch (vital.type)
        {
            case FleetVitalType.ProcessedFuel:
                break;
            case FleetVitalType.Ammunition:
                break;
            case FleetVitalType.CivilianGoods:
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
            case FleetVitalType.Range:
                break;
            default:
                break;
        }
    }

    public FleetVital GetVital(FleetVitalType type)
    {
        return _vitals[(int)type];
    }

    public override string GetTooltip()
    {
        string s = "";

        s += _name + "\n\n";

        for (int i = 0; i < _ships.Count; i++)
            s += "<i><color=yellow>" + _ships[i].name + "</color></i>, " + _ships[i].GetClass() + (i < _ships.Count - 1 ? ",\n" : ".\n\n");

        s += "Power: <color=red>" + _ships.Sum(sh => sh.weapons.Sum(w => w.dps)).ToString("#.##") + "</color>.";

        return s;
    }

    public delegate void VitalEvent(Vital v);
    public event VitalEvent OnVitalChanged;
}
