using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UnitCommandController : MonoBehaviour
{
    private SelectionController selectionController;
    private Pathfinding pathfinding;

    private float unitSpacing = 1f;
    private float formationSpreadMultiplier = 1.5f;
    private int framesBetweenUnitProcessing = 20;

    private void Start()
    {
        selectionController = GetComponent<SelectionController>();
        pathfinding = FindObjectOfType<Pathfinding>();
    }

    // ------------ MOVEMENT LOGIC ------------------
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

            targetNode = FindClosestWalkableNode(targetPosition);
        }

        var selectedUnits = selectionController.SelectedUnits;
        StartCoroutine(AssignPathsWithDelay(selectedUnits, targetNode));
    }

    private List<Vector3> FindSquareFormationPositions(Vector3 center, int count)
    {
        var positions = new List<Vector3>();
        var occupiedNodes = new HashSet<Node>();
        var gridSize = Mathf.CeilToInt(Mathf.Sqrt(count));

        for (var row = 0; row < gridSize; row++)
        {
            for (var col = 0; col < gridSize; col++)
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

            foreach (Node neighbor in pathfinding.grid.GetNeighbors(currentNode))
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

        for (var i = 0; i < units.Count; i++)
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
                path[path.Count - 1] = formationPosition; // Snap final path node to square formation position

                var unitController = unit.GetComponent<UnitController>();

                if (unitController != null)
                {
                    unitController.SetPath(path);
                }
            }

            // Wait for the specified number of frames before processing the next unit
            for (int f = 0; f < framesBetweenUnitProcessing; f++)
            {
                yield return null;
            }
        }
    }

    // ------------ ATTACK LOGIC ---------------------
    public void AttackTarget(SelectableObject target)
    {
        if (selectionController.SelectedUnits == null || selectionController.SelectedUnits.Count == 0)
        {
            Debug.LogWarning("No units selected to attack.");

            return;
        }

        if (target is BuildingBase building)
        {
            StartCoroutine(AssignBuildingAttackPaths(selectionController.SelectedUnits.Select(x => x.gameObject).ToList(), building));
        }
        else if (target is UnitBase enemyUnit)
        {
            StartCoroutine(AssignDynamicUnitAttackPaths(selectionController.SelectedUnits.Select(x => x.gameObject).ToList(), enemyUnit));
        }
    }

    private IEnumerator AssignBuildingAttackPaths(List<GameObject> units, BuildingBase building)
    {
        var boxCollider = building.transform.Find("PlacementTrigger").GetComponent<BoxCollider>();

        if (boxCollider == null)
        {
            Debug.LogError($"Building {building.name} is missing a BoxCollider.");
            yield break;
        }

        var boxCenter = boxCollider.transform.TransformPoint(boxCollider.center);
        var boxSize = boxCollider.size;
        var boxRotation = boxCollider.transform.rotation;

        var occupiedNodes = new HashSet<Node>();

        foreach (GameObject unit in units)
        {
            var unitBase = unit.GetComponent<UnitBase>();

            if (unitBase == null)
            {
                continue;
            }

            var attackRange = unitBase.AttackRange;
            Vector3 attackPosition;

            if (attackRange < 1.5f)
            {
                attackPosition = FindClosestWalkableNodeNearBuilding(
                    unit.transform.position,
                    boxCenter,
                    boxSize,
                    boxRotation,
                    occupiedNodes);
            }
            else
            {
                attackPosition = FindPositionAroundBox(boxCenter, boxSize, boxRotation, attackRange, occupiedNodes);
            }

            var startNode = pathfinding.grid.NodeFromWorldPoint(unit.transform.position);
            var targetNode = pathfinding.grid.NodeFromWorldPoint(attackPosition);

            if (startNode != null && targetNode != null && targetNode.walkable)
            {
                var path = pathfinding.FindPath(startNode.worldPosition, targetNode.worldPosition);

                var unitController = unit.GetComponent<UnitController>();

                if (unitController != null)
                {
                    unitController.SetAttackTarget(building, path, attackPosition);
                }
            }

            if (targetNode != null)
            {
                occupiedNodes.Add(targetNode);
            }

            for (var f = 0; f < framesBetweenUnitProcessing; f++)
            {
                yield return null;
            }
        }
    }

    private Vector3 FindClosestWalkableNodeNearBuilding(Vector3 unitPosition, Vector3 boxCenter, Vector3 boxSize, Quaternion boxRotation, HashSet<Node> occupiedNodes)
    {
        var halfX = boxSize.x / 2;
        var halfZ = boxSize.z / 2;

        var closestDistance = float.MaxValue;
        var closestPosition = unitPosition;

        for (var x = -halfX; x <= halfX; x += unitSpacing)
        {
            for (var z = -halfZ; z <= halfZ; z += unitSpacing)
            {
                var localPosition = new Vector3(x, 0, z);
                var worldPosition = boxCenter + boxRotation * localPosition;

                var node = pathfinding.grid.NodeFromWorldPoint(worldPosition);

                if (node != null && node.walkable && !occupiedNodes.Contains(node))
                {
                    var distance = Vector3.Distance(unitPosition, node.worldPosition);

                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestPosition = node.worldPosition;
                    }
                }
            }
        }

        if (closestPosition == unitPosition)
        {
            Debug.LogWarning($"No walkable nodes found near the building. Returning unit's current position.");
        }

        return closestPosition;
    }

    private Vector3 FindPositionAroundBox(Vector3 center, Vector3 size, Quaternion rotation, float attackRange, HashSet<Node> occupiedNodes)
    {
        var halfX = size.x / 2 + attackRange;
        var halfZ = size.z / 2 + attackRange;

        for (var x = -halfX; x <= halfX; x += unitSpacing)
        {
            for (var z = -halfZ; z <= halfZ; z += unitSpacing)
            {
                if (Mathf.Abs(x) < size.x / 2 && Mathf.Abs(z) < size.z / 2)
                {
                    continue;
                }

                var localPosition = new Vector3(x, 0, z);
                var worldPosition = center + rotation * localPosition;

                var node = pathfinding.grid.NodeFromWorldPoint(worldPosition);

                if (node != null && node.walkable && !occupiedNodes.Contains(node))
                {
                    occupiedNodes.Add(node);

                    return node.worldPosition;
                }
            }
        }

        Debug.LogWarning("Could not find a valid position around the building.");
        return center;
    }

    private IEnumerator AssignDynamicUnitAttackPaths(List<GameObject> units, UnitBase enemyUnit)
    {
        while (enemyUnit != null && enemyUnit.Health > 0)
        {
            var targetPosition = enemyUnit.transform.position;

            foreach (GameObject unit in units)
            {
                var startNode = pathfinding.grid.NodeFromWorldPoint(unit.transform.position);
                var targetNode = pathfinding.grid.NodeFromWorldPoint(targetPosition);

                if (startNode != null && targetNode != null && targetNode.walkable)
                {
                    var path = pathfinding.FindPath(startNode.worldPosition, targetNode.worldPosition);

                    var unitController = unit.GetComponent<UnitController>();

                    if (unitController != null)
                    {
                        unitController.SetAttackTarget(enemyUnit, path, targetPosition);
                    }
                }

                for (int f = 0; f < framesBetweenUnitProcessing; f++)
                {
                    yield return null;
                }
            }

            yield return new WaitForSeconds(0.1f);
        }
    }
}
