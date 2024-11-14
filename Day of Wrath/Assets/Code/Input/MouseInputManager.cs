using UnityEngine;

public class MouseInputManager : MonoBehaviour
{
    private SelectionController selectionController;
    private BuildingController buildingController;
    private MainCameraController mainCameraController;

    private Vector2 leftClickStartPos;
    private Vector2 rightClickStartPos;

    void Start()
    {
        selectionController = GetComponent<SelectionController>();
        buildingController = GetComponent<BuildingController>();
        mainCameraController = Camera.main.GetComponent<MainCameraController>();

        ResetMousePositions();
    }

    void Update()
    {
        HandleLeftClick();
        HandleRightClick();
        HandleScrollWheel();
    }

    private void HandleLeftClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            leftClickStartPos = Input.mousePosition;

            // Uncomment the following line to enable box selection starting
            // selectionController.StartBoxSelection(leftClickStartPos);
        }

        if (Input.GetMouseButton(0))
        {
            // Uncomment the following line to enable box selection continuation
            // selectionController.ContinueBoxSelection();
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (!selectionController.IsSelectionBoxActive)
            {
                // Check if we're placing a building; if so, place it
                if (buildingController != null && buildingController.IsPlacingBuilding())
                {
                    buildingController.PlaceBuilding();
                }
                else
                {
                    // Otherwise, perform point selection
                    selectionController.PointSelect();
                }
            }
            else
            {
                // Uncomment the following line to enable box selection finishing
                // selectionController.FinishBoxSelection();
            }

            ResetMousePositions();
        }
    }

    private void HandleRightClick()
    {
        if (Input.GetMouseButtonDown(1))
        {
            rightClickStartPos = Input.mousePosition;
        }

        if (Input.GetMouseButton(1))
        {
            if (rightClickStartPos.HasPassedDistanceThreshold(Input.mousePosition, GlobalSettings.MouseInput.CameraRotationDistanceThreshold))
            {
                mainCameraController.RotateCamera = true;
            }
        }

        if (Input.GetMouseButtonUp(1))
        {
            mainCameraController.RotateCamera = false;
            rightClickStartPos = Vector2.zero;

            if (buildingController != null && buildingController.IsPlacingBuilding())
            {
                buildingController.CancelPlacement();
            }
            else if (selectionController.AnySelectedUnits)
            {
                selectionController.ClearSelection();
            }
        }
    }

    private void HandleScrollWheel()
    {
        float scrollWheelInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollWheelInput != 0)
        {
            mainCameraController.ZoomCamera(scrollWheelInput);
        }
    }

    private void ResetMousePositions()
    {
        leftClickStartPos = Vector2.zero;
        rightClickStartPos = Vector2.zero;
    }
}
