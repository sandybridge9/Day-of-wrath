using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInputController : MonoBehaviour
{
    private UnitMovementControllerBase unitMovementControllerBase;
    private SelectionController selectionController;
    private MainCameraController mainCameraController;

    private float leftClickHoldLimit = 0.3f;
    private float rightClickHoldLimit = 0.3f;

    private float currentLeftClickHoldTime = 0;
    private float currentRightClickHoldTime = 0;

    void Start()
    {
        unitMovementControllerBase = GetComponent<UnitMovementControllerBase>();
        selectionController = GetComponent<SelectionController>();
        mainCameraController = GetComponent<MainCameraController>();
    }

    void Update()
    {
        GetLeftClickInput();

        GetRightClickInput();
    }

    private void GetLeftClickInput()
    {
        if (Input.GetMouseButtonDown(0))
        {

        }
    }

    private void GetRightClickInput()
    {
        if (Input.GetMouseButtonDown(1))
        {

        }
    }
}
