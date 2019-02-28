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
        if (!OverworldManager.isPlayerTurn || PlayerData.fleet.isBusy || DialogueUIManager.getInstance.isOpen || ConsoleManager.getInstance.isOpen)
            return;

        if (Input.GetKeyDown(KeyCode.Mouse0))
            ProcessLeftClick();
    }

    void ProcessLeftClick()
    {
        if (!OverworldManager.isPlayerTurn || PlayerData.fleet.isBusy || EventSystem.current.IsPointerOverGameObject())
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
            MovePlayerFleet(hit.point);
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
                OverworldCameraManager.getInstance.JumpTo(poi.location);
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
        if (PlayerData.fleet.GetVital(FleetVitalType.Movement).current >= Vector3.Distance(PlayerData.fleet.location, poi.location))
        {
            if (poi.type == PointOfInterestType.Fleet)
                ((Fleet)poi).Intercept(PlayerData.fleet);
            else
                poi.Interact();
        }
        else
            MovePlayerFleet(poi.location);
    }
    void MovePlayerFleet(Vector3 position)
    {
        PlayerData.fleet.Move(position);
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
