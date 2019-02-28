using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;

public class ShipUIEntity : MonoBehaviour
{
    [SerializeField]int _scanVisualizationResolution = 80;

    Text _name;

    Image _hull;
    Image _shield;

    [SerializeField]GameObject _moveObject;
    [SerializeField]GameObject _alignmentObject;
    [SerializeField]GameObject _worldUIObject;
    GameObject _worldUIInstance;

    [SerializeField]Material _lineMaterial;

    LineRenderer _alignmentVisualization;
    LineRenderer _movementVisualization;
    LineRenderer _scanVisualization;
    LineRenderer _targetVisualization;

    List<GameObject> _moveObjects = new List<GameObject>();

    Vector3 _visualizationXAngles = Vector3.zero;

    bool _isVisible = false;

    ShipEntity _entity;
    ShipEntity _target;
    Camera _camera;

    public void Initialize(ShipEntity entity)
    {
        _entity = entity;
        _camera = Camera.main;
        _alignmentObject = Instantiate(_alignmentObject, this.transform);

        InitializeWorldUI();
        InitializeAlignmentVisualization();
        InitializeMoveVisualization();
        InitializeScanVisualization();
        InitializeTargetVisualization();

        entity.OnSelected += OnSelected;
        entity.OnDeselected += OnDeselected;
        entity.OnMovesUpdated += OnMovesUpdated;
        entity.OnVitalChanged += OnVitalChanged;
        entity.OnDiscovered += UpdateWorldUI;
        entity.OnTargetChanged += OnTargetChanged;
        entity.OnDestroyed += OnDestroyed;
    }

    void Update()
    {
        Vector3 p = _camera.WorldToViewportPoint(this.transform.position);
        Vector3 op = _camera.WorldToScreenPoint(this.transform.position);
        op.z = 0;

        _worldUIInstance.transform.position = op;
        _worldUIInstance.SetActive(p.x > 0 && p.x < 1 && p.y > 0 && p.y < 1 && p.z > 0);

        _alignmentObject.transform.position = new Vector3(_entity.transform.position.x, AlignmentPlane.y, _entity.transform.position.z);
        _alignmentObject.transform.eulerAngles = Vector3.zero;

        _alignmentVisualization.transform.eulerAngles = Vector3.zero;
        _alignmentVisualization.SetPosition(0, _entity.transform.position);
        _alignmentVisualization.SetPosition(1, new Vector3(_entity.transform.position.x, AlignmentPlane.y, _entity.transform.position.z));
        
        if(_entity.teamID == 0)
        {
            //_moveVisualizations.transform.position = _entity.transform.position;
            _movementVisualization.transform.eulerAngles = Vector3.zero;
            //fix position
            if (_movementVisualization.positionCount > 0)
                _movementVisualization.SetPosition(0, _entity.transform.position);

            //_lineRendererX.transform.position = _entity.transform.position;
            _scanVisualization.transform.eulerAngles = _visualizationXAngles;

            //_lineRendererY.transform.position = _entity.transform.position;
            //_lineRendererY.transform.eulerAngles = _visualizationYAngles;

            if (_targetVisualization.enabled)
            {
                if (_target != null)
                {
                    _targetVisualization.SetPosition(0, _entity.transform.position);
                    _targetVisualization.SetPosition(1, _target.transform.position);
                }
                else if (_target == null)
                    _targetVisualization.enabled = false;
            }
        }
    }

    void OnVitalChanged(VitalType vt, float currentInPercent)
    {
        if (vt == VitalType.HullPoints)
            _hull.fillAmount = currentInPercent;
        else if (vt == VitalType.ShieldPoints)
            _shield.fillAmount = currentInPercent;
    }
    void OnMovesUpdated(List<Move> moves, Move current)
    {
        //clear old objects
        for (int i = 0; i < _moveObjects.Count; i++)
            Destroy(_moveObjects[i]);

        _moveObjects.Clear();

        if (current == null || moves == null)
        {
            _movementVisualization.positionCount = 0;
            return;
        }

        List<Vector3> points = new List<Vector3>();

        points.Add(_entity.transform.position);

        if(current != null)
            points.Add(current.position);

        for (int i = 0; i < moves.Count; i++)
            points.Add(moves[i].position);

        //create moveObjects
        //skip entity position
        for (int i = 1; i < points.Count; i++)
        {
            GameObject mo = Instantiate(_moveObject, points[i], Quaternion.identity, null);
            mo.SetActive(_isVisible);

            _moveObjects.Add(mo);
        }

        _movementVisualization.positionCount = points.Count;
        _movementVisualization.SetPositions(points.ToArray());
    }

    void OnSelected()
    {
        _isVisible = true;

        UpdateVisibility();
    }
    void OnDeselected()
    {
        _isVisible = false;

        UpdateVisibility();
    }
    void OnTargetChanged(ShipEntity t)
    {
        _target = t;

        if (_target == null)
            _targetVisualization.enabled = false;
    }
    void OnDestroyed(ShipEntity s)
    {
        _entity.OnSelected -= OnSelected;
        _entity.OnDeselected -= OnDeselected;
        _entity.OnMovesUpdated -= OnMovesUpdated;
        _entity.OnDiscovered -= UpdateWorldUI;
        _entity.OnVitalChanged -= OnVitalChanged;
        _entity.OnTargetChanged -= OnTargetChanged;
        _entity.OnDestroyed -= OnDestroyed;

        for (int i = 0; i < _moveObjects.Count; i++)
            Destroy(_moveObjects[i]);

        Destroy(_worldUIInstance);
        Destroy(_alignmentObject);
        Destroy(_alignmentVisualization);
        Destroy(_movementVisualization);
        Destroy(_targetVisualization);
        Destroy(this);
    }

    void UpdateVisibility()
    {
        //_alignmentObject.SetActive(_isVisible);
        //_alignmentVisualization.enabled = _isVisible;

        _worldUIInstance.transform.Find("locator").gameObject.SetActive(_isVisible);
        //_worldUIObject.transform.Find("name").GetComponent<Text>().color = _isVisible ? Color.white : Color.gray;

        _movementVisualization.enabled = _isVisible;
        _scanVisualization.enabled = _isVisible;
        _targetVisualization.enabled = _isVisible;

        for (int i = 0; i < _moveObjects.Count; i++)
            _moveObjects[i].SetActive(_isVisible);
    }

    void InitializeWorldUI()
    {
        _worldUIInstance = Instantiate(_worldUIObject, GameObject.Find("UI").transform.Find("Canvas"));
        _worldUIInstance.GetComponent<GenericTooltipHandler>().Initialize(
            OnMouseEnter,
            OnLeftClick,
            OnScrollClick,
            OnRightClick,
            OnMouseExit);

        _name = _worldUIInstance.transform.Find("name").GetComponent<Text>();

        _hull = _worldUIInstance.transform.Find("hull").GetComponent<Image>();
        _shield = _worldUIInstance.transform.Find("shield").GetComponent<Image>();
        _hull.gameObject.SetActive(_entity.isDiscovered);
        _shield.gameObject.SetActive(_entity.isDiscovered);

        _hull.fillAmount = _entity.GetVital(VitalType.HullPoints).inPercent;
        _shield.fillAmount = _entity.GetVital(VitalType.ShieldPoints).inPercent;

        _name.text = _entity.isDiscovered ? "'<i><color=" + (_entity.teamID == 0 ? "white" : "red") + ">" + _entity.name + "</color></i>'" : "[Unknown Signal]";
    }
    void UpdateWorldUI(bool isDiscovered)
    {
        if (isDiscovered)
            _worldUIInstance.GetComponent<GenericTooltipHandler>().UpdateActions(
                OnMouseEnter,
                OnLeftClick,
                OnScrollClick,
                OnRightClick,
                OnMouseExit);

        _name = _worldUIInstance.transform.Find("name").GetComponent<Text>();

        _hull = _worldUIInstance.transform.Find("hull").GetComponent<Image>();
        _shield = _worldUIInstance.transform.Find("shield").GetComponent<Image>();
        _hull.gameObject.SetActive(isDiscovered);
        _shield.gameObject.SetActive(isDiscovered);

        _hull.fillAmount = _entity.GetVital(VitalType.HullPoints).inPercent;
        _shield.fillAmount = _entity.GetVital(VitalType.ShieldPoints).inPercent;

        _name.text = _entity.isDiscovered ? "'<i><color=" + (_entity.teamID == 0 ? "white" : "red") + ">" + _entity.name + "</color></i>'" : "[Unknown Signal]";
    }
    void InitializeAlignmentVisualization()
    {
        Transform t = this.transform.Find("alignmentVisualization");
        t.SetParent(_entity.transform);
        t.position = _entity.transform.position;

        _alignmentVisualization = t.GetComponent<LineRenderer>();
        _alignmentVisualization.material = _lineMaterial;
        _alignmentVisualization.loop = false;
        _alignmentVisualization.useWorldSpace = true;
        _alignmentVisualization.positionCount = 2;
        _alignmentVisualization.startWidth = .1f;
        _alignmentVisualization.endWidth = .1f;
    }
    void InitializeMoveVisualization()
    {
        Transform t = this.transform.Find("moveVisualization");
        t.SetParent(_entity.transform);
        t.position = _entity.transform.position;

        _movementVisualization = t.GetComponent<LineRenderer>();
        _movementVisualization.material = _lineMaterial;
        _movementVisualization.loop = false;
        _movementVisualization.useWorldSpace = true;
        _movementVisualization.positionCount = 0;
        _movementVisualization.startWidth = .1f;
        _movementVisualization.endWidth = .1f;
    }
    void InitializeScanVisualization()
    {
        Transform t = this.transform.Find("scanVisualization");
        t.SetParent(_entity.transform);
        t.position = _entity.transform.position;

        _scanVisualization = t.GetComponent<LineRenderer>();
        _scanVisualization.material = _lineMaterial;
        _scanVisualization.loop = true;
        _scanVisualization.useWorldSpace = false;
        _scanVisualization.positionCount = _scanVisualizationResolution;
        _scanVisualization.startWidth = .1f;
        _scanVisualization.endWidth = .1f;

        float angle = 20f;
        float distance = _entity.GetVital(VitalType.ScanRadius).current;

        for (int a = 0; a < _scanVisualizationResolution; a++)
        {
            float x;
            float y = 0;
            float z;

            x = Mathf.Sin(Mathf.Deg2Rad * angle) * distance;
            z = Mathf.Cos(Mathf.Deg2Rad * angle) * distance;

            _scanVisualization.SetPosition(a, new Vector3(x, y, z));

            angle += (360f / _scanVisualizationResolution);
        }
    }
    void InitializeTargetVisualization()
    {
        Transform t = this.transform.Find("targetVisualization");
        t.SetParent(_entity.transform);
        t.position = _entity.transform.position;

        _targetVisualization = t.GetComponent<LineRenderer>();
        _targetVisualization.material = _lineMaterial;
        _targetVisualization.loop = false;
        _targetVisualization.useWorldSpace = true;
        _targetVisualization.positionCount = 2;
        _targetVisualization.startWidth = .1f;
        _targetVisualization.endWidth = .1f;
    }

    void OnMouseEnter()
    {

    }
    void OnLeftClick()
    {
        if(_entity.teamID == 0)
            CommandMapper.SelectShip(_entity, Input.GetKey(KeyCode.LeftShift), Input.GetKey(KeyCode.LeftControl));
        else
            CommandMapper.AddMove(_entity.transform.position, !Input.GetKey(KeyCode.LeftShift));
    }
    void OnScrollClick()
    {
        if (Input.GetKey(KeyCode.LeftAlt))
            CommandMapper.SetAlignmentPlane(_entity.transform.position.y);
        else
            CameraManager.getInstance.JumpTo(_entity.transform.position, true);
    }
    void OnRightClick()
    {
        if (_entity.isDiscovered)
            CommandMapper.UpdateTarget(_entity);
    }
    void OnMouseExit()
    {

    }
}