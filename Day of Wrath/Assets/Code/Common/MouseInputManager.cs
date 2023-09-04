using UnityEngine;

public class MouseInputManager : MonoBehaviour
{
    private UnitMovementControllerBase unitMovementController;
    private SelectionController selectionController;
    private MainCameraController mainCameraController;
    //private Camera mainCamera;

    private float holdThreshold = 0.5f;

    private float currentLeftClickHoldTime = 0f;
    private float currentRightClickHoldTime = 0f;

    private float MouseMovementDistanceThreshold = 1f;
    private Vector2 leftClickMouseScreenPositionOnBeginClick = Vector2.zero;
    private Vector2 rightClickMouseScreenPositionOnBeginClick = Vector2.zero;

    void Start()
    {
        unitMovementController = GetComponent<UnitMovementControllerBase>();
        selectionController = GetComponent<SelectionController>();
        mainCameraController = Camera.main.GetComponent<MainCameraController>();
        //mainCamera = Camera.main;
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
            currentLeftClickHoldTime = 0;
        }

        if (Input.GetMouseButtonDown(0))
        {
            leftClickMouseScreenPositionOnBeginClick = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            currentLeftClickHoldTime += Time.deltaTime;

            if (selectionController.IsSelectionBoxActive)
            {
                selectionController.ContinueBoxSelection();
            }
            else if (CheckMouseMovementDistanceThreshold(leftClickMouseScreenPositionOnBeginClick))
            {
                selectionController.StartBoxSelection(leftClickMouseScreenPositionOnBeginClick);
            }
        }
        else
        {
            currentLeftClickHoldTime = 0;
        }
    }

    private void GetRightClickInput()
    {
        if (Input.GetMouseButtonUp(1))
        {
            if(currentRightClickHoldTime < holdThreshold)
            {
                unitMovementController.MoveUnits();
            }

            rightClickMouseScreenPositionOnBeginClick = Vector2.zero;
            currentRightClickHoldTime = 0;

            mainCameraController.RotateCamera = false;
        }

        if (Input.GetMouseButtonDown(1))
        {
            rightClickMouseScreenPositionOnBeginClick = Input.mousePosition;
        }

        if (Input.GetMouseButton(1))
        {
            currentRightClickHoldTime += Time.deltaTime;

            if(currentRightClickHoldTime > holdThreshold)
            {
                mainCameraController.RotateCamera = true;
            }
        }
        else
        {
            currentRightClickHoldTime = 0;
        }
    }

    private bool CheckMouseMovementDistanceThreshold(Vector2 startPosition)
    {
        return Vector2.Distance(startPosition, Input.mousePosition) > MouseMovementDistanceThreshold;
    }
}
