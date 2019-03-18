using UnityEngine;

public class OverworldInputManager : MonoBehaviour
{
    static OverworldInputManager _instance;
    public static OverworldInputManager getInstance { get { return _instance; } }

    Camera _camera;

    void Awake()
    {
        _instance = this;
    }
    void Start()
    {
        _camera = Camera.main;
    }
    void Update()
    {
        GetInputs();
    }

    void GetInputs()
    {
        if (DialogueUIManager.getInstance.isOpen || ConsoleManager.getInstance.isOpen)
            return;

        if (!OverworldManager.isPlayerTurn)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
            ProcessSpacebar();
    }

    public void HandlePoIMouseCallback(CallbackType callbackType, PointOfInterest poi)
    {
        switch (callbackType)
        {
            case CallbackType.MouseEnter:
                break;
            case CallbackType.LeftDown:
                break;
            case CallbackType.ScrollDown:
                OverworldCameraManager.getInstance.JumpTo(poi.cell.location);
                break;
            case CallbackType.RightDown:
                break;
            case CallbackType.MouseExit:
                break;
            default:
                break;
        }
    }

    void ProcessSpacebar()
    {
        OverworldManager.EndCurrentTurn();
    }
}
public enum CallbackType
{
    MouseEnter,
    LeftDown,
    ScrollDown,
    RightDown,
    MouseExit
}
