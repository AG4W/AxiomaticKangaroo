using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;

public class ToolbarManager : MonoBehaviour
{
    static ToolbarManager _instance;
    public static ToolbarManager getInstance { get { return _instance; } }

    [SerializeField]Transform _toolbar;
    [SerializeField]GameObject _toolbarButtonItem;
    [SerializeField]GameObject _toolbarSeparator;
    [SerializeField]GameObject _toolbarResourceItem;

    List<GameObject> _resourceItems = new List<GameObject>();

    void Awake()
    {
        _instance = this;
    }
    public void Initialize()
    {
        InitializeButtons();
        InitializeResources();

        PlayerData.fleet.OnVitalChanged += UpdateResourceItem;
    }

    void InitializeButtons()
    {
        //create fleet management button
        GameObject fb = Instantiate(_toolbarButtonItem, _toolbar);

        fb.GetComponent<GenericTooltipHandler>().Initialize(
            () => TooltipManager.getInstance.OpenTooltip("Fleet.", Input.mousePosition),
            delegate
            {
                if (ManagementUIManager.getInstance.isOpen)
                    ManagementUIManager.getInstance.CloseManagementWindow();
                else
                    ManagementUIManager.getInstance.OpenManagementWindow();
            },
            null,
            null,
            () => TooltipManager.getInstance.CloseTooltip());

        //create fleet storage button
        GameObject fsb = Instantiate(_toolbarButtonItem, _toolbar);

        fsb.GetComponent<GenericTooltipHandler>().Initialize(
            () => TooltipManager.getInstance.OpenTooltip("Inventory.", Input.mousePosition),
            null,
            null,
            null,
            () => TooltipManager.getInstance.CloseTooltip());

        //create crew management button
        GameObject cmb = Instantiate(_toolbarButtonItem, _toolbar);

        cmb.GetComponent<GenericTooltipHandler>().Initialize(
            () => TooltipManager.getInstance.OpenTooltip("Crew.", Input.mousePosition),
            null,
            null,
            null,
            () => TooltipManager.getInstance.CloseTooltip());

        CreateSeparator();
    }
    void InitializeResources()
    {
        //fuel
        CreateResourceItem(FleetVitalType.ProcessedFuel,
            FleetVital.Format(FleetVitalType.ProcessedFuel) + " is crucial for your fleets ability to travel to other systems.\n" +
            "It is created by processing " + FleetVital.Format(FleetVitalType.NebulaGas) + " using a <color=yellow>Refinement Module</color> on a specialized ship\n" +
            "or at a <color=yellow>Orbital Refinery</color>.");

        //ammo
        CreateResourceItem(FleetVitalType.Ammunition,
            FleetVital.Format(FleetVitalType.Ammunition) + " is crucial for your fleets operational capabilities.\n" +
            "Low supplies of " + FleetVital.Format(FleetVitalType.Ammunition) + " might cause negative penalties.\n" +
            FleetVital.Format(FleetVitalType.Ammunition) + " can be manufactured by processing " + FleetVital.Format(FleetVitalType.Veldspar) + " using a <color=yellow>Manufacturing Module</color> on a specialized ship\n" +
            "or at a <color=yellow>Orbital Manufacturing Plant</color>.");

        //civilian supplies
        CreateResourceItem(FleetVitalType.CivilianGoods,
            FleetVital.Format(FleetVitalType.CivilianGoods) + " represents everyday goods consumed by the varying crews of your ships.\n" +
            "Low supplies of " + FleetVital.Format(FleetVitalType.CivilianGoods) + " might cause ships to desert.\n" +
            FleetVital.Format(FleetVitalType.CivilianGoods) + " can be manufactured by processing " + FleetVital.Format(FleetVitalType.Tritanite) + " using a <color=yellow>Manufacturing Module</color> on a specialized ship\n" +
            "or at a <color=yellow>Orbital Manufacturing Plant</color>.");

        CreateSeparator();

        //nebula gas
        CreateResourceItem(FleetVitalType.NebulaGas,
            FleetVital.Format(FleetVitalType.NebulaGas) + " is a highly volatile gas used to create " + FleetVital.Format(FleetVitalType.ProcessedFuel) + ".");
        //tritanite
        CreateResourceItem(FleetVitalType.Tritanite,
            FleetVital.Format(FleetVitalType.Tritanite) + " is a highly versatile mineral.\n" +
            "It is commonly used to manufacture " + FleetVital.Format(FleetVitalType.CivilianGoods) + ".");
        //veldspar
        CreateResourceItem(FleetVitalType.Veldspar,
            FleetVital.Format(FleetVitalType.Veldspar) + " is the most common mineral in the galaxy.\n" +
            "It is commonly used to manufacture " + FleetVital.Format(FleetVitalType.Ammunition) + ".");

        CreateSeparator();

        CreateResourceItem(FleetVitalType.Range,
            FleetVital.Format(FleetVitalType.Range) + " determines how far your fleet can move.");
    }
    void CreateResourceItem(FleetVitalType type, string tooltip)
    {
        FleetVital v = PlayerData.fleet.GetVital(type);
        GameObject g = Instantiate(_toolbarResourceItem, _toolbar);

        g.GetComponent<GenericTooltipHandler>().Initialize(
            () => TooltipManager.getInstance.OpenTooltip(tooltip + "\n\n", Input.mousePosition),
            null,
            null,
            null,
            () => TooltipManager.getInstance.CloseTooltip());

        g.transform.Find("icon").GetComponent<Image>().sprite = ModelDB.GetResourceIcon(type);
        g.transform.Find("icon").GetComponent<Image>().color = FleetVital.Color(type);

        _resourceItems.Add(g);

        UpdateResourceItem(v);
    }
    void CreateSeparator()
    {
        Instantiate(_toolbarSeparator, _toolbar);
    }

    //void UpdateAllResourceItems(Fleet fleet)
    //{
    //    for (int i = 0; i < System.Enum.GetNames(typeof(FleetVitalType)).Length; i++)
    //        UpdateResourceItem(fleet.GetVital((FleetVitalType)i));
    //}
    void UpdateResourceItem(Vital v)
    {
        FleetVital vital = v as FleetVital;

        Text t = _resourceItems[(int)vital.type].transform.Find("current").GetComponent<Text>();
        Text c = _resourceItems[(int)vital.type].transform.Find("changePerTurn").GetComponent<Text>();

        t.text = vital.ToString();
        t.color = Color.Lerp(Color.red, Color.green, vital.inPercent);

        c.text = "(" + vital.changePerTurn.ToString("+0.##;-0.##;0") + ")";
        c.color = vital.changePerTurn == 0 ? Color.yellow : vital.changePerTurn > 0 ? Color.green : Color.red;
    }
}
