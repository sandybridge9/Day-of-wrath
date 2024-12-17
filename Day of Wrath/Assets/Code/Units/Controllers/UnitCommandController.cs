using UnityEngine;
using System.Collections.Generic;

public class UnitCommandController : MonoBehaviour
{
    private SelectionController selectionController;

    private LayerMask groundLayers;
    private LayerMask unitMovementObstacleLayers;

    [Header("Unit Movement Position Calculation")]
    public float clearanceRadius = 2f;
    public float spacingBetweenUnitsInGrid = 1.5f;
    public int maximumPositionFindAttempts = 50;
    public float positionSearchRadiusPerAttempt = 4f;

    private void Start()
    {
        groundLayers = LayerManager.GroundLayers;
        unitMovementObstacleLayers = LayerManager.UnitMovementObstacleLayers;

        selectionController = GetComponent<SelectionController>();
    }

    public void MoveSelectedUnits(Vector3 targetPosition)
    {
        var unitCount = selectionController.SelectedUnits.Count;
        var gridPositions = CalculateGridPositions(targetPosition, unitCount);

        for (int i = 0; i < unitCount; i++)
        {
            var unit = selectionController.SelectedUnits[i];
            var unitController = unit.GetComponent<UnitController>();

            if (unitController != null)
            {
                var clearPosition = FindClearPosition(gridPositions[i]);
                unitController.SetMovementTargetPosition(clearPosition);
            }
        }
    }

    public void IssueAttackCommand(SelectableObject target)
    {
        foreach (var unit in selectionController.SelectedUnits)
        {
            var unitController = unit.GetComponent<UnitController>();

            if (unitController != null)
            {
                unitController.SetAttackTarget(target);
            }
        }
    }

    public List<Vector3> CalculateGridPositions(Vector3 basePosition, int unitCount)
    {
        var positions = new List<Vector3>();
        var gridSize = Mathf.CeilToInt(Mathf.Sqrt(unitCount));

        for (int row = 0; row < gridSize; row++)
        {
            for (int col = 0; col < gridSize; col++)
            {
                if (positions.Count >= unitCount) break;

                var offset = new Vector3(col * spacingBetweenUnitsInGrid, 0, row * spacingBetweenUnitsInGrid);
                positions.Add(basePosition + offset);
            }
        }

        return positions;
    }

    private Vector3 FindClearPosition(Vector3 desiredPosition)
    {
        var position = desiredPosition;

        for (int i = 0; i < maximumPositionFindAttempts; i++)
        {
            if (Physics.OverlapSphere(position, clearanceRadius, unitMovementObstacleLayers).Length == 0)
            {
                return position;
            }

            position += Random.insideUnitSphere * positionSearchRadiusPerAttempt;
            position.y = desiredPosition.y;
        }

        return ClampToNearestValidPosition(desiredPosition);
    }

    private Vector3 ClampToNearestValidPosition(Vector3 position)
    {
        if (Physics.Raycast(position + Vector3.up * 10f, Vector3.down, out var hit, 20f, groundLayers))
        {
            return hit.point;
        }

        return position;
    }
}
