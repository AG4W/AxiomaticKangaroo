using UnityEngine;
using UnityEngine.UI;

public class ConsoleManager : MonoBehaviour
{
    public static ConsoleManager getInstance { get; private set; }

    [SerializeField]GameObject _console;
    [SerializeField]GameObject _commandItem;

    [SerializeField]Transform _commands;
    [SerializeField]InputField _input;

    public bool isOpen { get { return _console.activeSelf; } }

    void Awake()
    {
        if (getInstance != null)
            Destroy(this.gameObject);
        else
        {
            getInstance = this;
            _console.SetActive(false);

            DontDestroyOnLoad(this.gameObject);
        }
    }
    void Start()
    {
        _input.onEndEdit.AddListener(delegate { ProcessInput(_input.text); });
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Delete))
            _console.SetActive(!_console.activeSelf);
    }

    void ProcessInput(string input)
    {
        CommandManager.ExecuteCommand(input.ToLower());
    }

    public static void Print(string text)
    {
        getInstance.PrintInternal(text);
    }
    void PrintInternal(string text)
    {
        if (_commands.childCount > 15)
            for (int i = 0; i < _commands.childCount; i++)
                Destroy(_commands.GetChild(i).gameObject);

        GameObject g = Instantiate(_commandItem, _commands);
        g.transform.Find("body").GetComponent<Text>().text = text;
    }
}
