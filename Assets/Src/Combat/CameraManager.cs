using UnityEngine;

public class CameraManager : MonoBehaviour
{
    static CameraManager _instance;
    public static CameraManager getInstance { get { return _instance; } }

    [SerializeField]float _movementSpeed = 5f;
    [SerializeField]float _rotationSpeed = 5f;

    Camera _camera;
    Vector3 _targetPosition;

    void Awake()
    {
        _instance = this;
        _camera = this.GetComponentInChildren<Camera>();
    }
	void Update()
    {
        UpdateMovePosition();

        if (Input.GetKey(KeyCode.Mouse2))
            Rotate();

        InterpolatePosition();
	}

    void UpdateMovePosition()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        float y = Input.GetAxisRaw("Depthical");

        Vector3 md = this.transform.TransformDirection(x, y, z).normalized;
        _targetPosition += md;
    }
    void Rotate()
    {
        float x = Input.GetAxisRaw("Mouse X");
        float y = Input.GetAxisRaw("Mouse Y");

        //Vector3 ld = new Vector3(-y, x, 0);

        //this.transform.eulerAngles += (ld * _rotationSpeed * Time.unscaledDeltaTime);
        this.transform.Rotate(Vector3.up, x * _rotationSpeed * Time.unscaledDeltaTime);
        _camera.transform.Rotate(Vector3.right, -y * _rotationSpeed * Time.unscaledDeltaTime);
    }
    void InterpolatePosition()
    {
        this.transform.position = Vector3.Lerp(this.transform.position, _targetPosition, _movementSpeed * Time.unscaledDeltaTime);
    }

    public void JumpTo(Vector3 position, bool addOffset = false)
    {
        if (addOffset)
        {
            position += (this.transform.forward * -1f) * 50f;
            position += this.transform.up * 25f;
        }

        _targetPosition = position;
    }
}
