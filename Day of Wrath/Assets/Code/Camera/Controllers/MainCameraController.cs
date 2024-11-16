using UnityEngine;

public class MainCameraController : MonoBehaviour
{
    public float MovementSpeed = GlobalSettings.Camera.MovementSpeed;
    public float ZoomSpeed = GlobalSettings.Camera.ZoomSpeed;
    public float RotationSpeed = GlobalSettings.Camera.RotationSpeed;
    public float MaxYRotationAngle = GlobalSettings.Camera.MaxYRotationAngle;

    [HideInInspector]
    public bool RotateCamera { get; set; } = false;

    [HideInInspector]
    public bool AllowCameraRotation { get; set; } = false;

    private Vector2 currentRotation;

    void Start()
    {
        currentRotation = transform.eulerAngles
            .GetVector2InspectorAnglesFromEulerAngles()
            .SwapXAndY();
    }

    void Update()
    {
        if (RotateCamera && AllowCameraRotation)
        {
            Rotate();
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void MoveCamera(float horizontalInput, float verticalInput)
    {
        Vector3 movementVector = Vector3.ClampMagnitude(
            transform.TransformDirection(new Vector3(horizontalInput, 0, verticalInput)),
            MovementSpeed);

        movementVector.y = 0;
        transform.position += MovementSpeed * Time.deltaTime * movementVector;
    }

    public void ZoomCamera(float scrollWheelInput)
    {
        transform.position += scrollWheelInput * ZoomSpeed * transform.forward;
    }

    private void Rotate()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentRotation.x += Input.GetAxis("Mouse X") * RotationSpeed;
        currentRotation.y -= Input.GetAxis("Mouse Y") * RotationSpeed;
        currentRotation.x = Mathf.Repeat(currentRotation.x, 360);
        currentRotation.y = Mathf.Clamp(currentRotation.y, -MaxYRotationAngle, MaxYRotationAngle);

        transform.rotation = Quaternion.Euler(currentRotation.y, currentRotation.x, 0);
    }
}
