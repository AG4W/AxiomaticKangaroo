using UnityEngine;

using System.Collections.Generic;

[RequireComponent(typeof(ShipVisualEntity))]
public class ShipEntity : MonoBehaviour
{
    string _name;
    int _teamID;

    Ship _ship;
    ShipEntity _target;
    List<ShipEntity> _availableTargets = new List<ShipEntity>();

    //recycled arrays
    ShipEntity[] _targetCandidates;
    ShipEntity[] _scanCandidates;

    #region Data
    Vital[] _vitals;

    float _scanTimer = 0f;
    float _optimalTargetDistance;

    Weapon[] _weapons;
    Utility[] _utilities;
    ShipComponent[] _components;
    #endregion
    #region Movement
    Vector3? _heading;
    Vector3? _movePosition;

    Vector3 _lastPosition;

    Move _currentMove = null;
    List<Move> _moves = new List<Move>();
    #endregion

    bool _isDiscovered = false;

    public new string name { get { return _name; } }
    public int teamID { get { return _teamID; } }

    public float speed { get { return (this.transform.position - _lastPosition).magnitude; } }
    public float optimalTargetDistance { get { return _optimalTargetDistance; } }

    public Weapon[] weapons { get { return _weapons; } }
    public Utility[] utilities { get { return _utilities; } }
    public ShipComponent[] components { get { return _components; } }

    public Ship ship { get { return _ship; } }
    public ShipEntity target { get { return _target; } }

    public bool isDiscovered { get { return _isDiscovered; } }

    public void Initialize(Ship ship, int teamID, bool isDiscovered)
    {
        _ship = ship;

        _name = ship.name;
        _teamID = teamID;
        _isDiscovered = isDiscovered;

        _vitals = new Vital[System.Enum.GetValues(typeof(VitalType)).Length];

        for (int i = 0; i < _vitals.Length; i++)
            _vitals[i] = new Vital(ship.GetVital((VitalType)i), (VitalType)i);

        //setup events
        for (int i = 0; i < _vitals.Length; i++)
            _vitals[i].OnVitalChanged += OnVitalChange;

        _weapons = ship.weapons;
        _utilities = ship.utilities;
        _components = ship.components;

        for (int i = 0; i < _components.Length; i++)
            _components[i].Initialize(this);

        this.GetComponent<ShipVisualEntity>().Initialize(this, ship.destroyedVFX);
        this.transform.Find("shipUIEntity").GetComponent<ShipUIEntity>().Initialize(this);

        SetOptimalDistance();
        Deselect();
    }

    void Update()
    {
        Scan();
        UpdateAvailableTargets();

        UpdateCurrentCommand();
        UpdateLocomotion();

        CycleAutoActivates();

        _lastPosition = this.transform.position;
    }

    void UpdateLocomotion()
    {
        //if we have a movepos, move and rotate towards it
        if (_movePosition != null)
        {
            _heading = (Vector3)_movePosition - this.transform.position;

            Translate();
            Rotate();
        }
    }
    void Translate()
    {
        this.transform.position += 
            this.transform.forward * GetVital(VitalType.MovementSpeed).current * Time.deltaTime;
    }
    void Rotate()
    {
        this.transform.rotation = Quaternion.Lerp(
            this.transform.rotation, Quaternion.LookRotation((Vector3)_heading), GetVital(VitalType.RotationSpeed).current * Time.deltaTime);
    }

    void Scan()
    {
        _scanTimer += Time.deltaTime;

        if(_scanTimer >= GetVital(VitalType.ScanRate).current)
        {
            _scanTimer = 0f;

            _scanCandidates = GameManager.GetExcluding(_teamID, false);

            //scan for enemies
            for (int i = 0; i < _scanCandidates.Length; i++)
            {
                float d = Vector3.Distance(this.transform.position, _scanCandidates[i].transform.position);

                if (d <= GetVital(VitalType.ScanRadius).current)
                {
                    if(_teamID == 0)
                        LogManager.getInstance.AddEntry("[" + this.name + "]: Hostile contact! Distance: " + d.ToString("#") + " units.", 15f, EntryType.Combat);

                    _scanCandidates[i].SetDiscoveredStatus(true);
                }
            }
            //scan for resources
            for (int i = 0; i < GameManager.resources.Count; i++)
            {
                if (GameManager.resources[i] == null || GameManager.resources[i].isDiscovered && GameManager.resources[i].isScanned)
                    continue;

                float d = Vector3.Distance(this.transform.position, GameManager.resources[i].transform.position);

                if (d <= (ship.scanner == null ? 1000 : ship.scanner.range))
                {
                    GameManager.resources[i].Discover();

                    if (ship.scanner != null)
                        GameManager.resources[i].Scan();
                }
            }
        }
    }
    void UpdateAvailableTargets()
    {
        _availableTargets.Clear();
        _targetCandidates = GameManager.GetExcluding(_teamID, true);

        for (int i = 0; i < _targetCandidates.Length; i++)
        {
            if (Vector3.Distance(_targetCandidates[i].transform.position, this.transform.position) <= _optimalTargetDistance)
                _availableTargets.Add(_targetCandidates[i]);
        }

        if (_target != null && _availableTargets.IndexOf(_target) != -1)
        {
            _availableTargets.Remove(_target);
            _availableTargets.Insert(0, _target);
        }
    }
    void CycleAutoActivates()
    {
        for (int i = 0; i < _availableTargets.Count; i++)
        {
            if(_availableTargets[i])

            for (int j = 0; j < _weapons.Length; j++)
            {
                if(!_weapons[j].hasCooldown && _weapons[j].autoActivate)
                    _weapons[j].AttemptFire(_availableTargets[i], this);
            }
        }

        for (int i = 0; i < _utilities.Length; i++)
        {
            if (!_utilities[i].hasCooldown && _utilities[i].autoActivate)
                _utilities[i].AttemptActivate();
        }
    }

    public void Select()
    {
        OnSelected?.Invoke();
    }
    public void Deselect()
    {
        OnDeselected?.Invoke();
    }

    void UpdateCurrentCommand()
    {
        if(_currentMove == null)
        {
            if(_moves.Count > 0)
            {
                SetCurrentCommand(_moves[0]);
                _moves.RemoveAt(0);
            }
        }
        else
        {
            if (_currentMove.Status())
            {
                if (_moves.Count > 0)
                {
                    SetCurrentCommand(_moves[0]);
                    _moves.RemoveAt(0);
                }
                else
                    SetCurrentCommand();
            }
        }
    }
    void SetCurrentCommand(Move m = null)
    {
        _currentMove = m;

        if (_currentMove != null)
            _currentMove.Execute();
        else
        {
            _movePosition = null;
            _heading = null;
        }

        OnMovesUpdated?.Invoke(_moves, _currentMove);
    }
    public void AddMove(Move m, bool clearQueue)
    {
        if (clearQueue)
        {
            _moves.Clear();
            _currentMove = null;
        }

        _moves.Add(m);

        OnMovesUpdated?.Invoke(_moves, _currentMove);
    }

    public void UpdateMovePosition(Vector3? position = null)
    {
        _movePosition = position;
    }
    public void UpdateTarget(ShipEntity target)
    {
        //cant target teammates
        if (target.teamID == _teamID)
            return;

        _target = target;

        if(_teamID == 0)
            LogManager.getInstance.AddEntry("[" + this.name + "]: " + "targeting " + target.name + ".");

        OnTargetChanged?.Invoke(target);
    }
    public void UpdateCurrentSpeed(float percentageOfMax)
    {
        GetVital(VitalType.MovementSpeed).Update(GetVital(VitalType.MovementSpeed).max * percentageOfMax);
    }
    public void SetCurrentSpeed(float value)
    {
        GetVital(VitalType.MovementSpeed).Set(value);
    }

    public void ApplyDamage(float amount)
    {
        Vital shield = GetVital(VitalType.ShieldPoints);
        Vital hull = GetVital(VitalType.HullPoints);

        if (hull.current <= 0f)
            return;

        if (shield.current >= amount)
            shield.Update(-amount);
        else if (shield.current > 0)
        {
            amount -= shield.current;

            shield.Update(-shield.current);
            hull.Update(-amount);
        }
        else
            hull.Update(-amount);

        OnVitalChanged?.Invoke(VitalType.ShieldPoints, shield.inPercent);
        OnVitalChanged?.Invoke(VitalType.HullPoints, hull.inPercent);
    }
    public void SetDiscoveredStatus(bool status)
    {
        _isDiscovered = status;

        OnDiscovered?.Invoke(status);
    }

    public Vital GetVital(VitalType vt)
    {
        return _vitals[(int)vt];
    }
    void OnVitalChange(VitalType v, float current)
    {
        OnVitalChanged?.Invoke(v, current);

        //hook in audio here
        if(v == VitalType.HullPoints && current <= 0f)
            OnShipDestroyed();
    }
    void OnShipDestroyed()
    {
        OnDestroyed?.Invoke(this);
        Destroy(this.gameObject);
    }

    void SetOptimalDistance()
    {
        for (int i = 0; i < _weapons.Length; i++)
        {
            float range = _weapons[i].range;

            if (_optimalTargetDistance == 0 || range < _optimalTargetDistance)
                _optimalTargetDistance = range;
        }
    }

    public delegate void VitalChangedEvent(VitalType v, float current);
    public event VitalChangedEvent OnVitalChanged;

    public delegate void TargetChangedEvent(ShipEntity t);
    public event TargetChangedEvent OnTargetChanged;

    public delegate void DiscoveredEvent(bool hasBeenDiscovered);
    public event DiscoveredEvent OnDiscovered;

    public delegate void DestroyedEvent(ShipEntity s);
    public event DestroyedEvent OnDestroyed;

    public delegate void MoveEvent(List<Move> moves, Move current);
    public event MoveEvent OnMovesUpdated;

    public delegate void SelectionEvent();
    public event SelectionEvent OnSelected;
    public event SelectionEvent OnDeselected;
}