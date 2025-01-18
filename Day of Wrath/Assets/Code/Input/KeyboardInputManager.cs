using UnityEngine;

public class KeyboardInputManager : MonoBehaviour
{
    private MainCameraController mainCameraController;
    private BuildingPlacementController buildingController;
    private SelectionController selectionController;
    private BuildingActionController buildingActionController;
    private PauseMenuController pauseMenuController;

    void Start()
    {
        mainCameraController = Camera.main.GetComponent<MainCameraController>();
        buildingController = GetComponent<BuildingPlacementController>();
        selectionController = GetComponent<SelectionController>();
        buildingActionController = GetComponent<BuildingActionController>();
        pauseMenuController = FindObjectOfType<PauseMenuController>();
    }

    void Update()
    {
        if (pauseMenuController.IsGamePaused)
        {
            return;
        }

        HandleCameraMovement();
        HandleCameraRotationKey();
        HandleBuildingInputs();
        HandleMultiSelectKey();
    }

    private void HandleCameraMovement()
    {
        var horizontalInput = Input.GetAxisRaw("Horizontal");
        var verticalInput = Input.GetAxisRaw("Vertical");

        if (!Mathf.Approximately(horizontalInput, 0f) || !Mathf.Approximately(verticalInput, 0f))
        {
            mainCameraController.MoveCamera(horizontalInput, verticalInput);
        }
    }

    private void HandleBuildingInputs()
    {
        if (buildingController.IsPlacingBuilding)
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

        if (Input.GetKeyDown(KeyCode.Delete))
        {
            buildingActionController.DeleteSelectedBuilding();
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
