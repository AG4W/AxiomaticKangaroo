using UnityEngine;

using System.Collections.Generic;
using System.Linq;

public static class CommandMapper
{
    static WorldUIManager _uiManager;
    static CameraManager _cameraManager;
    
    static Camera _camera;
    static Vector3 _selectionBoxOrigin;
    static bool _selectionBoxIsOpen = false;

    static ShipGroup[] _groups = new ShipGroup[10];
    static List<ShipEntity> _selectedShips;

    public static bool hasSelection { get { return _selectedShips.Count > 0; } }
    public static bool hasGroups
    {
        get
        {
            for (int i = 0; i < _groups.Length; i++)
            {
                if (_groups[i] != null)
                    return true;
            }

            return false;
        }
    }

    public static void Initialize()
    {
        GameManager.OnShipDestroyed += OnShipDestroyed;

        _camera = Camera.main;
        _uiManager = WorldUIManager.getInstance;
        _cameraManager = CameraManager.getInstance;

        _selectedShips = new List<ShipEntity>();
        _uiManager.Initialize();
    }

    #region Unit Selection
    public static void SelectShip(ShipEntity ship, bool shiftModifier = false, bool controlModifier = false, int teamID = 0)
    {
        if (ship.teamID != teamID)
            return;

        for (int i = 0; i < _selectedShips.Count; i++)
            _selectedShips[i].Deselect();

        if (ship == null)
            _selectedShips.Clear();
        else if (shiftModifier)
            _selectedShips.Add(ship);
        else if (controlModifier)
            _selectedShips.Remove(ship);
        else
        {
            _selectedShips.Clear();
            _selectedShips.Add(ship);
        }

        for (int i = 0; i < _selectedShips.Count; i++)
            _selectedShips[i].Select();

        _uiManager.OnShipsSelected(_selectedShips);
    }
    public static void SelectShips(List<ShipEntity> ships, bool shiftModifier = false, bool controlModifier = false, int teamID = 0)
    {
        for (int i = 0; i < ships.Count; i++)
        {
            if (ships[i].teamID != teamID)
                ships.RemoveAt(i);
        }

        for (int i = 0; i < _selectedShips.Count; i++)
            _selectedShips[i].Deselect();

        if (ships == null)
            _selectedShips.Clear();
        else if (shiftModifier)
            _selectedShips.AddRange(ships);
        else if (controlModifier)
        {
            for (int i = 0; i < ships.Count; i++)
                _selectedShips.Remove(ships[i]);
        }
        else
        {
            _selectedShips.Clear();
            _selectedShips.AddRange(ships);
        }

        for (int i = 0; i < _selectedShips.Count; i++)
            _selectedShips[i].Select();

        _uiManager.OnShipsSelected(_selectedShips);
    }
    public static void ClearSelection()
    {
        for (int i = 0; i < _selectedShips.Count; i++)
            _selectedShips[i].Deselect();

        _selectedShips.Clear();
        _uiManager.OnShipsSelected(_selectedShips);
    }

    public static void CreateGroup(int index)
    {
        if (_selectedShips.Count == 0)
            return;

        //need to remove selected ships from any old group
        for (int i = 0; i < _groups.Length; i++)
        {
            if (i == index)
                continue;

            for (int j = 0; j < _selectedShips.Count; j++)
            {
                if (_groups[i] != null && _groups[i].Contains(_selectedShips[j]))
                {
                    _groups[i].Remove(_selectedShips[j]);

                    if (_groups[i].ships.Count == 0)
                        RemoveGroup(i);
                    else
                        OverwriteGroup(i, new ShipGroup(_groups[i].ships.ToList()));
                }
            }
        }

        if (_groups[index] != null)
            RemoveGroup(index);

        _groups[index] = new ShipGroup(_selectedShips.ToList());

        OnGroupCreated?.Invoke(index);
    }
    static void OverwriteGroup(int index, ShipGroup sg)
    {
        if (_groups[index] != null)
            RemoveGroup(index);

        _groups[index] = sg;

        OnGroupCreated?.Invoke(index);
    }

    public static void SelectGroup(int index)
    {
        if (_groups[index] == null)
            return;

        SelectShips(_groups[index].ships);
        OnGroupSelected?.Invoke(index);
    }
    public static void RemoveGroup(int index)
    {
        _groups[index] = null;

        OnGroupRemoved?.Invoke(index);
    }

    public static ShipGroup GetGroup(int index)
    {
        return _groups[index];
    }
    #endregion
    #region Unit Commands
    public static void AddMove(Vector3? position, bool clearQueue)
    {
        if (_selectedShips.Count == 0)
            return;

        List<Vector3> formation = FormationManager.Get((Vector3)position, _selectedShips[0].transform.forward, _selectedShips.Count).ToList();

        //need to map the closest ship to the closest point
        for (int i = 0; i < _selectedShips.Count; i++)
        {
            if(position != null)
            {
                int closestIndex = -1;

                float distance = Mathf.Infinity;

                for (int j = 0; j < formation.Count; j++)
                {
                    float d = Vector3.Distance(_selectedShips[i].transform.position, formation[j]);

                    if (closestIndex == -1 || d < distance)
                        closestIndex = j;
                }

                _selectedShips[i].AddMove(new Move(formation[closestIndex], _selectedShips[i]), clearQueue);
                formation.RemoveAt(closestIndex);
            }
            else
                _selectedShips[i].AddMove(new Move(position, _selectedShips[i]), clearQueue);
        }
    }

    public static void UpdateTarget(ShipEntity se)
    {
        for (int i = 0; i < _selectedShips.Count; i++)
            _selectedShips[i].UpdateTarget(se);
    }
    public static void UpdateMovePosition(Vector3 position)
    {
        if (_selectedShips.Count == 0)
            return;

        List<Vector3> formation = FormationManager.Get(position, _selectedShips[0].transform.forward, _selectedShips.Count).ToList();

        //move to closest position in formation
        for (int i = 0; i < _selectedShips.Count; i++)
        {
            Vector3 np = formation
                .OrderByDescending(v => Vector3.Distance(v, _selectedShips[i].transform.position))
                .First();

            formation.Remove(np);

            _selectedShips[i].UpdateMovePosition(np);
        }
    }
    public static void UpdateTargetSpeed(float percentage)
    {
        if (_selectedShips.Count == 1)
            _selectedShips[0].UpdateTargetSpeed(percentage);
        else
        {
            float speed = _selectedShips.OrderBy(s => s.GetVital(VitalType.MovementSpeed).max).First().GetVital(VitalType.MovementSpeed).max;
            speed *= percentage;

            for (int i = 0; i < _selectedShips.Count; i++)
                _selectedShips[i].UpdateTargetSpeed(speed / _selectedShips[i].GetVital(VitalType.MovementSpeed).max);
        }
    }
    public static void MatchSpeed()
    {
        ShipEntity slowest = _selectedShips.OrderBy(s => s.GetVital(VitalType.MovementSpeed).max).First();

        for (int i = 0; i < _selectedShips.Count; i++)
        {
            if (_selectedShips[i] == slowest)
                _selectedShips[i].UpdateTargetSpeed(1f);
            else
                _selectedShips[i].UpdateTargetSpeed(slowest.GetVital(VitalType.MovementSpeed).max / _selectedShips[i].GetVital(VitalType.MovementSpeed).max);
        }
    }
    public static void Afterburn()
    {
        for (int i = 0; i < _selectedShips.Count; i++)
            _selectedShips[i].Burn();
    }

    public static void SetResourceTarget(ResourceEntity re)
    {
        for (int i = 0; i < _selectedShips.Count; i++)
            for (int j = 0; j < _selectedShips[i].utilities.Length; j++)
                if(_selectedShips[i].utilities[j] is ResourceGatheringUtility)
                    ((ResourceGatheringUtility)_selectedShips[i].utilities[j]).SetTarget(re);
    }
    
    public static bool IsSelected(ShipEntity s)
    {
        return _selectedShips.IndexOf(s) != -1;
    }
    #endregion
    #region General Misc
    static void OnShipDestroyed(ShipEntity s)
    {
        if (_selectedShips.IndexOf(s) != -1)
            _selectedShips.Remove(s);

        for (int i = 0; i < _groups.Length; i++)
        {
            if (_groups[i] != null && _groups[i].Contains(s))
            {
                _groups[i].Remove(s);

                if (_groups[i].ships.Count == 0)
                    RemoveGroup(i);
                else
                    OverwriteGroup(i, new ShipGroup(_groups[i].ships.ToList()));
            }
        }
    }
    #endregion
    #region Game Management
    public static void IncrementSimulationSpeed()
    {
        GameManager.IncrementSimulationSpeed();
    }
    public static void DecrementSimulationSpeed()
    {
        GameManager.DecrementSimulationSpeed();
    }
    public static void ToggleSimulation()
    {
        GameManager.ToggleSimulation();
    }
    public static void SetSimulationSpeed(SimulationSpeed speed)
    {
        GameManager.SetSimulationSpeed(speed);
    }
    #endregion
    #region UI
    public static void OpenSelectionBox(Vector3 origin)
    {
        if (_selectionBoxIsOpen)
            return;

        _selectionBoxIsOpen = true;
        _selectionBoxOrigin = origin;
        _uiManager.OpenSelectionBox(origin);
    }
    static void OnSelectionBoxClose(Vector3 origin, Vector3 end)
    {
        Vector3 v1 = _camera.ScreenToViewportPoint(origin);
        Vector3 v2 = _camera.ScreenToViewportPoint(end);

        Vector3 min = Vector3.Min(v1, v2);
        Vector3 max = Vector3.Max(v1, v2);

        min.z = _camera.nearClipPlane;
        max.z = _camera.farClipPlane;

        Bounds bounds = new Bounds();
        bounds.SetMinMax(min, max);

        List<ShipEntity> ships = new List<ShipEntity>();

        for (int i = 0; i < GameManager.ships.Count; i++)
        {
            ShipEntity s = GameManager.ships[i];

            if (bounds.Contains(_camera.WorldToViewportPoint(s.transform.position)) && s.teamID == 0)
                ships.Add(s);
        }

        SelectShips(ships);
    }
    public static void CloseSelectionBox()
    {
        OnSelectionBoxClose(_selectionBoxOrigin, Input.mousePosition);

        _selectionBoxIsOpen = false;
        _uiManager.CloseSelectionBox();
    }

    public static void ShiftAlignmentPlane(float y)
    {
        AlignmentPlane.Shift(y);
    }
    public static void SetAlignmentPlane(float y)
    {
        AlignmentPlane.Set(y);
    }
    public static void ToggleAlignmentPlane()
    {
        AlignmentPlane.Toggle();
    }
    #endregion

    public delegate void GroupEvent(int index);
    public static GroupEvent OnGroupCreated;
    public static GroupEvent OnGroupSelected;
    public static GroupEvent OnGroupRemoved;
}
public enum SpeedSetting
{
    Full,
    ThreeQuarters,
    Half,
    Silent,
    Match
}