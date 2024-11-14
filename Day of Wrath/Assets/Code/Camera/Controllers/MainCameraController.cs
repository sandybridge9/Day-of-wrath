using UnityEngine;

public class MainCameraController : MonoBehaviour
{
    public float CameraMovementSpeed = 10.0f;
    public float CameraZoomSpeed = 10.0f;
    public float CameraRotationSpeed = 1.0f;
    public float maxYRotationAngle = 80f;

    public bool RotateCamera { get; set; } = false;
    private Vector2 currentRotation;

    void Start()
    {
        currentRotation = transform.eulerAngles
            .GetVector2InspectorAnglesFromEulerAngles()
            .SwapXAndY();
    }

    void Update()
    {
        if (RotateCamera)
        {
            Rotate();
        }
        else
        {
            // Reset the cursor when not rotating
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void MoveCamera(float horizontalInput, float verticalInput)
    {
        Vector3 movementVector = Vector3.ClampMagnitude(
            transform.TransformDirection(new Vector3(horizontalInput, 0, verticalInput)),
            CameraMovementSpeed);

        movementVector.y = 0;
        transform.position += CameraMovementSpeed * Time.deltaTime * movementVector;
    }

    public void ZoomCamera(float scrollWheelInput)
    {
        transform.position += scrollWheelInput * CameraZoomSpeed * transform.forward;
    }

    private void Rotate()
    {
        // Lock cursor and hide it when rotating
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentRotation.x += Input.GetAxis("Mouse X") * CameraRotationSpeed;
        currentRotation.y -= Input.GetAxis("Mouse Y") * CameraRotationSpeed;
        currentRotation.x = Mathf.Repeat(currentRotation.x, 360);
        currentRotation.y = Mathf.Clamp(currentRotation.y, -maxYRotationAngle, maxYRotationAngle);

        transform.rotation = Quaternion.Euler(currentRotation.y, currentRotation.x, 0);
    }
}
