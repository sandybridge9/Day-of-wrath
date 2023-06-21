using UnityEngine;

public class UnitMovementControllerBase : MonoBehaviour
{
    public LayerMask WalkableLayers;

    public SelectionController SelectionController;

    private bool newPositionIsActive = false;
    private Vector3 newPosition;

    void Update()
    {
        TryGetNewPosition();

        OrderSelectedUnitsToMove();
    }

    private void TryGetNewPosition()
    {
        if (Input.GetMouseButtonDown(0)
            && SelectionController.AnySelectedUnits)
        {
            var mousePosition = Input.mousePosition;
            var castPointRay = Camera.main.ScreenPointToRay(mousePosition);

            if (Physics.Raycast(castPointRay, out var hitRay, Mathf.Infinity, WalkableLayers))
            {
                Debug.Log("Setting new movement position");
                newPosition = hitRay.point;
                newPositionIsActive = true;
            }
        }
    }

    private void OrderSelectedUnitsToMove()
    {
        if(!newPositionIsActive)
        {
            return;
        }

        foreach(var unit in SelectionController.SelectedUnits)
        {
            Debug.Log("Ordering unit to move");
            unit.BeginMoving(newPosition);
        }

        newPositionIsActive = false;
    }
}
