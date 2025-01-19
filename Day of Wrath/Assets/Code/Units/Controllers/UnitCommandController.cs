using UnityEngine;
using System.Collections.Generic;

public class UnitCommandController : MonoBehaviour
{
    private SelectionController selectionController;
    private Pathfinding pathfinding;

    [Header("Formation Settings")]
    public float unitSpacing = 1f; // Spacing between units in the formation

    private void Start()
    {
        selectionController = GetComponent<SelectionController>();
        pathfinding = FindObjectOfType<Pathfinding>();
    }

    public void MoveSelectedUnits(Vector3 targetPosition)
    {
        if (selectionController.SelectedUnits == null || selectionController.SelectedUnits.Count == 0)
        {
            Debug.LogWarning("No units selected to move.");
            return;
        }

        int unitCount = selectionController.SelectedUnits.Count;

        // Get valid target node
        Node targetNode = pathfinding.grid.NodeFromWorldPoint(targetPosition);
        if (!targetNode.walkable)
        {
            Debug.LogWarning("Target position is not walkable.");
            return;
        }

        // Group units by their starting nodes to minimize pathfinding calls
        Dictionary<Node, List<GameObject>> unitsByNode = GroupUnitsByStartNode();

        // Calculate formation positions
        List<Vector3> formationPositions = CalculateFormationPositions(targetNode.worldPosition, unitCount);

        // Assign paths
        int formationIndex = 0;
        foreach (var group in unitsByNode)
        {
            Node startNode = group.Key;
            List<GameObject> units = group.Value;

            // Calculate a shared path for the group
            List<Vector3> sharedPath = pathfinding.FindPath(startNode.worldPosition, targetNode.worldPosition);

            foreach (var unit in units)
            {
                var unitController = unit.GetComponent<UnitController>();
                if (unitController == null) continue;

                // Ensure the unit ends at a valid formation position
                Vector3 targetFormationPosition = GetValidFormationPosition(formationPositions, formationIndex++, targetNode);

                // Adjust the path for the individual unit
                List<Vector3> adjustedPath = AdjustPathForUnit(sharedPath, unit.transform.position, targetFormationPosition);
                unitController.SetPath(adjustedPath);
            }
        }
    }

    private Dictionary<Node, List<GameObject>> GroupUnitsByStartNode()
    {
        var groups = new Dictionary<Node, List<GameObject>>();

        foreach (var unit in selectionController.SelectedUnits)
        {
            var startNode = pathfinding.grid.NodeFromWorldPoint(unit.transform.position);

            if (!groups.ContainsKey(startNode))
            {
                groups[startNode] = new List<GameObject>();
            }

            groups[startNode].Add(unit.gameObject);
        }

        return groups;
    }

    private List<Vector3> CalculateFormationPositions(Vector3 center, int unitCount)
    {
        List<Vector3> positions = new List<Vector3>();
        int gridSize = Mathf.CeilToInt(Mathf.Sqrt(unitCount));

        for (int row = 0; row < gridSize; row++)
        {
            for (int col = 0; col < gridSize; col++)
            {
                if (positions.Count >= unitCount) break;

                Vector3 offset = new Vector3((col - gridSize / 2) * unitSpacing, 0, (row - gridSize / 2) * unitSpacing);
                positions.Add(center + offset);
            }
        }

        return positions;
    }

    private Vector3 GetValidFormationPosition(List<Vector3> formationPositions, int index, Node targetNode)
    {
        Vector3 position = formationPositions[index];
        Node node = pathfinding.grid.NodeFromWorldPoint(position);

        // Ensure the position is walkable
        return node.walkable ? node.worldPosition : targetNode.worldPosition;
    }

    private List<Vector3> AdjustPathForUnit(List<Vector3> sharedPath, Vector3 unitStartPosition, Vector3 targetFormationPosition)
    {
        List<Vector3> adjustedPath = new List<Vector3>();

        foreach (var point in sharedPath)
        {
            Vector3 direction = (point - sharedPath[0]).normalized;
            float distance = Vector3.Distance(sharedPath[0], point);
            Vector3 adjustedPoint = unitStartPosition + direction * distance;

            adjustedPath.Add(adjustedPoint);
        }

        // Ensure the final position is the formation position
        adjustedPath[adjustedPath.Count - 1] = targetFormationPosition;
        return adjustedPath;
    }
}
