﻿using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;
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

    [Header("Crew Window")]
    [SerializeField]GameObject _officerItem;
    [SerializeField]GameObject _noOfficerAvailableItem;
    [SerializeField]GameObject _crewWindow;
    [SerializeField]Transform _crewList;

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
            () => OpenCrewWindow(s, _officer.transform.position),
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

            g.transform.Find("title").GetComponent<Text>().text = w == null ? "[Empty Slot]" : "<color=#" + ColorUtility.ToHtmlStringRGB(w.GetColor()) + ">" + w.name + "</color>";
            g.transform.Find("body").GetComponent<Text>().text = w == null ? "" : "<color=red>" + w.minDamage.ToString("0.##") + "</color> - <color=green>" + w.maxDamage.ToString("0.##") + "</color>";

            if(w != null)
                g.transform.Find("icon").GetComponent<Image>().sprite = w.icon;

            g.GetComponent<GenericTooltipHandler>().Initialize(
                () => TooltipManager.getInstance.OpenTooltip(w == null ? "Left-click to equip a weapon in this slot." : w.GetSummary(), Input.mousePosition),
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

            g.transform.Find("title").GetComponent<Text>().text = u == null ? "[Empty Slot]" : "<color=#" + ColorUtility.ToHtmlStringRGB(u.GetColor()) + ">" + u.name + "</color>";
            g.transform.Find("body").GetComponent<Text>().text = "";

            if(u != null)
                g.transform.Find("icon").GetComponent<Image>().sprite = u.icon;

            g.GetComponent<GenericTooltipHandler>().Initialize(
                () => TooltipManager.getInstance.OpenTooltip(u == null ? "Left-click to equip a utility in this slot." : u.GetSummary(), Input.mousePosition),
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

    void OpenInventoryWindow(Ship s, int index, bool isWeapon, Vector3 position)
    {
        ClearInventoryWindow();
        List<ShipComponent> components = new List<ShipComponent>();

        if (isWeapon)
            components = PlayerData.inventory.Where(i => i is Weapon && i.minimumSize <= s.size).ToList();
        else
            components = PlayerData.inventory.Where(i => i is Utility && i.minimumSize <= s.size).ToList();

        components = components.OrderBy(i => i.name).OrderByDescending(i => i.rarity.rarity).ToList();

        for (int i = 0; i < components.Count; i++)
        {
            ShipComponent sc = components[i];

            GameObject g = Instantiate(_listItem, _inventoryList);

            g.transform.Find("title").GetComponent<Text>().text = "<color=#" + ColorUtility.ToHtmlStringRGB(sc.GetColor()) + ">" + sc.name + "</color>";
            g.transform.Find("body").GetComponent<Text>().text = isWeapon ? "<color=red>" + (sc as Weapon).minDamage.ToString("0.##") + "</color> - <color=green>" + (sc as Weapon).maxDamage.ToString("0.##") + "</color>" : "";
            g.transform.Find("icon").GetComponent<Image>().sprite = sc.icon;

            g.GetComponent<GenericTooltipHandler>().Initialize(
                () => TooltipManager.getInstance.OpenTooltip(sc.GetSummary(), Input.mousePosition),
                delegate
                {
                    ShipComponent old;

                    if (isWeapon)
                    {
                        old = s.weapons[index];
                        s.weapons[index] = sc as Weapon;
                    }
                    else
                    {
                        old = s.utilities[index];
                        s.utilities[index] = sc as Utility;
                    }

                    UpdateShipListAndOpenSelectedWindow(s);
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

    void OpenCrewWindow(Ship s, Vector3 position)
    {
        ClearCrewWindow();

        if(PlayerData.officers.Count == 0)
            Instantiate(_noOfficerAvailableItem, _crewList);
        else
        {
            for (int i = 0; i < PlayerData.officers.Count; i++)
            {
                Officer o = PlayerData.officers[i];
                GameObject g = Instantiate(_officerItem, _crewList);

                g.transform.Find("portrait").GetComponent<Image>().sprite = o.portrait;
                g.transform.Find("text").GetComponent<Text>().text = o.ToString();
                g.GetComponent<GenericTooltipHandler>().Initialize(
                    null,
                    delegate
                    {
                        //unassign old officer
                        if (s.officer != null)
                            s.officer.Assign(null);

                        //unassign new officer from old assignment
                        if(o.assignment != null)
                            o.assignment.AssignOfficer(null);

                        s.AssignOfficer(o);
                        UpdateShipListAndOpenSelectedWindow(s);
                        CloseCrewWindow();
                    },
                    null,
                    null,
                    null);
            }
        }

        _crewWindow.GetComponent<GenericTooltipHandler>().Initialize(null, null, null, null, () => CloseCrewWindow());
        _crewWindow.transform.position = position;
        _crewWindow.SetActive(true);
    }
    void CloseCrewWindow()
    {
        _crewWindow.SetActive(false);
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
    void ClearCrewWindow()
    {
        for (int i = 0; i < _crewList.childCount; i++)
            Destroy(_crewList.GetChild(i).gameObject);
    }
}
