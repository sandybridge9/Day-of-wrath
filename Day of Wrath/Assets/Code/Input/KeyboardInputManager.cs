using UnityEngine;

public class KeyboardInputManager : MonoBehaviour
{
    private MainCameraController mainCameraController;
    private BuildingPlacementController buildingController;
    private SelectionController selectionController;

    void Start()
    {
        mainCameraController = Camera.main.GetComponent<MainCameraController>();
        buildingController = GetComponent<BuildingPlacementController>();
        selectionController = GetComponent<SelectionController>();
    }

    void Update()
    {
        HandleCameraMovement();
        HandleCameraRotationKey();
        HandleBuildingInputs();
        HandleMultiSelectKey();
    }

    private void HandleCameraMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        if (horizontalInput != 0 || verticalInput != 0)
        {
            mainCameraController.MoveCamera(horizontalInput, verticalInput);
        }
    }

    private void HandleBuildingInputs()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            buildingController.StartBuildingPlacement();
        }

        if (buildingController.IsPlacingBuilding())
        {
            if (Input.GetKey(KeyCode.Q))
            {
                buildingController.RotateBuilding(-1);
            }
            else if (Input.GetKey(KeyCode.E))
            {
                buildingController.RotateBuilding(1);
            }
        }
    }

    private void HandleMultiSelectKey()
    {
        selectionController.IsMultiSelectEnabled = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    }

    private void HandleCameraRotationKey()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            mainCameraController.AllowCameraRotation = true;
        }

        if (Input.GetKeyUp(KeyCode.R))
        {
            mainCameraController.AllowCameraRotation = false;
        }
    }
}
