using UnityEngine;
using UnityEngine.UI;

public class ConsoleManager : MonoBehaviour
{
    static ConsoleManager _instance;
    public static ConsoleManager getInstance { get { return _instance; } }

    [SerializeField]GameObject _console;
    [SerializeField]InputField _input;

    public bool isOpen { get { return _console.activeSelf; } }

    void Awake()
    {
        if (_instance != null)
            Destroy(this.gameObject);
        else
        {
            _instance = this;
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
        if (Input.GetKeyDown(KeyCode.Return))
            _console.SetActive(!_console.activeSelf);
    }

    void ProcessInput(string input)
    {
        CommandManager.ExecuteCommand(input.ToLower());
    }
}
