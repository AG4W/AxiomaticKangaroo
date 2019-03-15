using UnityEngine;

public class OverworldCameraManager : MonoBehaviour
{
    static OverworldCameraManager _instance;
    public static OverworldCameraManager getInstance { get { return _instance; } }

    [SerializeField]float _rotationSpeed = 5f;
    [SerializeField]float _translationSpeed = 5f;
    [SerializeField]float _cameraTranslationSpeed = 25f;

    [SerializeField]Offset[] _cameraOffsets;
    int _offsetIndex = 1;

    Vector3 _jigTargetPosition;

    Camera _camera;

    void Awake()
    {
        _instance = this;
    }
    void Start()
    {
        _camera = this.GetComponentInChildren<Camera>();

        _offsetIndex = _cameraOffsets.Length - 1;
        MoveCameraToOffset();
    }
    void Update()
    {
        if(RuntimeData.system != null && !DialogueUIManager.getInstance.isOpen)
        {
            UpdateMovePosition();
            UpdateCameraOffset();

            Rotate();
        }

        Interpolate();
    }

    void UpdateMovePosition()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 forward = _offsetIndex == _cameraOffsets.Length - 1 ? _camera.transform.up : _camera.transform.forward;
        Vector3 right = _camera.transform.right;

        //project to horizontal plane
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = forward * z + right * x;

        _jigTargetPosition += (moveDirection * (_offsetIndex + 1));

        float s = (RuntimeData.system.size * HexMetrics.size) * 2;

        _jigTargetPosition.x = Mathf.Clamp(_jigTargetPosition.x, 0, s);
        _jigTargetPosition.z = Mathf.Clamp(_jigTargetPosition.z, 0, s);
    }
    void UpdateCameraOffset()
    {
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            if (_offsetIndex != 0)
                _offsetIndex--;
        }
        else if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            if (_offsetIndex < _cameraOffsets.Length - 1)
                _offsetIndex++;
        }

        MoveCameraToOffset();
    }

    void Rotate()
    {
        float r = Input.GetAxisRaw("Depthical");

        this.transform.Rotate(this.transform.up, r * _rotationSpeed * Time.unscaledDeltaTime);
    }

    void Interpolate()
    {
        this.transform.position = Vector3.Lerp(
                this.transform.position,
                _jigTargetPosition,
                _translationSpeed * Time.deltaTime);

        //camera
        //_camera.transform.localPosition = 
        //    Vector3.Lerp(
        //        _camera.transform.localPosition, 
        //        _cameraTargetPosition,
        //        _zoomSpeed * Time.deltaTime);
    }
    void MoveCameraToOffset()
    {
        _camera.transform.localPosition = Vector3.Lerp(_camera.transform.localPosition, _cameraOffsets[_offsetIndex].position, _cameraTranslationSpeed * Time.unscaledDeltaTime);
        _camera.transform.localEulerAngles = Vector3.Lerp(_camera.transform.localEulerAngles, _cameraOffsets[_offsetIndex].eulerAngles, _cameraTranslationSpeed * Time.unscaledDeltaTime);
    }

    public void JumpTo(Vector3 position)
    {
        _jigTargetPosition = position;
    }
}
[System.Serializable]
public struct Offset
{
    [SerializeField]Vector3 _position;
    [SerializeField]Vector3 _eulerAngles;

    public Vector3 position { get { return _position; } }
    public Vector3 eulerAngles { get { return _eulerAngles; } }

    public Offset(Vector3 position, Vector3 eulerAngles)
    {
        _position = position;
        _eulerAngles = eulerAngles;
    }
}
