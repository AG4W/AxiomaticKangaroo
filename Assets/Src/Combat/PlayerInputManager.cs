using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerInputManager : MonoBehaviour
{
    static PlayerInputManager _instance;
    public static PlayerInputManager getInstance { get { return _instance; } }

    Vector3 _selectionBoxOrigin;
    Camera _camera;

    float _alignmentPlaneSensitivity = 250f;
    float _doubleClickThreshold = .2f;
    float _mouse0timer = 0f;
    float _mouse1timer = 0f;

    KeyCode[] _alphaKeys = new KeyCode[] 
    {
        KeyCode.Alpha0,
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5,
        KeyCode.Alpha6,
        KeyCode.Alpha7,
        KeyCode.Alpha8,
        KeyCode.Alpha9,
    };

    bool _shiftModifier = false;
    bool _controlModifier = false;
    
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
        _shiftModifier = Input.GetKey(KeyCode.LeftShift);
        _controlModifier = Input.GetKey(KeyCode.LeftControl);

        if (Input.GetKeyDown(KeyCode.KeypadPlus))
            CommandMapper.IncrementSimulationSpeed();
        else if (Input.GetKeyDown(KeyCode.KeypadMinus))
            CommandMapper.DecrementSimulationSpeed();
        else if (Input.GetKeyDown(KeyCode.Space))
            CommandMapper.ToggleSimulation();

        //48 <-> 57 == 0 - 9
        for (int i = 0; i < _alphaKeys.Length; i++)
        {
            if (Input.GetKeyDown(_alphaKeys[i]))
            {
                if (Input.GetKey(KeyCode.CapsLock))
                    CreateGroup(i);
                else
                    SelectGroup(i);
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab))
            CommandMapper.ToggleAlignmentPlane();

        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetKeyDown(KeyCode.Mouse0))
            _selectionBoxOrigin = Input.mousePosition;
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            if (_mouse0timer <= _doubleClickThreshold)
                ProcessPrimaryKey();
            else
                CommandMapper.CloseSelectionBox();

            _mouse0timer = 0f;
        }
        if (Input.GetKey(KeyCode.Mouse0))
        {
            _mouse0timer += Time.unscaledDeltaTime;

            if (_mouse0timer > _doubleClickThreshold)
                CommandMapper.OpenSelectionBox(_selectionBoxOrigin);
        }
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            if (_mouse1timer <= _doubleClickThreshold)
                ProcessSecondaryKey();
            else
            {
                //OnSecondaryKeyUp after hold
            }

            _mouse1timer = 0f;
        }
        if (Input.GetKey(KeyCode.Mouse1))
        {
            _mouse1timer += Time.unscaledDeltaTime;

            if (_mouse1timer > _doubleClickThreshold)
            {
                //rotate heading of selected ships
                ShiftAlignmentPlane();
            }
        }
    }

    void ProcessPrimaryKey()
    {
        Vector3 p = GetClickedPosition();
        GameObject g = GetClickedObject();
        ShipEntity se = GetClickedShip(g);

        //if we leftclicked on a ship
        if (se != null)
        {
            //if its player-owned
            if (se.teamID == 0)
                CommandMapper.SelectShip(se, _shiftModifier, _controlModifier);
            else
                CommandMapper.UpdateTarget(se);
        }
        else
            CommandMapper.AddMove(p, !_shiftModifier);
    }
    void ProcessSecondaryKey()
    {
        //ignore UI clicks
        //if (EventSystem.current.IsPointerOverGameObject())
        //    return;

        CommandMapper.ClearSelection();
    }

    void SelectGroup(int index)
    {
        CommandMapper.SelectGroup(index);
    }
    void CreateGroup(int index)
    {
        CommandMapper.CreateGroup(index);
    }

    void ShiftAlignmentPlane()
    {
        float y = Input.GetAxisRaw("Mouse Y") * _alignmentPlaneSensitivity * Time.unscaledDeltaTime;
        CommandMapper.ShiftAlignmentPlane(y);
    }

    Vector3 GetClickedPosition()
    {
        RaycastHit hit;
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, (1 << 31)))
            return hit.point;

        return Vector3.zero;
    }
    GameObject GetClickedObject()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out hit, Mathf.Infinity, (1 << 30)))
            return hit.transform.gameObject;

        return null;
    }
    ShipEntity GetClickedShip(GameObject clickedObject)
    {
        if (clickedObject == null)
            return null;

        return clickedObject.transform.root.GetComponent<ShipEntity>();
    }
}
