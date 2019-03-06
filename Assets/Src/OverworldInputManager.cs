using UnityEngine;
using UnityEngine.EventSystems;

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

        if (!OverworldManager.isPlayerTurn || PlayerData.fleet.isBusy)
            return;
    }

    public void HandlePoIMouseCallback(CallbackType callbackType, PointOfInterest poi)
    {
        switch (callbackType)
        {
            case CallbackType.MouseEnter:
                break;
            case CallbackType.LeftDown:
                ProcessPoILeftClick(poi);
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

    void ProcessPoILeftClick(PointOfInterest poi)
    {
        if (PlayerData.fleet.GetVital(FleetVitalType.Range).current >= Vector3.Distance(PlayerData.fleet.cell.location, poi.cell.location))
        {
            if (poi.type == PointOfInterestType.Fleet)
                ((Fleet)poi).Intercept(PlayerData.fleet);
            else
                poi.Interact();
        }
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
