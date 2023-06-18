using UnityEngine;
using Cursor = UnityEngine.Cursor;

public class MainCameraController : MonoBehaviour
{
    public float CameraMovementSpeed = 10.0f;
    public float CameraZoomSpeed = 10.0f;
    public float CameraRotationSpeed = 1.0f;
    public float maxYRotationAngle = 80f;

    private Vector2 currentRotation;

    void Start()
    {
        currentRotation = transform.eulerAngles
            .GetVector2InspectorAnglesFromEulerAngles()
            .CrossXAndY();
    }

    void Update()
    {
        Move();

        Zoom();

        Rotate();
    }

    private void Move()
    {
        var movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        transform.position += CameraMovementSpeed * Time.deltaTime * movement;
    }

    private void Zoom()
    {
        var scrollWheelInput = Input.GetAxis("Mouse ScrollWheel");

        if (scrollWheelInput != 0)
        {
            transform.position += scrollWheelInput * CameraZoomSpeed * transform.forward;
        }
    }

    private void Rotate()
    {
        if (Input.GetMouseButton(1))
        {
            Cursor.lockState = CursorLockMode.Locked;

            currentRotation.x += Input.GetAxis("Mouse X") * CameraRotationSpeed;
            currentRotation.y -= Input.GetAxis("Mouse Y") * CameraRotationSpeed;
            currentRotation.x = Mathf.Repeat(currentRotation.x, 360);
            currentRotation.y = Mathf.Clamp(currentRotation.y, -maxYRotationAngle, maxYRotationAngle);

            transform.rotation = Quaternion.Euler(currentRotation.y, currentRotation.x, 0);
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
