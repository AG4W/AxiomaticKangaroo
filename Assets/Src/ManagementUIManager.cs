using UnityEngine;
using UnityEngine.UI;

using System.Linq;

public class ManagementUIManager : MonoBehaviour
{
    static ManagementUIManager _instance;
    public static ManagementUIManager getInstance { get { return _instance; } }

    [Header("Management List")]
    [SerializeField]GameObject _managementWindow;

    [SerializeField]Transform _shipList;
    [SerializeField]GameObject _shipListItem;

    [Header("Selected Ship")]
    [SerializeField]GameObject _selectedShipWindow;

    [SerializeField]Text _name;

    [SerializeField]Transform _weapons;
    [SerializeField]Transform _utilities;
    [SerializeField]GameObject _componentItem;

    [SerializeField]Transform _shipStats;
    [SerializeField]Transform _fleetStats;
    [SerializeField]Transform _modifiers;
    [SerializeField]GameObject _statListItem;

    [SerializeField]GameObject _officer;
    [SerializeField]GameObject _disband;
    [SerializeField]GameObject _done;

    public bool isOpen { get { return _managementWindow.activeSelf; } }

    void Awake()
    {
        _instance = this;

        _managementWindow.SetActive(false);
        _selectedShipWindow.SetActive(false);
    }

    public void OpenManagementWindow()
    {
        UpdateShipList();

        _managementWindow.SetActive(true);
    }
    public void CloseManagementWindow()
    {
        _managementWindow.SetActive(false);
    }

    void UpdateShipList()
    {
        Clear();

        for (int i = 0; i < PlayerData.fleet.ships.Count; i++)
            CreateShipItem(PlayerData.fleet.ships[i]);
    }
    void UpdateShipListAndOpenSelectedWindow(Ship s)
    {
        UpdateShipList();
        UpdateSelectedWindow(s);
    }
    void CreateShipItem(Ship s)
    {
        GameObject g = Instantiate(_shipListItem, _shipList);

        g.transform.Find("name").GetComponent<Text>().text = "<color=yellow>" + s.name + "</color>";
        g.transform.Find("class").GetComponent<Text>().text = s.GetClass();
        g.transform.Find("officer").GetComponent<Image>().sprite = s.officer == null ? ModelDB.defaultPortrait : s.officer.portrait;

        g.transform.Find("edit").GetComponent<GenericTooltipHandler>().Initialize(
            () => TooltipManager.getInstance.OpenTooltip("Left-click to open details.", Input.mousePosition),
            () => UpdateSelectedWindow(s),
            null,
            null,
            () => TooltipManager.getInstance.CloseTooltip());
        g.transform.Find("officer").GetComponent<GenericTooltipHandler>().Initialize(
            () => TooltipManager.getInstance.OpenTooltip(s.officer == null ? "No officer assigned." : s.officer.name, Input.mousePosition),
            null,
            null,
            null,
            () => TooltipManager.getInstance.CloseTooltip());
    }

    void UpdateSelectedWindow(Ship s)
    {
        ClearSelectedWindow();
        CreateStatItems(s);

        _name.text = "<color=yellow>" + s.name + "</color>, " + s.GetClass();

        for (int i = 0; i < s.weapons.Length; i++)
            CreateComponentItem(_weapons, s.weapons[i]);
        for (int i = 0; i < s.utilities.Length; i++)
            CreateComponentItem(_utilities, s.utilities[i]);

        _officer.transform.Find("name").GetComponent<Text>().text = s.officer == null ? "No officer assigned." : s.officer.rank + " " + s.officer.name;
        _officer.transform.Find("misc").GetComponent<Text>().text = s.officer == null ? "Left-click to assign." : "Left-click to replace.";
        _officer.transform.Find("portrait").GetComponent<Image>().sprite = s.officer == null ? ModelDB.defaultPortrait : s.officer.portrait;
        _officer.GetComponent<GenericTooltipHandler>().Initialize(
            () => TooltipManager.getInstance.OpenTooltip(s.officer == null ? "Assign." : "Replace.", Input.mousePosition),
            null,
            null,
            null,
            () => TooltipManager.getInstance.CloseTooltip());

        _done.GetComponent<GenericTooltipHandler>().Initialize(
            () => TooltipManager.getInstance.OpenTooltip("Apply changes.", Input.mousePosition),
            delegate 
            {
                _selectedShipWindow.SetActive(false);

                UpdateShipList();
            },
            null,
            null,
            () => TooltipManager.getInstance.CloseTooltip());
        _disband.GetComponent<GenericTooltipHandler>().Initialize(
            () => TooltipManager.getInstance.OpenTooltip("Remove this ship from the fleet.\n\n<color=red>WARNING: This will remove any equipment and/or officers assigned to this ship.\n\nThis action is not reversible.</color>", Input.mousePosition),
            delegate 
            {
                _selectedShipWindow.SetActive(false);

                PlayerData.fleet.RemoveShip(s);
                UpdateShipList();
            },
            null,
            null,
            () => TooltipManager.getInstance.CloseTooltip());

        _selectedShipWindow.SetActive(true);
    }
    void CreateStatItems(Ship s)
    {
        //weapons
        for (int i = 0; i < s.weapons.Length; i++)
        {
            int a = i;

            GameObject g = Instantiate(_statListItem, _shipStats);

            g.transform.Find("title").GetComponent<Text>().text = s.weapons[i].name;
            g.transform.Find("body").GetComponent<Text>().text = "<color=red>" + s.weapons[i].minDamage + "</color> - <color=green>" + s.weapons[i].maxDamage + "</color>";
            g.GetComponent<GenericTooltipHandler>().Initialize(
                () => TooltipManager.getInstance.OpenTooltip(s.weapons[a].name + "\n" + s.weapons[a].GetSummary(), Input.mousePosition),
                null,
                null,
                null,
                () => TooltipManager.getInstance.CloseTooltip());
        }

        //fleet vitals
        CreateGenericItem(_fleetStats, "Fuel Storage", s.GetVital(VitalType.FuelStorage).ToString(), "");
        CreateGenericItem(_fleetStats, "Goods Storage", s.GetVital(VitalType.GoodsStorage).ToString(), "");
        CreateGenericItem(_fleetStats, "Ore Storage", s.GetVital(VitalType.OreStorage).ToString(), "");
        CreateGenericItem(_fleetStats, "Gas Storage", s.GetVital(VitalType.GasStorage).ToString(), "");
    }
    void CreateComponentItem(Transform root, ShipComponent component)
    {
        GameObject g = Instantiate(_componentItem, root);

        g.transform.Find("icon").GetComponent<Image>().sprite = component.icon;
        g.transform.Find("icon").GetComponent<Image>().color = component.ColorByRarity();
        g.transform.Find("borders").GetComponent<Image>().color = component.ColorByRarity();

        g.transform.GetComponent<GenericTooltipHandler>().Initialize(
            () => TooltipManager.getInstance.OpenTooltip(component.name + "\n" + component.GetSummary(), Input.mousePosition),
            null,
            null,
            null,
            () => TooltipManager.getInstance.CloseTooltip());
    }
    void CreateGenericItem(Transform root, string title, string body, string tooltip)
    {
        GameObject g = Instantiate(_statListItem, root);

        g.transform.Find("title").GetComponent<Text>().text = title;
        g.transform.Find("body").GetComponent<Text>().text = body;
        g.GetComponent<GenericTooltipHandler>().Initialize(
            () => TooltipManager.getInstance.OpenTooltip(tooltip, Input.mousePosition),
            null,
            null,
            null,
            () => TooltipManager.getInstance.CloseTooltip());
    }

    void Clear()
    {
        for (int i = 0; i < _shipList.childCount; i++)
            Destroy(_shipList.GetChild(i).gameObject);
    }
    void ClearSelectedWindow()
    {
        for (int i = 0; i < _weapons.childCount; i++)
            Destroy(_weapons.GetChild(i).gameObject);
        for (int i = 0; i < _utilities.childCount; i++)
            Destroy(_utilities.GetChild(i).gameObject);

        for (int i = 0; i < _modifiers.childCount; i++)
            Destroy(_modifiers.GetChild(i).gameObject);
        for (int i = 0; i < _shipStats.childCount; i++)
            Destroy(_shipStats.GetChild(i).gameObject);
        for (int i = 0; i < _fleetStats.childCount; i++)
            Destroy(_fleetStats.GetChild(i).gameObject);
    }
}
