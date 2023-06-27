using UnityEngine;

public class MouseInputManager : MonoBehaviour
{
    private UnitMovementControllerBase unitMovementControllerBase;
    private SelectionController selectionController;
    private MainCameraController mainCameraController;
    private Camera mainCamera;

    private float holdThreshold = 0.5f;

    private float currentLeftClickHoldTime = 0;
    private float currentRightClickHoldTime = 0;

    void Start()
    {
        unitMovementControllerBase = GetComponent<UnitMovementControllerBase>();
        selectionController = GetComponent<SelectionController>();
        mainCameraController = GetComponent<MainCameraController>();
        mainCamera = Camera.main;
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
            if(currentLeftClickHoldTime < holdThreshold)
            {
                ShootAndProcessRayCast();
            }

            currentLeftClickHoldTime = 0;
        }

        if (Input.GetMouseButton(0))
        {
            currentLeftClickHoldTime += Time.deltaTime;
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
                selectionController.ClearSelection();
            }

            currentRightClickHoldTime = 0;

            mainCameraController.RotateCamera = false;
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

    private void ShootAndProcessRayCast()
    {
        var mousePosition = Input.mousePosition;
        var castPointRay = mainCamera.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(castPointRay, out var raycastHit, Mathf.Infinity))
        {
            raycastHit.transform.gameObject.layer
        }
    }
}
