using UnityEngine;

public class MainCameraController : MonoBehaviour
{
    public float MovementSpeed = GlobalSettings.Camera.MovementSpeed;
    public float ZoomSpeed = GlobalSettings.Camera.ZoomSpeed;
    public float RotationSpeed = GlobalSettings.Camera.RotationSpeed;
    public float MaxYRotationAngle = GlobalSettings.Camera.MaxYRotationAngle;

    [Header("Zoom Limits")]
    public float MinZoomHeight = 10f;
    public float MaxZoomHeight = 18f;

    [Header("World Bounds")]
    public Vector3 MinWorldBounds = new Vector3(-50, 10, -50);
    public Vector3 MaxWorldBounds = new Vector3(50, 18, 50);

    [HideInInspector]
    public bool RotateCamera { get; set; } = false;

    [HideInInspector]
    public bool AllowCameraRotation { get; set; } = false;

    private Vector2 currentRotation;
    private Vector3 forward;
    private Vector3 right;

    void Start()
    {
        currentRotation = transform.eulerAngles
            .GetVector2InspectorAnglesFromEulerAngles()
            .SwapXAndY();

        CacheMovementVectors();
    }

    void Update()
    {
        if (RotateCamera && AllowCameraRotation)
        {
            Rotate();
            CacheMovementVectors();
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void MoveCamera(float horizontalInput, float verticalInput)
    {
        var movementVector = (right * horizontalInput + forward * verticalInput).normalized;

        transform.position += MovementSpeed * Time.deltaTime * movementVector;

        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, MinWorldBounds.x, MaxWorldBounds.x),
            Mathf.Clamp(transform.position.y, MinWorldBounds.y, MaxWorldBounds.y),
            Mathf.Clamp(transform.position.z, MinWorldBounds.z, MaxWorldBounds.z)
        );
    }

    public void ZoomCamera(float scrollWheelInput)
    {
        var newPosition = transform.position + scrollWheelInput * ZoomSpeed * transform.forward;

        var clampedY = Mathf.Clamp(newPosition.y, MinZoomHeight, MaxZoomHeight);

        if (Mathf.Abs(clampedY - transform.position.y) > Mathf.Epsilon)
        {
            newPosition.y = clampedY;
            transform.position = newPosition;
        }
    }

    private void CacheMovementVectors()
    {
        forward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        right = Vector3.ProjectOnPlane(transform.right, Vector3.up).normalized;
    }

    private void Rotate()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentRotation.x += Input.GetAxisRaw("Mouse X") * RotationSpeed;
        currentRotation.y -= Input.GetAxisRaw("Mouse Y") * RotationSpeed;
        currentRotation.x = Mathf.Repeat(currentRotation.x, 360);
        currentRotation.y = Mathf.Clamp(currentRotation.y, -MaxYRotationAngle, MaxYRotationAngle);

        transform.rotation = Quaternion.Euler(currentRotation.y, currentRotation.x, 0);
    }
}
