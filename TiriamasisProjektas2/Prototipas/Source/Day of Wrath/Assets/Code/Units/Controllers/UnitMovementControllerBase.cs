using UnityEngine;

public class UnitMovementControllerBase : MonoBehaviour
{
    public LayerMask WalkableLayers;

    public SelectionController SelectionController;

    private Vector3 newPosition;

    public void MoveUnits()
    {
        if (!SelectionController.AnySelectedUnits)
        {
            return;
        }

        var mousePosition = Input.mousePosition;
        var castPointRay = Camera.main.ScreenPointToRay(mousePosition);

        if (!Physics.Raycast(castPointRay, out var hitRay, Mathf.Infinity, WalkableLayers))
        {
            return;
        }

        newPosition = hitRay.point;

        foreach (var unit in SelectionController.SelectedUnits)
        {
            unit.BeginMoving(newPosition);
        }
    }
}
