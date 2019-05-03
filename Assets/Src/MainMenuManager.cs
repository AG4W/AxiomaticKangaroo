using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;

using System.Collections;
using System.Diagnostics;
using System.Linq;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]PostProcessVolume _mainCameraPost;

    [SerializeField]GameObject _mainTitle;
    [SerializeField]GameObject _planet;
    [SerializeField]PlanetType _planetType;

    [Header("Menu Items")]
    [SerializeField]GameObject _new;
    [SerializeField]GameObject _load;
    [SerializeField]GameObject _instructions;
    [SerializeField]GameObject _options;
    [SerializeField]GameObject _credits;
    [SerializeField]GameObject _quit;

    [Header("Media")]
    [SerializeField]Transform _media;

    [Header("Panels")]
    [Header("Setup")]
    [SerializeField]GameObject _setupPanel;
    [SerializeField]Dropdown _difficulties;

    [SerializeField]Text _seed;
    [SerializeField]Text _briefing;

    [SerializeField]GameObject _randomizeSeed;
    [SerializeField]GameObject _start;

    [Header("Saves")]
    [SerializeField]GameObject _savesPanel;
    [SerializeField]GameObject _saveItem;
    [SerializeField]GameObject _noSavesItem;
    [SerializeField]Transform _savesList;

    [Header("Instructions")]
    [SerializeField]GameObject _instructionsPanel;
    [SerializeField]GameObject _instructionItem;
    [SerializeField]Transform _instructionsList;

    [Header("Options")]
    [SerializeField]GameObject _optionsPanel;

    [Header("Credits")]
    [SerializeField]GameObject _creditsPanel;

    GameObject[] _panels;

    void Start()
    {
        //initialize settings
        Settings.Load();
        //load misc stuff
        NameGenerator.Initialize();
        ModelDB.Initialize();
        ItemDB.Initialize();
        EventDB.Initialize();
        RarityDB.Initialize();
        TraitDB.Initialize();
        PaletteDB.Initialize();
        //initalize event manager
        Event.Initialize();

        //load saves first
        SaveManager.Initialize();

        _planet.GetComponent<PlanetEntity>().Initialize(
            PaletteDB.Get(_planetType).Instantiate(), 
            Random.Range(0.2f, .9f), 
            true);

        //hide stuff
        _panels = new GameObject[] { _setupPanel, _savesPanel, _instructionsPanel, _optionsPanel, _creditsPanel };

        for (int i = 0; i < _panels.Length; i++)
            _panels[i].SetActive(false);

        InitalizeMediaIcons();
        InitializeMenues();

        //start main ost
        AudioManager.getInstance.PlayOST(OSTTheme.MainMenu);
    }
    void InitalizeMediaIcons()
    {
        _media.transform.Find("twitter").GetComponent<GenericTooltipHandler>().Initialize(
            () => TooltipManager.getInstance.OpenTooltip("Reach me on Twitter!", Input.mousePosition),
            () => Process.Start("http://twitter.com/n4ttuggla"),
            null,
            null,
            () => TooltipManager.getInstance.CloseTooltip());

        _media.transform.Find("youtube").GetComponent<GenericTooltipHandler>().Initialize(
            () => TooltipManager.getInstance.OpenTooltip("Coming soon!", Input.mousePosition),
            null,
            null,
            null,
            () => TooltipManager.getInstance.CloseTooltip());

        _media.transform.Find("patreon").GetComponent<GenericTooltipHandler>().Initialize(
            () => TooltipManager.getInstance.OpenTooltip("Support me on Patreon!", Input.mousePosition),
            () => Process.Start("https://patreon.com/stellarwinds"),
            null,
            null,
            () => TooltipManager.getInstance.CloseTooltip());

        _media.transform.Find("discord").GetComponent<GenericTooltipHandler>().Initialize(
            () => TooltipManager.getInstance.OpenTooltip("Join the community!", Input.mousePosition),
            () => Process.Start("https://discord.gg/pygc2Kw"),
            null,
            null,
            () => TooltipManager.getInstance.CloseTooltip());
    }
    void InitializeMenues()
    {
        _mainTitle.GetComponent<GenericTooltipHandler>().Initialize(
            null,
            delegate
            {
                StopAllCoroutines();
                StartCoroutine(SwitchPlanetLevelsAsync());
            },
            null,
            null,
            null);

        _mainTitle.transform.parent.Find("subtitle").GetComponent<Text>().text = "[<color=red>BUILD</color>]: " + Application.version;

        _new.GetComponent<GenericTooltipHandler>().Initialize(
            () => TooltipManager.getInstance.OpenTooltip("Start a new campaign.", Input.mousePosition),
            () => ToggleSetupMenu(),
            null,
            null,
            () => TooltipManager.getInstance.CloseTooltip());

        _load.GetComponent<GenericTooltipHandler>().Initialize(
            () => TooltipManager.getInstance.OpenTooltip("Load an existing campaign. <color=red>(not available in " + Application.version + ")</color>", Input.mousePosition),
            () => ToggleSavesMenu(),
            null,
            null,
            () => TooltipManager.getInstance.CloseTooltip());

        _instructions.GetComponent<GenericTooltipHandler>().Initialize(
            () => TooltipManager.getInstance.OpenTooltip("Having trouble understanding a concept? Check here.", Input.mousePosition),
            () => ToggleInstructionsMenu(),
            null,
            null,
            () => TooltipManager.getInstance.CloseTooltip());

        _options.GetComponent<GenericTooltipHandler>().Initialize(
            () => TooltipManager.getInstance.OpenTooltip("Customize audio, gameplay and video options.<color=red> (audio settings only in " + Application.version + ")</color>", Input.mousePosition),
            () => ToggleOptionsMenu(),
            null,
            null,
            () => TooltipManager.getInstance.CloseTooltip());

        _credits.GetComponent<GenericTooltipHandler>().Initialize(
            () => TooltipManager.getInstance.OpenTooltip("View credits roll.", Input.mousePosition),
            () => TogglePanels(_creditsPanel),
            null,
            null,
            () => TooltipManager.getInstance.CloseTooltip());

        _quit.GetComponent<GenericTooltipHandler>().Initialize(
            () => TooltipManager.getInstance.OpenTooltip("Quit to the desktop.", Input.mousePosition),
            () => Application.Quit(),
            null,
            null,
            () => TooltipManager.getInstance.CloseTooltip());

        InitializeInstructionsMenu();
    }

    void ToggleSetupMenu()
    {
        TogglePanels(_setupPanel);

        if (_setupPanel.activeSelf)
            UpdateSetupMenu();
    }
    void UpdateSetupMenu()
    {
        if (RuntimeData.save == null)
            RuntimeData.SetSave(new Save("new game", new SaveData(Random.Range(0, int.MaxValue))));

        _seed.text = RuntimeData.save.data.seed.ToString();
        _randomizeSeed.GetComponent<GenericTooltipHandler>().Initialize(
            () => TooltipManager.getInstance.OpenTooltip("Randomize seed.", Input.mousePosition),
            delegate
            {
                RuntimeData.SetSave(new Save("new game", new SaveData(Random.Range(0, int.MaxValue))));
                UpdateSetupMenu();
            },
            null,
            null,
            () => TooltipManager.getInstance.CloseTooltip());

        _difficulties.onValueChanged.RemoveAllListeners();
        _difficulties.onValueChanged.AddListener(
            delegate
            {
                RuntimeData.save.data.SetDifficulty((Difficulty)_difficulties.value);
            });

        _start.GetComponent<GenericTooltipHandler>().Initialize(
            () => TooltipManager.getInstance.OpenTooltip("Good luck!", Input.mousePosition),
            delegate
            {
                StarSystem system = new StarSystem(int.Parse(_seed.text));
                system.Generate();

                RuntimeData.SetSystem(system);
                SceneManager.LoadScene("Overworld");
            },
            null,
            null,
            () => TooltipManager.getInstance.CloseTooltip());

        _briefing.text = 
            CommandManager.ProcessFlags(
                "[<color=red>SCENARIO</color>]:\n\n" +
                "Following the surprise strike and apocalyptic destruction of its core worlds, and the destruction of the <color=cyan>1st</color>, <color=cyan>4th</color> and <color=cyan>13th</color> Homeguard Fleets above the surface of <color=yellow>" + RuntimeData.save.data.player.homePlanet + "</color> - the <color=cyan>" + RuntimeData.save.data.player.name + "</color> accepted an unconditional surrender, and was subsquently annexed by the <color=red>" + RuntimeData.save.data.enemy.name + "</color>.\n\n" +
                "Slated for a long-awaited period of rest and repairs at the <color=yellow>" + NameGenerator.GetPOIName(PointOfInterestType.Planet) + " Shipyards</color>, your fleet was caught off-guard by a superior <color=red>" + RuntimeData.save.data.enemy.name + "</color> raiding fleet.\n\n" +
                "Outmatched and outgunned, with the majority of your fleet destroyed, you ordered an uncalculated emergency FTL-jump.\n" +
                "With your navigational systems out of function, your navigational officers had to plot the jump manually under stress - creating huge inaccuracies in the final destination.\n\n" +
                "Nighly escaping certain doom, your fleet travelled instantly across space in the blink of an eye.\n" +
                "Emerging battered, but alive, in an unknown star system in an uncharted area of space.\n\n" +
                "Determined to survive and carry the flame of resistance, your journey home begins.\n\n" +
                "[<color=red>TRAITS</color>]:\n\n" +
                "LISTFACTIONMODIFIERS");
    }

    void ToggleSavesMenu()
    {
        TogglePanels(_savesPanel);

        if (_savesPanel.activeSelf)
            UpdateSavesMenu();
    }
    void UpdateSavesMenu()
    {
        for (int i = 0; i < _savesList.childCount; i++)
            Destroy(_savesList.GetChild(i).gameObject);

        if (SaveManager.hasSaves)
        {
            for (int i = 0; i < SaveManager.saves.Count; i++)
            {
                int a = i;

                GameObject g = Instantiate(_saveItem, _savesList);

                g.transform.Find("lastPlayed").GetComponent<Text>().text = "[<color=red>LAST PLAYED</color>]: " + SaveManager.saves[i].datestamp;
                g.transform.Find("title").GetComponent<Text>().text = "[<color=red>FILE</color>]: " + SaveManager.saves[i].name + ", build: " + SaveManager.saves[i].build;

                g.transform.Find("load").GetComponent<GenericTooltipHandler>().Initialize(
                    () => TooltipManager.getInstance.OpenTooltip("Load this savefile.", Input.mousePosition),
                    null,
                    null,
                    null,
                    () => TooltipManager.getInstance.CloseTooltip());

                g.transform.Find("delete").GetComponent<GenericTooltipHandler>().Initialize(
                    () => TooltipManager.getInstance.OpenTooltip("Delete this savefile.", Input.mousePosition),
                    delegate
                    {
                        SaveManager.Delete(SaveManager.saves[a]);
                        TooltipManager.getInstance.CloseTooltip();
                        UpdateSavesMenu();
                    },
                    null,
                    null,
                    () => TooltipManager.getInstance.CloseTooltip());
            }
        }
        else
            Instantiate(_noSavesItem, _savesList);
    }

    void InitializeInstructionsMenu()
    {
        TextAsset[] entries = Resources.LoadAll<TextAsset>("Manual/");

        for (int i = 0; i < entries.Length; i++)
        {
            GameObject g = Instantiate(_instructionItem, _instructionsList);

            g.transform.SetSiblingIndex(int.Parse(entries[i].name.Substring(0, 1)));
            g.transform.Find("header").GetComponent<Text>().text = "[<color=orange>" + CommandManager.ProcessFlags(entries[i].name) + "</color>]";
            g.transform.Find("text").GetComponent<Text>().text = CommandManager.ProcessFlags(entries[i].text);
        }
    }
    void ToggleInstructionsMenu()
    {
        TogglePanels(_instructionsPanel);
    }
    
    void ToggleOptionsMenu()
    {
        TogglePanels(_optionsPanel);

        if (_optionsPanel.activeSelf)
            UpdateOptionsMenu();
    }
    void UpdateOptionsMenu()
    {
        Transform l = _optionsPanel.transform.Find("list");

        //setup audio
        for (int i = 0; i < System.Enum.GetNames(typeof(VolumeSource)).Length; i++)
        {
            VolumeSource v = (VolumeSource)i;
            UpdateVolumeItem(l.Find(v.ToString().ToLower()), v);
        }
        //setup graphics
        for (int i = 0; i < System.Enum.GetNames(typeof(GraphicSetting)).Length; i++)
        {
            GraphicSetting g = (GraphicSetting)i;
            UpdateSimpleGraphicItem(l.Find(g.ToString().ToLower()), g);
        }
    }
    void UpdateVolumeItem(Transform root, VolumeSource vs)
    {
        Slider slider = root.GetComponentInChildren<Slider>();

        slider.value = Settings.GetVolume(vs);
        slider.onValueChanged.RemoveAllListeners();
        slider.onValueChanged.AddListener(delegate 
            {
                Settings.SetVolume(vs, slider.value);
            });
    }
    void UpdateSimpleGraphicItem(Transform root, GraphicSetting ppe)
    {
        Dropdown dropdown = root.GetComponentInChildren<Dropdown>();

        dropdown.options[0].text = "On";
        dropdown.options[1].text = "Off";

        dropdown.onValueChanged.RemoveAllListeners();
        dropdown.onValueChanged.AddListener(delegate
        {
            Settings.SetGraphicSetting(ppe, dropdown.value == 0);
        });
    }

    void TogglePanels(GameObject gameObject)
    {
        for (int i = 0; i < _panels.Length; i++)
        {
            if (_panels[i] == gameObject)
                gameObject.SetActive(!gameObject.activeSelf);
            else
                _panels[i].SetActive(false);
        }

        StopCoroutine("InterpolateDoFAsync");
        StartCoroutine(InterpolateDoFAsync(_panels.Any(p => p.activeSelf)));
    }

    IEnumerator SwitchPlanetLevelsAsync()
    {
        Renderer renderer = _planet.transform.Find("planet").GetComponent<Renderer>();
        float owl = renderer.material.GetFloat("_waterLevel");
        float nwl = Random.Range(0.2f, .9f);

        float t = 0f;

        while(t <= 3f)
        {
            t += Time.deltaTime;
            renderer.material.SetFloat("_waterLevel", Mathf.Lerp(owl, nwl, t / 3f));
            yield return null;
        }
    }
    IEnumerator InterpolateDoFAsync(bool fade)
    {
        DepthOfField dof = _mainCameraPost.profile.GetSetting<DepthOfField>();
        float t = 0f;
        float duration = .5f;

        while(t <= duration)
        {
            t += Time.deltaTime;

            if (fade)
                dof.focusDistance.value = Mathf.Lerp(2.5f, 0.1f, t / duration);
            else
                dof.focusDistance.value = Mathf.Lerp(0.1f, 2.5f, t / duration);

            yield return null;
        }
    }
}
