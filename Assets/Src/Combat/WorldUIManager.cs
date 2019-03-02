using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

public class WorldUIManager : MonoBehaviour
{
    static WorldUIManager _instance;
    public static WorldUIManager getInstance { get { return _instance; } }

    [Header("World UI")]
    [SerializeField]GameObject _contactItem;

    [Header("User UI")]
    [SerializeField]GameObject _selectedShips;
    [SerializeField]GameObject _abilities;

    [SerializeField]GameObject _weapons;
    [SerializeField]GameObject _utilities;
    [SerializeField]GameObject _abilityItem;

    [SerializeField]GameObject _shipSpeedSettings;

    [SerializeField]Transform _groups;
    [SerializeField]GameObject _groupItem;
    [SerializeField]GameObject _groupShipItem;

    [SerializeField]GameObject _simulationSpeed;
    [SerializeField]Color _simulationSpeedCurrentColor = new Color(1f, .5f, 0f);
    [SerializeField]Color _simulationSpeedDefaultColor = Color.white;
    [SerializeField]GameObject _exitButton;

    GameObject[] _groupItems = new GameObject[10];
    List<ContactItem> _contactItems = new List<ContactItem>();

    Texture2D _whiteTexture;
    Vector3 _selectionBoxOrigin;
    bool _drawSelectionBox = false;

    Camera _camera;
    [SerializeField]Transform _canvas;

    public Transform canvas { get { return _canvas; } }

    void Awake()
    {
        _instance = this;
    }
    void Update()
    {
        UpdateContactItems();
    }
    void OnGUI()
    {
        if (_drawSelectionBox)
        {
            Rect r = GetScreenRect(_selectionBoxOrigin, Input.mousePosition);
            DrawScreenRect(r, new Color(.8f, .8f, .95f, .25f));
            DrawScreenRectBorder(r, 2, new Color(.8f, .8f, .95f));
        }
    }

    public void Initialize()
    {
        _whiteTexture = new Texture2D(1, 1);
        _whiteTexture.SetPixel(0, 0, Color.white);
        _whiteTexture.Apply();

        //_exitButton = GameObject.Find("leave");
        _selectedShips.SetActive(false);
        _groups.gameObject.SetActive(false);
        _camera = Camera.main;

        AlignmentPlane.Initialize();
        InitializeShipSpeedSelections();
        InitializeSimulationSpeedSelections();

        CommandMapper.OnGroupCreated += CreateGroupItem;
        CommandMapper.OnGroupRemoved += RemoveGroupItem;

        GameManager.OnSimulationSpeedChanged += OnSimulationSpeedChanged;
        GameManager.OnGameStatusUpdated += UpdateExitButton;
        GameManager.OnLeave += OnLeave;
    }

    void InitializeShipSpeedSelections()
    {
        for (int i = 0; i < System.Enum.GetValues(typeof(SpeedSetting)).Length; i++)
        {
            int a = i;

            _shipSpeedSettings.transform.GetChild(i)
                .GetComponent<Button>()
                .onClick
                .AddListener(() => CommandMapper.UpdateCurrentSpeed((SpeedSetting)a));
        }
    }
    void InitializeSimulationSpeedSelections()
    {
        for (int i = 0; i < _simulationSpeed.transform.childCount; i++)
        {
            int a = i;

            _simulationSpeed.transform.GetChild(i)
                .GetComponent<GenericTooltipHandler>().Initialize(
                () => TooltipManager.getInstance.OpenTooltip("Set simulation speed to: " + (SimulationSpeed)a + ".\n\n Use [<color=yellow>NUMPAD+</color>], [<color=yellow>NUMPAD-</color>] and [<color=yellow>SPACE</color>] to switch these settings rapidly.", Input.mousePosition),
                delegate
                {
                    CommandMapper.SetSimulationSpeed((SimulationSpeed)a);
                    TooltipManager.getInstance.CloseTooltip();
                },
                null,
                null,
                () => TooltipManager.getInstance.CloseTooltip());
        }
    }

    public void OnShipsSelected(List<ShipEntity> ships)
    {
        _selectedShips.SetActive(ships.Count > 0);

        if (!_selectedShips.activeSelf)
            return;

        UpdateSelectedShipInfo(ships);
        UpdateAbilities(ships);
    }
    void UpdateSelectedShipInfo(List<ShipEntity> ships)
    {
        Transform controls = _selectedShips.transform.Find("controls");

        controls.Find("name").GetComponent<Text>().text = ships[0].name;
        controls.Find("class").GetComponent<Text>().text = "n/a";

        controls.Find("hull").GetComponent<Image>().fillAmount = ships[0].GetVital(VitalType.HullPoints).inPercent;
        controls.Find("shield").GetComponent<Image>().fillAmount = ships[0].GetVital(VitalType.ShieldPoints).inPercent;
    }

    void UpdateAbilities(List<ShipEntity> ships)
    {
        Transform wList = _weapons.transform.Find("list");
        Transform uList = _utilities.transform.Find("list");
        //clear old
        for (int i = 0; i < wList.transform.childCount; i++)
            Destroy(wList.transform.GetChild(i).gameObject);
        for (int i = 0; i < uList.transform.childCount; i++)
            Destroy(uList.transform.GetChild(i).gameObject);

        _abilities.SetActive(ships.Count > 0);

        if (!_abilities.activeSelf)
            return;

        if(ships.Count == 1)
        {
            CreateAbilities(_weapons, ships[0].weapons, ships[0]);
            CreateAbilities(_utilities, ships[0].utilities, ships[0]);
        }
        else
        {
            //TODO: fix me
            //for every ship
                //for every component type
                    //create ability
                    //set activation to activate that for ALL ships with that utility
                    //...
                    //fuck me
        }
    }
    void CreateAbilities(GameObject root, ShipComponent[] components, ShipEntity ship)
    {
        root.SetActive(components.Length > 0);

        if (!root.activeSelf)
            return;

        LayoutElement le = root.transform.GetComponent<LayoutElement>();
        Transform list = root.transform.Find("list");

        for (int i = 0; i < components.Length; i++)
        {
            int a = i;

            GameObject g = Instantiate(_abilityItem, list);

            //set icon
            if(components[i].icon != null)
                g.transform.Find("icon").GetComponent<Image>().sprite = components[i].icon;

            //set tooltip actions
            GameObject autoActivate = g.transform.Find("autoActivate").gameObject;
            autoActivate.SetActive(components[i].autoActivate);

            //set activate action
            g.GetComponent<GenericTooltipHandler>()
                .Initialize(
                    delegate
                    {
                        components[a].OnTooltipEnter();
                        components[a].DrawVisualization();
                        TooltipManager.getInstance.OpenTooltip(components[a].name + "\n" + components[a].description, g.transform.position);
                    },
                    //add useaction for tooltip
                    delegate
                    {
                        if (components[a] is Weapon)
                        {
                            Weapon w = components[a] as Weapon;

                            if (!w.hasCooldown)
                                w.AttemptFire(null, ship);
                        }
                        else if (components[a] is Utility)
                            ((Utility)components[a]).AttemptActivate();
                    },
                    delegate
                    {
                        components[a].ToggleAutoActivate();
                        autoActivate.SetActive(components[a].autoActivate);
                    },
                    null,
                    delegate
                    {
                        components[a].OnTooltipExit();
                        Visualizer.getInstance.Hide();
                        TooltipManager.getInstance.CloseTooltip();
                    });

            StartCoroutine(UpdateCooldownUI(components[i], g.transform.Find("cooldown").GetComponent<Image>()));
        }

        float sizeX = 0f;

        //spacing left/right
        sizeX += 10f;
        sizeX += 5f * (components.Length - 1);
        sizeX += 40f * components.Length;

        le.preferredWidth = sizeX;
    }

    void UpdateExitButton(bool safeToLeave)
    {
        //Debug.Log(_exitButton);

        Color c1 = safeToLeave ? new Color(.25f, .5f, .25f) : new Color(.5f, .4f, .2f);
        Color c2 = safeToLeave ? new Color(.2f, .5f, .2f) : new Color(.4f, .3f, .2f);

        _exitButton.transform.Find("background").GetComponent<Image>().material.SetColor("_color1", c1);
        _exitButton.transform.Find("background").GetComponent<Image>().material.SetColor("_color2", c2);

        _exitButton.transform.Find("status").GetComponent<Text>().text = safeToLeave ? "[FTL JUMP PLOTTED]" : "[CANNOT PLOT FTL JUMP]";
        _exitButton.transform.Find("header").GetComponent<Text>().text = safeToLeave ? "[LEAVE AREA]" : "[EMERGENCY JUMP]";
        _exitButton.GetComponent<GenericTooltipHandler>().Initialize(
            () => TooltipManager.getInstance.OpenTooltip(safeToLeave ? "Return the system map." : "Plotting an emergency jump will take 30 seconds.", Input.mousePosition),
            () => GameManager.Leave(),
            null,
            null,
            () => TooltipManager.getInstance.CloseTooltip());
    }
    void OnSimulationSpeedChanged(SimulationSpeed currentSpeed)
    {
        //update time buttons
        for (int i = 0; i < _simulationSpeed.transform.childCount; i++)
        {
            Text t = _simulationSpeed.transform.GetChild(i).GetComponentInChildren<Text>();
            t.color = i == (int)currentSpeed ? _simulationSpeedCurrentColor : _simulationSpeedDefaultColor;
        }
    }

    void CreateGroupItem(int index)
    {
        ShipGroup group = CommandMapper.GetGroup(index);
        GameObject gi = Instantiate(_groupItem, null);
        gi.transform.SetParent(_groups);

        gi.transform.Find("header").transform.Find("header").GetComponent<Text>().text = "[GROUP]: " + index;
        gi.transform.Find("header").transform.Find("header").GetComponent<GenericTooltipHandler>().Initialize(
            () => TooltipManager.getInstance.OpenTooltip("[<color=yellow>LEFT-CLICK</color>] to select.\n[<color=yellow>SCROLL-CLICK</color>] to focus.", Input.mousePosition),
            () => CommandMapper.SelectGroup(index),
            () => CameraManager.getInstance.JumpTo(CommandMapper.GetGroup(index).GetCenter(), true),
            null,
            () => TooltipManager.getInstance.CloseTooltip());
        gi.transform.Find("header").transform.Find("remove").GetComponent<GenericTooltipHandler>().Initialize(
            () => TooltipManager.getInstance.OpenTooltip("Remove group.", Input.mousePosition),
            () => CommandMapper.RemoveGroup(index),
            null,
            null,
            () => TooltipManager.getInstance.CloseTooltip());

        Transform list = gi.transform.Find("list");

        for (int i = 0; i < group.ships.Count; i++)
            CreateGroupShipItem(list, group.ships[i]);

        _groupItems[index] = gi;
        _groupItems[index].transform.SetSiblingIndex(index);

        UpdateGroupsVisibility();
    }
    void CreateGroupShipItem(Transform list, ShipEntity ship)
    {
        GameObject g = Instantiate(_groupShipItem, list);

        g.transform.Find("name").GetComponent<Text>().text = ship.name;

        Image hl = g.transform.Find("highlight").GetComponent<Image>();
        Image s = g.transform.Find("shield").GetComponent<Image>();
        Image h = g.transform.Find("hull").GetComponent<Image>();

        s.fillAmount = ship.GetVital(VitalType.ShieldPoints).inPercent;
        h.fillAmount = ship.GetVital(VitalType.HullPoints).inPercent;

        g.GetComponent<GenericTooltipHandler>().Initialize(
            () => TooltipManager.getInstance.OpenTooltip("[<color=yellow>LEFT-CLICK</color>] to select.\n[<color=yellow>SCROLL-CLICK</color>] to focus.", Input.mousePosition),
            () => CommandMapper.SelectShip(ship),
            () => CameraManager.getInstance.JumpTo(ship.transform.position, true),
            null,
            () => TooltipManager.getInstance.CloseTooltip());

        g.GetComponent<GroupShipUIEntity>().Initialize(hl, s, h, ship);
    }

    void RemoveGroupItem(int index)
    {
        Transform list = _groupItems[index].gameObject.transform.Find("list");

        for (int i = 0; i < list.childCount; i++)
            list.GetChild(i).GetComponent<GroupShipUIEntity>().OnRemoved();

        Destroy(_groupItems[index].gameObject);
        UpdateGroupsVisibility();
    }
    void UpdateGroupsVisibility()
    {
        _groups.gameObject.SetActive(CommandMapper.hasGroups);
    }

    public void CreateContactItem(string text, string tooltip, Vector3 position, float lifetime = -1f)
    {
        GameObject g = Instantiate(_contactItem, _canvas);

        g.transform.position = _camera.WorldToScreenPoint(position);
        g.GetComponentInChildren<Text>().text = text;
        g.GetComponent<GenericTooltipHandler>().Initialize(
            () => TooltipManager.getInstance.OpenTooltip(tooltip, Input.mousePosition),   //should bring up tooltip
            () => CommandMapper.AddMove(position, !Input.GetKey(KeyCode.LeftShift)),
            delegate
            {
                if (Input.GetKey(KeyCode.LeftAlt))
                    CommandMapper.SetAlignmentPlane(position.y);
                else
                    CameraManager.getInstance.JumpTo(position, true);
            },
            null,
            () => TooltipManager.getInstance.CloseTooltip());  //should kill tooltip

        _contactItems.Add(new ContactItem(g, position));

        if (lifetime > 0f)
            Destroy(g, lifetime);
    }
    void UpdateContactItems()
    {
        for (int i = 0; i < _contactItems.Count; i++)
        {
            if (_contactItems[i].item == null)
                _contactItems.RemoveAt(i);
            else
            {
                Vector3 p = _camera.WorldToViewportPoint(_contactItems[i].position);
                Vector3 op = _camera.WorldToScreenPoint(_contactItems[i].position);
                op.z = 0;

                _contactItems[i].item.transform.position = op;
                _contactItems[i].item.SetActive(p.x > 0 && p.x < 1 && p.y > 0 && p.y < 1 && p.z > 0);
            }
        }
    }

    public void OpenSelectionBox(Vector3 origin)
    {
        _selectionBoxOrigin = origin;
        _drawSelectionBox = true;
    }
    public void CloseSelectionBox()
    {
        _drawSelectionBox = false;
    }
    Rect GetScreenRect(Vector3 start, Vector3 end)
    {
        start.y = Screen.height - start.y;
        end.y = Screen.height - end.y;

        var tl = Vector3.Min(start, end);
        var br = Vector3.Max(start, end);

        return Rect.MinMaxRect(tl.x, tl.y, br.x, br.y);
    }
    void DrawScreenRect(Rect rect, Color color)
    {
        GUI.color = color;
        GUI.DrawTexture(rect, _whiteTexture);
        GUI.color = Color.white;
    }
    void DrawScreenRectBorder(Rect rect, float thickness, Color color)
    {
        DrawScreenRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);
        DrawScreenRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);
        DrawScreenRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color);
        DrawScreenRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color);
    }

    void OnLeave()
    {
        CommandMapper.OnGroupCreated -= CreateGroupItem;
        CommandMapper.OnGroupRemoved -= RemoveGroupItem;

        GameManager.OnGameStatusUpdated -= UpdateExitButton;
        GameManager.OnLeave -= OnLeave;
    }

    IEnumerator UpdateCooldownUI(ShipComponent component, Image image)
    {
        while (image != null)
        {
            image.fillAmount = component.cooldownRemainingInPercent;
            yield return null;
        }
    }
}
public class ContactItem
{
    public GameObject item;
    public Vector3 position;

    public ContactItem(GameObject item, Vector3 position)
    {
        this.item = item;
        this.position = position;
    }
}