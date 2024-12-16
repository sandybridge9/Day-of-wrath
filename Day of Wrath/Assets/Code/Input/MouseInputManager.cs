using UnityEngine;
using UnityEngine.EventSystems;

public class MouseInputManager : MonoBehaviour
{
    private SelectionController selectionController;
    private BuildingPlacementController buildingController;
    private MainCameraController mainCameraController;
    private UnitCommandController unitCommandController;

    private Vector2 leftClickStartPos;
    private Vector2 rightClickStartPos;

    private LayerMask walkableLayers;
    private LayerMask unitLayer;

    private bool isDragging = false;
    private float dragThreshold = 5f;

    void Start()
    {
        selectionController = GetComponent<SelectionController>();
        buildingController = GetComponent<BuildingPlacementController>();
        mainCameraController = Camera.main.GetComponent<MainCameraController>();
        unitCommandController = GetComponent<UnitCommandController>();

        walkableLayers = LayerManager.WalkableLayers;
        unitLayer = LayerManager.UnitLayer;

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
        if (EventSystem.current.IsPointerOverGameObject() && !selectionController.IsBoxSelecting)
        {
            return;
        }

        if (buildingController.IsPlacingBuilding)
        {
            if (Input.GetMouseButtonUp(0))
            {
                buildingController.PlaceBuilding();

                ResetMousePositions();
            }

            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            leftClickStartPos = Input.mousePosition;
            isDragging = false;
        }

        if (Input.GetMouseButton(0))
        {
            if (!isDragging && Vector2.Distance(leftClickStartPos, Input.mousePosition) > dragThreshold)
            {
                isDragging = true;
            }

            if (isDragging)
            {
                if (!selectionController.IsBoxSelecting)
                {
                    selectionController.StartBoxSelection(leftClickStartPos);
                }
                else
                {
                    selectionController.UpdateBoxSelection(Input.mousePosition);
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (isDragging)
            {
                selectionController.FinishBoxSelection();
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
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

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
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out var hit1, Mathf.Infinity, unitLayer))
                {
                    Debug.Log("Unit/units is selected and got orders to attack another unit");

                    if (hit1.collider.TryGetComponent<UnitBase>(out var targetUnit))
                    {
                        unitCommandController.IssueAttackCommand(targetUnit);
                    }
                }
                else if (Physics.Raycast(ray, out var hit2, Mathf.Infinity, walkableLayers))
                {
                    Debug.Log("Unit/units is selected and got orders to move to a location");
                    unitCommandController.MoveSelectedUnits(hit2.point);
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
        isDragging = false;
    }
}
