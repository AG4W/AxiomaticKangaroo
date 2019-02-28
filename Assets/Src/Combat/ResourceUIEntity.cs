using UnityEngine;
using UnityEngine.UI;

public class ResourceUIEntity : MonoBehaviour
{
    [SerializeField]GameObject _uiPrefab;
    [SerializeField]GameObject _alignmentObject;
    GameObject _uiItem;

    LineRenderer _alignmentVisualization;

    ResourceEntity _entity;
    Camera _camera;

    bool _isDiscovered = false;

    public void Initialize(ResourceEntity entity)
    {
        entity.OnDiscovered += OnDiscovered;
        entity.OnScanned += OnScanned;
        entity.OnCountChanged += OnCountChanged;
        entity.OnDestroyed += OnDestroyed;

        _entity = entity;
        _camera = Camera.main;

        OnInitialize();
    }
    void OnInitialize()
    {
        _uiItem = Instantiate(_uiPrefab, WorldUIManager.getInstance.canvas);
        _uiItem.transform.Find("text").GetComponent<Text>().text = "[Unknown Signal]";

        _uiItem.GetComponent<GenericTooltipHandler>().Initialize(
            null,   //should bring up tooltip
            delegate 
            {
                CommandMapper.AddMove(_entity.transform.position, !Input.GetKey(KeyCode.LeftShift));

                if (_isDiscovered)
                    CommandMapper.SetResourceTarget(_entity);
            },
            delegate
            {
                if (Input.GetKey(KeyCode.LeftAlt))
                    CommandMapper.SetAlignmentPlane(_entity.transform.position.y);
                else
                    CameraManager.getInstance.JumpTo(_entity.transform.position, true);
            },
            null,
            null);  //should kill tooltip

        InitializeAlignmentVisualization();
    }
    void InitializeAlignmentVisualization()
    {
        Transform t = this.transform.Find("alignmentVisualization");
        t.SetParent(_entity.transform);
        t.position = _entity.transform.position;

        _alignmentVisualization = t.GetComponent<LineRenderer>();
        _alignmentVisualization.loop = false;
        _alignmentVisualization.useWorldSpace = true;
        _alignmentVisualization.positionCount = 2;
        _alignmentVisualization.startWidth = .1f;
        _alignmentVisualization.endWidth = .1f;
        _alignmentVisualization.SetPosition(0, _entity.transform.position);
    }

    void Update()
    {
        Vector3 p = _camera.WorldToViewportPoint(_entity.transform.position);
        Vector3 op = _camera.WorldToScreenPoint(_entity.transform.position);
        op.z = 0;

        _uiItem.SetActive(p.x > 0 && p.x < 1 && p.y > 0 && p.y < 1 && p.z > 0);
        _uiItem.transform.position = op;

        _alignmentVisualization.transform.eulerAngles = Vector3.zero;
        _alignmentVisualization.SetPosition(1, new Vector3(_entity.transform.position.x, AlignmentPlane.y, _entity.transform.position.z));
    }

    void OnDiscovered()
    {
        _isDiscovered = true;
        _uiItem.transform.Find("text").GetComponent<Text>().text =
            "[" + _entity.name + "]:\n" +
            "??? units of " + FleetVital.Format(_entity.type);

        _uiItem.transform.Find("icon").GetComponent<Image>().sprite = ModelDB.GetResourceIcon(_entity.type);
        _uiItem.transform.Find("icon").GetComponent<Image>().color = FleetVital.Color(_entity.type);

        LogManager.getInstance.AddEntry("??? units of " + FleetVital.Format(_entity.type) + " has been discovered.");
    }
    void OnScanned()
    {
        _uiItem.transform.Find("text").GetComponent<Text>().text =
            "[" + _entity.name + "]:\n" +
            _entity.count.ToString("0.##") + " units of " + FleetVital.Format(_entity.type);

        LogManager.getInstance.AddEntry(_entity.count.ToString("0.##") + " units of " + FleetVital.Format(_entity.type) + " has been scanned.");
    }
    void OnCountChanged(float count)
    {
        _uiItem.transform.Find("text").GetComponent<Text>().text =
            "[" + _entity.name + "]:\n" +
            _entity.count.ToString("0.##") + " units of " + FleetVital.Format(_entity.type);
    }
    void OnDestroyed()
    {
        _entity.OnDiscovered -= OnDiscovered;
        _entity.OnScanned -= OnScanned;
        _entity.OnCountChanged -= OnCountChanged;
        _entity.OnDestroyed -= OnDestroyed;

        if(_uiItem != null)
            Destroy(_uiItem);
    }
}