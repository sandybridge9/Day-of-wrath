using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    private UnitBase thisUnit;
    private float movementSpeed;

    private List<Vector3> currentPath;
    private int currentPathIndex = 0;

    private Pathfinding pathfinding;

    [Header("Combat Settings")]
    public float attackCooldown = 1f; // Time between attacks
    private float lastAttackTime;

    private SelectableObject currentTarget;
    private Vector3 designatedAttackPosition;

    private void Start()
    {
        thisUnit = GetComponent<UnitBase>();
        movementSpeed = thisUnit.MovementSpeed;
        pathfinding = FindObjectOfType<Pathfinding>();
    }

    private void Update()
    {
        if (currentTarget != null)
        {
            if (IsTargetDead())
            {
                StopAndClearTarget();

                return;
            }

            if (currentTarget is BuildingBase && IsAtDesignatedPosition())
            {
                AttackTarget();
            }
            else if (IsWithinAttackRange())
            {
                AttackTarget();
            }
            else if (currentPath != null && currentPathIndex < currentPath.Count)
            {
                MoveAlongPath();
            }
        }
        else if (currentPath != null && currentPathIndex < currentPath.Count)
        {
            var targetPosition = currentPath[currentPathIndex];

            if (!IsTargetNodeWalkable(targetPosition))
            {
                RecalculatePath();

                return;
            }

            MoveTowards(targetPosition);

            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                currentPathIndex++;
            }
        }
    }

    public void SetPath(List<Vector3> path)
    {
        CancelCurrentAction();

        currentPath = path;
        currentPathIndex = 0;
    }

    private void MoveTowards(Vector3 targetPosition)
    {
        var direction = (targetPosition - transform.position).normalized;

        if (direction.magnitude > 0.1f)
        {
            var targetRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        transform.position += movementSpeed * Time.deltaTime * direction;
    }

    private bool IsTargetNodeWalkable(Vector3 position)
    {
        var node = pathfinding.grid.NodeFromWorldPoint(position);

        return node != null && node.walkable;
    }

    private void RecalculatePath()
    {
        if (currentPath == null || currentPathIndex >= currentPath.Count)
        {
            return;
        }

        var currentTarget = currentPath[currentPath.Count - 1];
        var currentNode = pathfinding.grid.NodeFromWorldPoint(transform.position);

        if (currentNode != null)
        {
            var newPath = pathfinding.FindPath(currentNode.worldPosition, currentTarget);

            if (newPath.Count > 0)
            {
                SetPath(newPath);
            }
            else
            {
                currentPath.Clear();
                currentPathIndex = 0;

                Debug.LogWarning($"Unit {gameObject.name} could not find a valid path to {currentTarget}, stopping.");
            }
        }
    }

    public void SetAttackTarget(SelectableObject target, List<Vector3> path, Vector3 attackPosition)
    {
        CancelCurrentAction();

        currentTarget = target;
        designatedAttackPosition = attackPosition;

        currentPath = path;
        currentPathIndex = 0;

        Debug.Log(designatedAttackPosition);
    }

    private void MoveAlongPath()
    {
        var targetPosition = currentPath[currentPathIndex];
        var direction = (targetPosition - transform.position).normalized;

        if (direction.magnitude > 0.1f)
        {
            var targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        transform.position += direction * movementSpeed * Time.deltaTime;

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            currentPathIndex++;
        }
    }

    private void AttackTarget()
    {
        if (currentTarget == null || Time.time - lastAttackTime < attackCooldown)
        {
            return;
        }

        lastAttackTime = Time.time;

        if (currentTarget is UnitBase targetUnit)
        {
            targetUnit.TakeDamage(thisUnit.Damage);

            if (targetUnit.Health <= 0)
            {
                StopAndClearTarget();
            }
        }
        else if (currentTarget is BuildingBase targetBuilding)
        {
            targetBuilding.TakeDamage(thisUnit.Damage);

            if (targetBuilding.Health <= 0)
            {
                StopAndClearTarget();
            } 
        }
    }

    private bool IsAtDesignatedPosition()
    {
        return Vector3.Distance(transform.position, designatedAttackPosition) <= 0.1f;
    }

    private bool IsWithinAttackRange()
    {
        if (currentTarget is BuildingBase)
        {
            return Vector3.Distance(transform.position, designatedAttackPosition) <= 0.1f;
        }

        return Vector3.Distance(transform.position, currentTarget.transform.position) <= thisUnit.AttackRange;
    }

    private bool IsTargetDead()
    {
        if (currentTarget is UnitBase unit && unit.Health <= 0)
        {
            return true;
        }
        if (currentTarget is BuildingBase building && building.Health <= 0)
        {
            return true;
        }

        return false;
    }

    private void StopAndClearTarget()
    {
        currentTarget = null;

        currentPath = null;
        currentPathIndex = 0;
    }

    private void CancelCurrentAction()
    {
        currentTarget = null;

        currentPath = null;
        currentPathIndex = 0;
    }
}

//private Pathfinding pathfinding;
//private List<Vector3> currentPath;
//private int currentPathIndex = 0;

//public float movementSpeed = 5f;
//public float avoidanceRadius = 1.5f;

//private void Start()
//{
//    pathfinding = FindObjectOfType<Pathfinding>();
//}
//public void SetMovementTarget(Vector3 target)
//{
//    // Calculate the path to the target position using the grid
//    currentPath = pathfinding.FindPath(transform.position, target);
//    currentPathIndex = 0;
//}
//private void Update()
//{
//    if (currentPath == null || currentPathIndex >= currentPath.Count) return;

//    Vector3 targetPosition = currentPath[currentPathIndex];
//    MoveTowards(targetPosition);

//    if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
//    {
//        currentPathIndex++;
//    }
//}

//private void MoveTowards(Vector3 targetPosition)
//{
//    Vector3 direction = (targetPosition - transform.position).normalized;
//    transform.position += direction * movementSpeed * Time.deltaTime;
//}







//public void SetMovementTarget(Vector3 target)
//{
//    currentPath = pathfinding.FindPath(transform.position, target);
//    currentPathIndex = 0;
//}

//private void Update()
//{
//    if (currentPath == null || currentPathIndex >= currentPath.Count) return;

//    Vector3 targetPosition = currentPath[currentPathIndex];
//    transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);

//    if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
//    {
//        currentPathIndex++;
//    }

//    if (Physics.CheckSphere(transform.position, 0.5f, LayerManager.BuildingLayer))
//    {
//        RecalculatePath();
//    }
//}

//public List<Vector3> GetCurrentPath()
//{
//    return currentPath ?? new List<Vector3>();
//}

//public void SetPath(List<Vector3> path)
//{
//    currentPath = path;
//    currentPathIndex = 0;
//}

//private void RecalculatePath()
//{
//    if (currentPathIndex < currentPath.Count - 1)
//    {
//        SetMovementTarget(currentPath[currentPath.Count - 1]);
//    }
//}
//private UnitBase thisUnit;

//[Header("Movement control settings")]
//public float avoidanceRadius = 3f;
//public float steeringStrength = 100f;
//private float movementHeightOffset = 0f;
//private Vector3 movementTargetPosition;
//private LayerMask unitMovementObstacleLayers;

//private SelectableObject targetToAttack;

//private bool isTargetBuilding;
//private Vector3 buildingAttackPosition;

//private bool isMoving = false;
//private bool isAttacking = false;
//private bool isChasing = false;

//private void Start()
//{
//    thisUnit = GetComponent<UnitBase>();
//    unitMovementObstacleLayers = LayerManager.UnitMovementObstacleLayers;

//    Collider unitCollider = GetComponent<Collider>();

//    if (unitCollider != null)
//    {
//        movementHeightOffset = unitCollider.bounds.extents.y;
//    }
//}

//private void Update()
//{
//    if (isMoving)
//    {
//        MoveToTarget();
//    }

//    if (isAttacking)
//    {
//        AttackTarget();
//    }
//}

//public void SetMovementTargetPosition(Vector3 position)
//{
//    if (Physics.Raycast(position + Vector3.up * 5f, Vector3.down, out var hit, 10f, LayerManager.GroundLayers))
//    {
//        position.y = hit.point.y;
//    }

//    isChasing = false;
//    isAttacking = false;
//    targetToAttack = null;
//    isTargetBuilding = false;
//    buildingAttackPosition = Vector3.zero;

//    movementTargetPosition = position;
//    isMoving = true;
//}

//public void SetAttackTarget(SelectableObject target)
//{
//    if (!target.IsFromDifferentTeam(thisUnit.Team))
//    {
//        return;
//    }

//    targetToAttack = target;

//    isTargetBuilding = target is BuildingBase;
//    isChasing = !isTargetBuilding;

//    if (isTargetBuilding)
//    {
//        var buildingCollider = target.GetComponentInChildren<Collider>();

//        if (buildingCollider != null)
//        {
//            var directionToBuilding = (buildingCollider.bounds.center - transform.position).normalized;

//            if (Physics.Raycast(transform.position, directionToBuilding, out var hit, Mathf.Infinity, LayerManager.UnitMovementObstacleLayers))
//            {
//                buildingAttackPosition = hit.point - directionToBuilding * thisUnit.AttackRange;
//            }
//            else
//            {
//                buildingAttackPosition = buildingCollider.bounds.center;
//            }
//        }
//        else
//        {
//            buildingAttackPosition = target.transform.position;
//        }
//    }

//    isMoving = true;
//    isAttacking = false;
//}

//private void MoveToTarget()
//{
//    var targetPosition = isChasing && targetToAttack != null
//        ? targetToAttack.transform.position
//        : isTargetBuilding
//            ? buildingAttackPosition
//            : movementTargetPosition;

//    var direction = (targetPosition - transform.position).normalized;
//    direction = ApplyObstacleAvoidance(direction);

//    var movePosition = transform.position + direction * thisUnit.MovementSpeed * Time.deltaTime;
//    transform.position = movePosition;

//    var distanceThreshold = isTargetBuilding
//        ? thisUnit.AttackRange
//        : 0.1f;

//    if (Vector3.Distance(transform.position, targetPosition) < distanceThreshold)
//    {
//        isMoving = false;

//        if (isChasing || isTargetBuilding)
//        {
//            isAttacking = true;
//        }
//    }
//}

//private Vector3 ApplyObstacleAvoidance(Vector3 moveDirection)
//{
//    if (Physics.Raycast(transform.position, moveDirection, out var hit, avoidanceRadius, unitMovementObstacleLayers))
//    {
//        var avoidanceDirection = Vector3.Cross(hit.normal, Vector3.up).normalized;
//        moveDirection += avoidanceDirection * steeringStrength;
//    }

//    return moveDirection.normalized;
//}

//private void AttackTarget()
//{
//    if (targetToAttack == null)
//    {
//        isAttacking = false;

//        return;
//    }

//    if (!targetToAttack.IsFromDifferentTeam(thisUnit.Team))
//    {
//        targetToAttack = null;
//        isAttacking = false;

//        return;
//    }

//    var distance = isTargetBuilding
//        ? Vector3.Distance(transform.position, buildingAttackPosition)
//        : Vector3.Distance(transform.position, targetToAttack.transform.position);

//    if (distance > thisUnit.AttackRange)
//    {
//        isAttacking = false;
//        isMoving = true;
//        isChasing = !isTargetBuilding;

//        return;
//    }

//    if (isTargetBuilding)
//    {
//        ((BuildingBase)targetToAttack).TakeDamage(thisUnit.Damage * Time.deltaTime);

//        if (((BuildingBase)targetToAttack).Health <= 0)
//        {
//            targetToAttack = null;
//            isAttacking = false;
//        }

//        return;
//    }

//    ((UnitBase)targetToAttack).TakeDamage(thisUnit.Damage * Time.deltaTime);

//    if (((UnitBase)targetToAttack).Health <= 0)
//    {
//        targetToAttack = null;
//        isAttacking = false;
//    }
//}


/*
 * using UnityEngine;
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



using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    private UnitBase thisUnit;
    private float movementSpeed;

    private List<Vector3> currentPath;
    private int currentPathIndex = 0;
    //private bool

    private void Start()
    {
        thisUnit = GetComponent<UnitBase>();

        movementSpeed = thisUnit.MovementSpeed;
    }

    private void Update()
    {
        if (currentPath == null || currentPathIndex >= currentPath.Count) return;

        Vector3 targetPosition = currentPath[currentPathIndex];
        MoveTowards(targetPosition);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            currentPathIndex++;
        }
    }

    public void SetPath(List<Vector3> path)
    {
        currentPath = path;
        currentPathIndex = 0;
    }

    private void MoveTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * movementSpeed * Time.deltaTime;
    }
}
 * */