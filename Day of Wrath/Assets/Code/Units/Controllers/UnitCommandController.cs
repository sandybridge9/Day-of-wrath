using UnityEngine;

public class UnitCommandController : MonoBehaviour
{
    private SelectionController selectionController;

    private void Start()
    {
        selectionController = GetComponent<SelectionController>();
    }

    public void MoveSelectedUnits(Vector3 targetPosition)
    {
        foreach (var unit in selectionController.SelectedUnits)
        {
            var movementController = unit.GetComponent<UnitMovementController>();
            if (movementController != null)
            {
                movementController.SetTargetPosition(targetPosition);
            }
        }
    }
}
