using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;

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

    List<PointOfInterest> _pois = new List<PointOfInterest>();
    List<Fleet> _occupants = new List<Fleet>();

    LocalMapData _localMapData;
    Vector3 _location;

    Random _random;

    bool _isOccupied;

    public int column { get { return _column; } }
    public int row { get { return _row; } }

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
    }
    public void RemovePointOfInterest(PointOfInterest poi)
    {
        _pois.Remove(poi);
    }

    public void Occupy(Fleet fleet)
    {
        _isOccupied = true;
        _occupants.Add(fleet);

        OnStatusChanged?.Invoke(_isOccupied);
    }
    public void Enter()
    {
        //clear old
        _localMapData.fleets.Clear();

        for (int i = 0; i < _occupants.Count; i++)
            _localMapData.AddFleet(_occupants[i]);

        RuntimeData.SetLocalMapData(_localMapData);
        SceneManager.LoadScene("LocalMap");
    }
    public void Leave(Fleet fleet)
    {
        _isOccupied = false;
        _occupants.Remove(fleet);

        OnStatusChanged?.Invoke(_isOccupied);
    }

    public string GetTooltip()
    {
        string s = "";

        s += "Sector: [" + _column + ", " + _row + "]\n\n";


        for (int i = 0; i < _pois.Count; i++)
            s += _pois[i].name + "\n";

        s += "Average resource caps:\n\n";

        s += FleetVital.Format(FleetVitalType.NebulaGas) + " " + _resourceDensities[0].ToString("0.##") + "%\n";
        s += FleetVital.Format(FleetVitalType.Veldspar) + " " + _resourceDensities[1].ToString("0.##") + "%\n";
        s += FleetVital.Format(FleetVitalType.Tritanite) + " " + _resourceDensities[2].ToString("0.##") + "%\n";

        return s;
    }

    public delegate void CellStatusEvent(bool status);
    public event CellStatusEvent OnStatusChanged;

    public Vector3 ToCubic()
    {
        int x = _column;
        int z = _row - (_column + (_column & 1)) / 2;
        int y = -x - z;

        return new Vector3(x, y, z);
    }
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
}
