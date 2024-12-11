using UnityEngine;

public class MouseInputManager : MonoBehaviour
{
    private SelectionController selectionController;
    private BuildingPlacementController buildingController;
    private MainCameraController mainCameraController;
    private UnitCommandController unitCommandController;

    private Vector2 leftClickStartPos;
    private Vector2 rightClickStartPos;

    private LayerMask walkableLayers;

    void Start()
    {
        selectionController = GetComponent<SelectionController>();
        buildingController = GetComponent<BuildingPlacementController>();
        mainCameraController = Camera.main.GetComponent<MainCameraController>();
        unitCommandController = GetComponent<UnitCommandController>();

        walkableLayers = LayerManager.WalkableLayers;

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
        if (Input.GetMouseButtonUp(0))
        {
            if (buildingController.IsPlacingBuilding)
            {
                buildingController.PlaceBuilding();
            }
            else
            {
                selectionController.PointSelect();
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
            if (mainCameraController.AllowCameraRotation
                && rightClickStartPos.HasPassedDistanceThreshold(Input.mousePosition, GlobalSettings.MouseInput.CameraRotationDistanceThreshold))
            {
                mainCameraController.RotateCamera = true;
            }
        }

        if (Input.GetMouseButtonUp(1))
        {
            mainCameraController.RotateCamera = false;
            rightClickStartPos = Vector2.zero;

            if (mainCameraController.AllowCameraRotation)
            {
                return;
            }

            if (buildingController != null && buildingController.IsPlacingBuilding)
            {
                buildingController.CancelPlacement();
            }
            else if (selectionController.AnySelectedUnits)
            {
                Debug.Log("boss we got selected units, need to move em.");

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, walkableLayers))
                {
                    unitCommandController.MoveSelectedUnits(hit.point);
                }
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
