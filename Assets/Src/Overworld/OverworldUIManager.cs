using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;

public class OverworldUIManager : MonoBehaviour
{
    static OverworldUIManager _instance;
    public static OverworldUIManager getInstance { get { return _instance; } }

    [SerializeField]Transform _canvas;

    [SerializeField]GameObject _currentSectorDetails;
    [SerializeField]Text _sectorDetails;
    [SerializeField]Text _sectorResources;

    [Header("World")]
    [SerializeField]GameObject _poiItem;
    List<GameObject> _poiItems = new List<GameObject>();

    Camera _camera;

    void Awake()
    {
        _instance = this;
    }
    void Update()
    {
        UpdateItems();
    }
    
    public void Initialize()
    {
        _camera = Camera.main;
        
        RuntimeData.system.OnPointOfInterestAdded += CreatePointOfInterestItem;
        InitializePointsOfInterest();
    }

    void InitializePointsOfInterest()
    {
        for (int i = 0; i < RuntimeData.system.pointsOfInterest.Count; i++)
            CreatePointOfInterestItem(RuntimeData.system.pointsOfInterest[i]);
    }
    void CreatePointOfInterestItem(PointOfInterest poi)
    {
        GameObject item = Instantiate(_poiItem, _canvas);

        Text t = item.transform.Find("headerParent").GetComponentInChildren<Text>();
        t.text = "[" + poi.GetType().ToString() + "]: " + poi.name + " ";

        item.transform.Find("icon").GetComponent<Image>().sprite = ModelDB.GetIcon(poi.type);
        item.GetComponent<GenericTooltipHandler>()
            .Initialize(
                () => poi.OnMouseEnter(),
                () => poi.OnLeftClick(),
                () => poi.OnScrollClick(),
                () => poi.OnRightClick(),
                () => poi.OnMouseExit());

        _poiItems.Add(item);
    }

    void UpdateItems()
    {
        for (int i = 0; i < RuntimeData.system.pointsOfInterest.Count; i++)
        {
            Vector3 p = _camera.WorldToViewportPoint(RuntimeData.system.pointsOfInterest[i].location);

            _poiItems[i].SetActive(p.x > 0 && p.x < 1 && p.y > 0 && p.y < 1 && p.z > 0);

            if (!_poiItems[i].activeSelf)
                continue;

            _poiItems[i].transform.position = _camera.WorldToScreenPoint(RuntimeData.system.pointsOfInterest[i].location);
        }
    }

    public void UpdateCurrentSector(Cell sector)
    {
        _currentSectorDetails.SetActive(sector != null);

        if (_currentSectorDetails.activeSelf)
        {
            _sectorDetails.text = sector.GetDetails();
            _sectorResources.text = sector.GetResources();
        }
    }
}
