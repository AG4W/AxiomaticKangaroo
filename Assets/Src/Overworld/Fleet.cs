using UnityEngine;
using UnityEngine.SceneManagement;

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
        UpdateDetectionAndMovement();
        return base.prefab;
    }

    public override void OnLeftClick()
    {
        if (_teamID == 0)
            return;

        base.OnLeftClick();
    }

    public void Intercept(Fleet interceptingFleet)
    {
        LocalMapData lmd = base.cell.localMapData;
        lmd.AddFleet(this);
        lmd.AddFleet(interceptingFleet);
        lmd.SetPlayerKnowsAboutEnemy(true);

        RuntimeData.SetLocalMapData(lmd);
        SceneManager.LoadSceneAsync("LocalMap");
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

    public void OnTurnEnd()
    {
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
        GetVital(FleetVitalType.Range).SetMax(slowestShip.GetVital(VitalType.MovementSpeed) * 200f, true);

        UpdateDetectionAndMovement();
        OnStatsUpdated?.Invoke(this);
    }
    void UpdateDetectionAndMovement()
    {
        if (base.prefab == null)
            return;

        //UpdateVisualization(
        //    base.prefab.transform.Find("detectionRange").GetComponent<LineRenderer>(), 
        //    GetVital(FleetVitalType.Detection).current,
        //    FleetVital.Color(FleetVitalType.Detection),
        //    _teamID != 0);

        UpdateVisualization(
            base.prefab.transform.Find("movementRange").GetComponent<LineRenderer>(), 
            GetVital(FleetVitalType.Range).max, 
            FleetVital.Color(FleetVitalType.Range),
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

        s += "Military Power: <color=red>" + _ships.Sum(sh => sh.weapons.Sum(w => w.dps)).ToString("#.##") + "</color>.\n";
        s += "\n" + base.GetTooltip();

        return s;
    }

    public delegate void FleetStatsEvent(Fleet fleet);
    public event FleetStatsEvent OnStatsUpdated;
}
