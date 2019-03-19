using UnityEngine;
using UnityEngine.SceneManagement;

using Random = System.Random;
using System.Collections.Generic;
using System.Linq;

public class Cell
{
    int _column;
    int _row;

    float[] _resourceDensities = new float[]
        {
            0f,
            0f,
            0f
        };
    float[] _travelCosts = new float[]
    {
        -1f,
        -.1f
    };

    List<PointOfInterest> _pois = new List<PointOfInterest>();
    List<Fleet> _fleets = new List<Fleet>();

    LocalMapData _localMapData;
    Vector3 _location;

    Random _random;

    bool _isOccupied;

    public int column { get { return _column; } }
    public int row { get { return _row; } }

    public float[] travelCosts { get { return _travelCosts; } }

    public LocalMapData localMapData { get { return _localMapData; } }
    public Vector3 location { get { return _location; } }

    public bool isOccupied { get { return _isOccupied; } }

    public Cell(int column, int row)
    {
        _column = column;
        _row = row;

        _random = new Random(column * row);

        _location = this.ToWorld();
        _isOccupied = false;

        _localMapData = new LocalMapData("Sector [" + _column + ", " + _row + "]", _resourceDensities, _location);
    }

    public void AddPointOfInterest(PointOfInterest poi)
    {
        _pois.Add(poi);

        if (poi is Celestial)
            _localMapData.AddCelestial(poi as Celestial);

        if (poi.type == PointOfInterestType.Nebula || poi.type == PointOfInterestType.Star)
            _resourceDensities[0] += _random.NextFloat(.5f, .8f);
        else if (poi.type == PointOfInterestType.Planet)
        {
            _resourceDensities[1] += _random.NextFloat(0f, .5f);
            _resourceDensities[2] += _random.NextFloat(0f, .25f);
        }
    }
    public void RemovePointOfInterest(PointOfInterest poi)
    {
        if (poi is Celestial)
            _localMapData.RemoveCelestial(poi as Celestial);

        _pois.Remove(poi);
    }

    public void Occupy(Fleet fleet)
    {
        _isOccupied = true;
        _pois.Add(fleet);
        _fleets.Add(fleet);

        OnStatusChanged?.Invoke(_isOccupied);
    }
    public void Enter()
    {
        //clear old
        _localMapData.SetFleets(_fleets);

        RuntimeData.SetLocalMapData(_localMapData);
        HexGrid.Enter();
        SceneManager.LoadScene("LocalMap");
    }
    public void Leave(Fleet fleet)
    {
        _isOccupied = false;
        _pois.Remove(fleet);
        _fleets.Remove(fleet);

        OnStatusChanged?.Invoke(_isOccupied);
    }

    public string GetDetails()
    {
        string s = "";

        s += "Sector: [" + _column + ", " + _row + "]\n";
        s += "Distance: " + this.Distance(PlayerData.fleet.cell) + "\n\n";
        s += "Travel Cost:\n";
        s += _travelCosts[0].ToString(" +0.##;-0.##") + " " + FleetVital.Format(FleetVitalType.ProcessedFuel) + ".\n\n";

        if (_pois.Count > 0)
        {
            s += "Points of Interest:\n";

            for (int i = 0; i < _pois.Count; i++)
            {
                if (_pois[i].type == PointOfInterestType.Fleet)
                    s += "- " + _pois[i].name + " (" + _pois[i].type + "), power: <color=red> " + ((Fleet)_pois[i]).ships.Sum(sh => sh.weapons.Sum(w => w.dps)).ToString("#.##") + "</color>.";
                else
                    s += "- " + _pois[i].name + " (" + _pois[i].type + ").\n";
            }

            s += "\n";
        }

        return s;
    }
    public string GetResources()
    {
        string s = "";

        s += _localMapData.hasResources ? "Resources:\n" : "No Resources.";

        if (_localMapData.hasResources)
        {
            s += "- " + FleetVital.Format(FleetVitalType.NebulaGas) + " " + (_resourceDensities[0] * 100f).ToString("0.##") + ".\n";
            s += "- " + FleetVital.Format(FleetVitalType.Veldspar) + " " + (_resourceDensities[1] * 100f).ToString("0.##") + ".\n";
            s += "- " + FleetVital.Format(FleetVitalType.Tritanite) + " " + (_resourceDensities[2] * 100f).ToString("0.##") + ".";
        }

        return s;
    }

    public delegate void CellStatusEvent(bool status);
    public event CellStatusEvent OnStatusChanged;

    public Vector3 ToWorld()
    {
        Vector3 position;

        position.x = _column * (HexMetrics.width * .75f);
        position.y = 0f;
        position.z = _row * HexMetrics.height;

        if (Mathf.Abs(_column) % 2 != 0)
            position.z += HexMetrics.height / 2;

        return position;
    }
    public Vector3 ToCubic()
    {
        int x = _column;
        int z = _row - (_column + (_column & 1)) / 2;
        int y = -x - z;

        return new Vector3(x, y, z);
    }

    public float Distance(Cell other)
    {
        return DistanceCubic(this.ToCubic(), other.ToCubic());
    }
    float DistanceCubic(Vector3 a, Vector3 b)
    {
        return (Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z)) / 2f;
    }
}
public enum GridDirection
{
    North,
    NorthEast,
    SouthEast,
    South,
    SouthWest,
    NorthWest
}