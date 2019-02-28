using UnityEngine;

public class ResourceEntity : MonoBehaviour
{
    [SerializeField]string _name;

    [SerializeField]float _minCount = 0f;
    [SerializeField]float _maxCount = 5f;

    float _count;

    [SerializeField]FleetVitalType _type;

    bool _isDiscovered = false;
    bool _isScanned = false;

    public new string name { get { return _name; } }

    public float count { get { return _count; } }
    public FleetVitalType type { get { return _type; } }

    public bool isDiscovered { get { return _isDiscovered; } }
    public bool isScanned { get { return _isScanned; } }

    void Start()
    {
        _count = Random.Range(_minCount, _maxCount);

        this.GetComponent<ResourceUIEntity>().Initialize(this);
    }

    public void Discover()
    {
        if (_isDiscovered)
            return;

        _isDiscovered = true;
        OnDiscovered?.Invoke();
    }
    public void Scan()
    {
        if (_isScanned)
            return;

        _isScanned = true;
        OnScanned?.Invoke();
    }

    public float Extract(float requestedAmount)
    {
        float r = 0f;

        if(_count < requestedAmount)
        {
            r = _count;
            _count = 0f;
        }
        else
        {
            r = requestedAmount;
            _count -= requestedAmount;
        }

        OnResourceChanged();
        return r;
    }
    public GameObject GetVisualTarget()
    {
        Transform t = this.transform.Find("visualTargets");
        return t.GetChild(Random.Range(0, t.childCount)).gameObject;
    }

    void OnResourceChanged()
    {
        OnCountChanged?.Invoke(_count);

        if (_count <= 0f)
            OnEmpty();
    }
    void OnEmpty()
    {
        OnDestroyed?.Invoke();
        Destroy(this.gameObject);
    }

    public delegate void DiscoveredEvent();
    public DiscoveredEvent OnDiscovered;
    public DiscoveredEvent OnScanned;
    public delegate void ResourceChangedEvent(float count);
    public ResourceChangedEvent OnCountChanged;
    public delegate void DestroyedEvent();
    public DestroyedEvent OnDestroyed;
}