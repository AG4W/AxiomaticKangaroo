using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField]Transform _shipStats;
    [SerializeField]Transform _fleetStats;
    [SerializeField]Transform _modifiers;

    [SerializeField]GameObject _listItem;
    [SerializeField]GameObject _separator;
    [SerializeField]GameObject _header;

    [SerializeField]GameObject _officer;
    [SerializeField]GameObject _disband;
    [SerializeField]GameObject _done;

    [Header("Inventory Window")]
    [SerializeField]GameObject _inventoryWindow;
    [SerializeField]Transform _inventoryList;

    public bool isOpen { get { return _managementWindow.activeSelf; } }

    void Awake()
    {
        _instance = this;

        _managementWindow.SetActive(false);
        _inventoryWindow.SetActive(false);

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
        CreateItems(s);

        _name.text = "<color=yellow>" + s.name + "</color>, " + s.GetClass();

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
    void CreateItems(Ship s)
    {
        //create header
        CreateHeader(_shipStats, "[Weapons]:", "");
        //create separator
        //CreateSeparator(_shipStats);

        //weapons
        for (int i = 0; i < s.weapons.Length; i++)
        {
            int a = i;
            Weapon w = s.weapons[i];

            GameObject g = Instantiate(_listItem, _shipStats);

            g.transform.Find("title").GetComponent<Text>().text = w == null ? "[Empty Slot]" : w.name;
            g.transform.Find("body").GetComponent<Text>().text = w == null ? "" : "<color=red>" + w.minDamage + "</color> - <color=green>" + w.maxDamage + "</color>";

            if(w != null)
                g.transform.Find("icon").GetComponent<Image>().sprite = w.icon;

            g.GetComponent<GenericTooltipHandler>().Initialize(
                () => TooltipManager.getInstance.OpenTooltip(w == null ? "Left-click to equip a weapon in this slot." : w.name + "\n" + w.GetSummary(), Input.mousePosition),
                () => OpenInventoryWindow(s, a, true, g.transform.position),
                null,
                delegate 
                {
                    if (w != null)
                    {
                        s.weapons[a] = null;
                        UpdateShipListAndOpenSelectedWindow(s);
                        PlayerData.inventory.Add(w);
                    }
                },
                () => TooltipManager.getInstance.CloseTooltip());
        }

        //create separator
        CreateSeparator(_shipStats);
        //create header
        CreateHeader(_shipStats, "[Utilities]:", "");
        //create separator
        //CreateSeparator(_shipStats);

        //utilities
        for (int i = 0; i < s.utilities.Length; i++)
        {
            int a = i;
            Utility u = s.utilities[i];

            GameObject g = Instantiate(_listItem, _shipStats);

            g.transform.Find("title").GetComponent<Text>().text = u == null ? "[Empty Slot]" : s.utilities[i].name;
            g.transform.Find("body").GetComponent<Text>().text = "";

            if(u != null)
                g.transform.Find("icon").GetComponent<Image>().sprite = u.icon;

            g.GetComponent<GenericTooltipHandler>().Initialize(
                () => TooltipManager.getInstance.OpenTooltip(u == null ? "Left-click to equip a utility in this slot." : s.utilities[a].name + "\n" + s.utilities[a].GetSummary(), Input.mousePosition),
                () => OpenInventoryWindow(s, a, false, g.transform.position),
                null,
                delegate
                {
                    if (u != null)
                    {
                        s.utilities[a] = null;
                        UpdateShipListAndOpenSelectedWindow(s);
                        PlayerData.inventory.Add(u);
                    }
                },
                () => TooltipManager.getInstance.CloseTooltip());
        }

        //create separator
        CreateSeparator(_shipStats);

        //fleet vitals
        CreateGenericItem(_fleetStats, "Fuel Storage", s.GetVital(VitalType.FuelStorage).ToString(), "");
        CreateGenericItem(_fleetStats, "Goods Storage", s.GetVital(VitalType.GoodsStorage).ToString(), "");
        CreateGenericItem(_fleetStats, "Ore Storage", s.GetVital(VitalType.OreStorage).ToString(), "");
        CreateGenericItem(_fleetStats, "Gas Storage", s.GetVital(VitalType.GasStorage).ToString(), "");
    }
    void CreateGenericItem(Transform root, string title, string body, string tooltip)
    {
        GameObject g = Instantiate(_listItem, root);

        g.transform.Find("title").GetComponent<Text>().text = title;
        g.transform.Find("body").GetComponent<Text>().text = body;
        g.GetComponent<GenericTooltipHandler>().Initialize(
            () => TooltipManager.getInstance.OpenTooltip(tooltip, Input.mousePosition),
            null,
            null,
            null,
            () => TooltipManager.getInstance.CloseTooltip());
    }
    void CreateSeparator(Transform root)
    {
        Instantiate(_separator, root);
    }
    void CreateHeader(Transform root, string text, string tooltip)
    {
        GameObject g = Instantiate(_header, root);

        g.transform.Find("title").GetComponent<Text>().text = text;
        g.GetComponent<GenericTooltipHandler>().Initialize(
            () => TooltipManager.getInstance.OpenTooltip(tooltip, Input.mousePosition),
            null,
            null,
            null,
            () => TooltipManager.getInstance.CloseTooltip());
    }

    void OpenInventoryWindow(Ship ship, int index, bool isWeapon, Vector3 position)
    {
        ClearInventoryWindow();
        List<ShipComponent> components = new List<ShipComponent>();

        if(isWeapon)
            components = PlayerData.inventory
                .Where(i => i is Weapon)
                .OrderBy(i => i.name)
                .ToList();
        else
            components = PlayerData.inventory
                .Where(i => i is Utility)
                .OrderBy(i => i.name)
                .ToList();

        for (int i = 0; i < components.Count; i++)
        {
            ShipComponent sc = components[i];

            GameObject g = Instantiate(_listItem, _inventoryList);

            g.transform.Find("title").GetComponent<Text>().text = sc.name;
            g.transform.Find("body").GetComponent<Text>().text = isWeapon ? "<color=red>" + (sc as Weapon).minDamage + "</color> - <color=green>" + (sc as Weapon).maxDamage + "</color>" : "";
            g.transform.Find("icon").GetComponent<Image>().sprite = sc.icon;

            g.GetComponent<GenericTooltipHandler>().Initialize(
                () => TooltipManager.getInstance.OpenTooltip(sc.name + "\n" + sc.GetSummary(), Input.mousePosition),
                delegate
                {
                    ShipComponent old;

                    if (isWeapon)
                    {
                        old = ship.weapons[index];
                        ship.weapons[index] = sc as Weapon;
                    }
                    else
                    {
                        old = ship.utilities[index];
                        ship.utilities[index] = sc as Utility;
                    }

                    UpdateShipListAndOpenSelectedWindow(ship);
                    CloseInventoryWindow();
                    PlayerData.inventory.Remove(sc);

                    if (old != null)
                        PlayerData.inventory.Add(sc);
                },
                null,
                null,
                () => TooltipManager.getInstance.CloseTooltip());
        }

        _inventoryWindow.transform.position = position;
        _inventoryWindow.GetComponent<GenericTooltipHandler>().Initialize(
            null,
            null,
            null,
            null,
            () => CloseInventoryWindow());
        _inventoryWindow.SetActive(true);
    }
    void CloseInventoryWindow()
    {
        _inventoryWindow.SetActive(false);
    }

    void Clear()
    {
        for (int i = 0; i < _shipList.childCount; i++)
            Destroy(_shipList.GetChild(i).gameObject);
    }
    void ClearSelectedWindow()
    {
        for (int i = 0; i < _modifiers.childCount; i++)
            Destroy(_modifiers.GetChild(i).gameObject);
        for (int i = 0; i < _shipStats.childCount; i++)
            Destroy(_shipStats.GetChild(i).gameObject);
        for (int i = 0; i < _fleetStats.childCount; i++)
            Destroy(_fleetStats.GetChild(i).gameObject);
    }
    void ClearInventoryWindow()
    {
        for (int i = 0; i < _inventoryList.childCount; i++)
            Destroy(_inventoryList.GetChild(i).gameObject);
    }
}
