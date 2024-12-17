using UnityEngine;

public class MouseInputManager : MonoBehaviour
{
    public const float LeftClickMouseMovementDistanceThreshold = 1f;
    public const float RightClickMouseMovementDistanceThreshold = 40f;

    private UnitMovementControllerBase unitMovementController;
    private SelectionController selectionController;
    private MainCameraController mainCameraController;

    private Vector2 leftClickMouseScreenPositionOnBeginClick;
    private Vector2 rightClickMouseScreenPositionOnBeginClick;

    void Start()
    {
        unitMovementController = GetComponent<UnitMovementControllerBase>();
        selectionController = GetComponent<SelectionController>();
        mainCameraController = Camera.main.GetComponent<MainCameraController>();

        leftClickMouseScreenPositionOnBeginClick = Vector2.zero;
        rightClickMouseScreenPositionOnBeginClick = Vector2.zero;
    }

    void Update()
    {
        GetLeftClickInput();

        GetRightClickInput();
    }

    private void GetLeftClickInput()
    {
        if(Input.GetMouseButtonUp(0))
        {
            if (selectionController.IsSelectionBoxActive)
            {
                selectionController.FinishBoxSelection();
            }
            else
            {
                selectionController.PointSelect();
            }

            leftClickMouseScreenPositionOnBeginClick = Vector2.zero;
        }

        if (Input.GetMouseButtonDown(0))
        {
            leftClickMouseScreenPositionOnBeginClick = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            if (selectionController.IsSelectionBoxActive)
            {
                selectionController.ContinueBoxSelection();
            }
            else if (leftClickMouseScreenPositionOnBeginClick
                .HasPassedDistanceThreshold(Input.mousePosition, LeftClickMouseMovementDistanceThreshold))
            {
                selectionController.StartBoxSelection(leftClickMouseScreenPositionOnBeginClick);
            }
        }
    }

    private void GetRightClickInput()
    {
        if (Input.GetMouseButtonUp(1))
        {
            rightClickMouseScreenPositionOnBeginClick = Vector2.zero;

            if (mainCameraController.RotateCamera == true)
            {
                mainCameraController.RotateCamera = false;

                return;
            }

            unitMovementController.MoveUnits();
        }

        if (Input.GetMouseButtonDown(1))
        {
            rightClickMouseScreenPositionOnBeginClick = Input.mousePosition;
        }

        if (Input.GetMouseButton(1))
        {
            if(rightClickMouseScreenPositionOnBeginClick
                .HasPassedDistanceThreshold(Input.mousePosition, RightClickMouseMovementDistanceThreshold))
            {
                mainCameraController.RotateCamera = true;
            }
        }
    }
}
