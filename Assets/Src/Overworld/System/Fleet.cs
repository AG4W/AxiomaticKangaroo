using UnityEngine;

using Random = System.Random;
using System.Collections.Generic;
using System.Linq;

public class Fleet : PointOfInterest
{
    FleetVital[] _vitals;

    public int teamID { get; private set; }

    public List<Ship> ships { get; private set; }

    public Fleet(string name, Vector3 position, Random random, int teamID, List<Ship> ships) : base(name, position, random)
    {
        this.name = name;

        this.teamID = teamID;
        this.ships = ships;

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
        if (teamID == 0)
            return;

        base.OnLeftClick();
    }

    public override void Move(Vector3 position, float speed = 200)
    {
        float distance = Vector3.Distance(base.position, position);

        //not enough movepoints
        if(distance > GetVital(FleetVitalType.Range).current)
        {
            GetVital(FleetVitalType.Range).Set(0f);
            base.Move(Vector3.Lerp(base.position, position, GetVital(FleetVitalType.Range).current / distance), speed);

            OverworldManager.EndCurrentTurn();
        }
        else
        {
            GetVital(FleetVitalType.Range).Update(-distance);
            base.Move(position, speed);

            if (GetVital(FleetVitalType.Range).current <= 0)
                OverworldManager.EndCurrentTurn();
        }
    }

    public void AddShip(Ship s)
    {
        s.OnStatsUpdated += UpdateVitals;
        ships.Add(s);

        if(teamID == 0)
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
        ships.Remove(s);

        if(teamID == 0)
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
        Ship slowestShip = ships.OrderBy(s => s.GetVital(VitalType.MovementSpeed)).FirstOrDefault();

        GetVital(FleetVitalType.ProcessedFuel).SetMax(ships.Sum(s => s.GetVital(VitalType.FuelStorage)), false);
        GetVital(FleetVitalType.Ammunition).SetMax(ships.Sum(s => s.GetVital(VitalType.GoodsStorage)), false);
        GetVital(FleetVitalType.CivilianGoods).SetMax(ships.Sum(s => s.GetVital(VitalType.GoodsStorage)), false);
        GetVital(FleetVitalType.NebulaGas).SetMax(ships.Sum(s => s.GetVital(VitalType.GasStorage)), false);
        GetVital(FleetVitalType.Tritanite).SetMax(ships.Sum(s => s.GetVital(VitalType.OreStorage)), false);
        GetVital(FleetVitalType.Veldspar).SetMax(ships.Sum(s => s.GetVital(VitalType.OreStorage)), false);
        GetVital(FleetVitalType.Range).SetMax(400, false);
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

        s += base.name + "\n\n";

        for (int i = 0; i < ships.Count; i++)
            s += "<i><color=yellow>" + ships[i].name + "</color></i>, " + ships[i].GetClass() + (i < ships.Count - 1 ? ",\n" : ".\n\n");

        s += "Power: <color=red>" + ships.Sum(sh => sh.weapons.Sum(w => w.dps)).ToString("#.##") + "</color>.";

        return s;
    }

    public delegate void VitalEvent(Vital v);
    public event VitalEvent OnVitalChanged;
}
