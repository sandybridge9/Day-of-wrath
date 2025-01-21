using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitCommandController : MonoBehaviour
{
    private SelectionController selectionController;
    private Pathfinding pathfinding;

    private float unitSpacing = 0.5f;
    private float formationSpreadMultiplier = 1.5f;
    private int framesBetweenUnitProcessing = 20;

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

        var targetNode = pathfinding.grid.NodeFromWorldPoint(targetPosition);
        if (!targetNode.walkable)
        {
            Debug.LogWarning("Target position is not walkable.");
            return;
        }

        var selectedUnits = selectionController.SelectedUnits;
        StartCoroutine(AssignPathsWithDelay(selectedUnits, targetNode));
    }

    private List<Vector3> FindSquareFormationPositions(Vector3 center, int count)
    {
        var positions = new List<Vector3>();
        var occupiedNodes = new HashSet<Node>();
        var gridSize = Mathf.CeilToInt(Mathf.Sqrt(count));

        for (int row = 0; row < gridSize; row++)
        {
            for (int col = 0; col < gridSize; col++)
            {
                if (positions.Count >= count)
                {
                    break;
                }

                var offset = new Vector3(
                    (col - gridSize / 2) * unitSpacing * formationSpreadMultiplier,
                    0,
                    (row - gridSize / 2) * unitSpacing * formationSpreadMultiplier
                );

                var potentialPosition = center + offset;
                var node = pathfinding.grid.NodeFromWorldPoint(potentialPosition);

                if (node != null && node.walkable && !occupiedNodes.Contains(node))
                {
                    positions.Add(node.worldPosition);
                    occupiedNodes.Add(node);
                }
            }
        }

        return positions;
    }

    private Node FindClosestWalkableNode(Vector3 position)
    {
        var startNode = pathfinding.grid.NodeFromWorldPoint(position);

        if (startNode.walkable)
        {
            return startNode;
        }

        var nodesToCheck = new Queue<Node>();
        var visitedNodes = new HashSet<Node>();
        nodesToCheck.Enqueue(startNode);

        while (nodesToCheck.Count > 0)
        {
            var currentNode = nodesToCheck.Dequeue();

            if (currentNode.walkable)
            {
                return currentNode;
            }

            foreach (var neighbor in pathfinding.grid.GetNeighbors(currentNode))
            {
                if (!visitedNodes.Contains(neighbor))
                {
                    visitedNodes.Add(neighbor);
                    nodesToCheck.Enqueue(neighbor);
                }
            }
        }

        Debug.LogWarning($"Could not find a walkable node near {position}. Using original position.");
        return startNode;
    }

    private IEnumerator AssignPathsWithDelay(List<UnitBase> units, Node targetNode)
    {
        var formationPositions = FindSquareFormationPositions(targetNode.worldPosition, units.Count);

        for (int i = 0; i < units.Count; i++)
        {
            if (i >= formationPositions.Count)
            {
                break;
            }

            var unit = units[i];
            var startPosition = unit.transform.position;
            var startNode = FindClosestWalkableNode(startPosition);

            var path = pathfinding.FindPath(startNode.worldPosition, targetNode.worldPosition);

            if (path.Count > 0)
            {
                var formationPosition = formationPositions[i];
                path[path.Count - 1] = formationPosition;

                var unitController = unit.GetComponent<UnitController>();

                if (unitController != null)
                {
                    unitController.SetPath(path);
                }
            }

            for (int f = 0; f < framesBetweenUnitProcessing; f++)
            {
                yield return null;
            }
        }
    }
}
