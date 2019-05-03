using UnityEngine;
using UnityEngine.UI;

public class DialogueUIManager : MonoBehaviour
{
    static DialogueUIManager _instance;
    public static DialogueUIManager getInstance { get { return _instance; } }

    [SerializeField]GameObject _window;
    [SerializeField]Text _header;
    [SerializeField]Text _body;

    [SerializeField]Transform _options;
    [SerializeField]GameObject _optionItem;

    public bool isOpen { get { return _window.activeSelf; } }

    void Awake()
    {
        if(_instance != null)
            Destroy(this.transform.gameObject);
        else
        {
            _instance = this;
            _window.SetActive(false);

            DontDestroyOnLoad(this.transform);
        }
    }

    public void DisplayDialogueEvent(DialogueEvent dialogue)
    {
        for (int i = 0; i < _options.childCount; i++)
            Destroy(_options.GetChild(i).gameObject);

        //create options
        for (int i = 0; i < dialogue.options.Count; i++)
            CreateDialogueOption(dialogue.options[i], i);

        LogManager.getInstance.AddEntry("[Event Received]: '" + dialogue.name + "'", 15f, EntryType.Dialogue);

        _header.text = CommandManager.ProcessFlags(dialogue.name);
        _body.text = CommandManager.ProcessFlags(dialogue.body);

        float height = 0;

        //header
        height += 5 + 15 + 5;
        //art
        height += 150 + 5;
        //body 
        float bodyHeight = _body.preferredHeight;

        RectTransform bt = (RectTransform)_body.transform.parent;
        bt.sizeDelta = new Vector2(bt.sizeDelta.x, bodyHeight);

        height += bodyHeight + 5;

        //options
        int optionsHeight = 25 * (dialogue.options.Count) + 5 * (dialogue.options.Count - 1);

        RectTransform ot = (RectTransform)_options;
        ot.sizeDelta = new Vector2(ot.sizeDelta.x, optionsHeight);

        height += optionsHeight + 5;

        RectTransform rt = (RectTransform)_window.transform;
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, height);

        _window.gameObject.SetActive(true);
    }

    void CreateDialogueOption(DialogueOption option, int index)
    {
        GameObject o = Instantiate(_optionItem, _options);

        o.transform.Find("text").GetComponent<Text>().text = index + ". " + CommandManager.ProcessFlags(option.text);

        if(CommandManager.EvaluateOption(option))
            o.transform.GetComponent<GenericTooltipHandler>().Initialize(
                () => TooltipManager.getInstance.OpenTooltip(CommandManager.ProcessFlags(option.tooltip), Input.mousePosition),
                delegate 
                {
                    CommandManager.ExecuteCommand(option.commands);
                    TooltipManager.getInstance.CloseTooltip();

                    if(option.closeAfterSelect)
                        CloseDialogueEvent();
                },
                null,
                null,
                () => TooltipManager.getInstance.CloseTooltip());
        else
        {
            o.transform.GetComponent<GenericTooltipHandler>().Initialize(
                () => TooltipManager.getInstance.OpenTooltip(CommandManager.ProcessFlags(option.tooltip), Input.mousePosition),
                null,
                null,
                null,
                () => TooltipManager.getInstance.CloseTooltip());

            o.GetComponent<GenericTooltipHandler>().OverrideUIElementColor(Color.red);
            o.GetComponent<GenericTooltipHandler>().HighlightOnEnterExit(false);
        }
    }
    void CloseDialogueEvent()
    {
        _window.gameObject.SetActive(false);
    }
}