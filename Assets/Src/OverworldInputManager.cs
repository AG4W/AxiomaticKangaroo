using UnityEngine;
using UnityEngine.EventSystems;

public class OverworldInputManager : MonoBehaviour
{
    public static OverworldInputManager getInstance { get; private set; }

    Camera _camera;

    void Awake()
    {
        getInstance = this;
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

        if (Input.GetKeyDown(KeyCode.Space))
            ProcessSpacebar();

        if (EventSystem.current.IsPointerOverGameObject())
            return;
        
        if (Input.GetKeyDown(KeyCode.Mouse0))
            ProcessLeftClick();
    }

    public void HandlePoIMouseCallback(CallbackType callbackType, PointOfInterest poi)
    {
        switch (callbackType)
        {
            case CallbackType.MouseEnter:
                break;
            case CallbackType.LeftDown:
                float d = Vector3.Distance(poi.position, PlayerData.fleet.position);

                if (d > PlayerData.fleet.GetVital(FleetVitalType.Range).current)
                    PlayerData.fleet.Move(poi.position);
                else
                    poi.Interact();

                break;
            case CallbackType.ScrollDown:
                OverworldCameraManager.getInstance.JumpTo(poi.position);
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
    void ProcessLeftClick()
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        //0 here is the fixed "world" y-level, change to whatever else you want to align with
        PlayerData.fleet.Move(ray.origin - (ray.direction / ray.direction.y) * ray.origin.y);
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
