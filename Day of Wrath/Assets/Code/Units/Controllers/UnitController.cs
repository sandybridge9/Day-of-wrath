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
    public float attackCooldown = 1f;
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
        else
        {
            HandleIdleBehavior();
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

    private void HandleIdleBehavior()
    {
        if (thisUnit.UnitTemper == Temper.Defensive)
        {
            // Defensive units do nothing unless attacked
            return;
        }

        if (thisUnit.UnitTemper == Temper.Aggressive)
        {
            AttackNearbyEnemiesOrBuildings();
        }
    }

    private void AttackNearbyEnemiesOrBuildings()
    {
        Debug.Log("I should engage that something");
        Collider[] colliders = Physics.OverlapSphere(transform.position, thisUnit.TemperRange, LayerManager.AttackableLayers);

        foreach (Collider collider in colliders)
        {
            Debug.Log(collider);
            if (collider.TryGetComponent<UnitBase>(out var enemyUnit) && enemyUnit.Team != thisUnit.Team)
            {
                EngageUnit(enemyUnit);

                return;
            }

            var enemyBuilding = collider.GetComponentInParent<BuildingBase>();
            if (enemyBuilding != null && enemyBuilding.Team != thisUnit.Team)
            {
                Debug.Log("I should engage that building");
                EngageBuilding(enemyBuilding);
                return;
            }
        }
    }

    private void EngageUnit(UnitBase enemyUnit)
    {
        var startNode = pathfinding.grid.NodeFromWorldPoint(transform.position);
        var targetNode = pathfinding.grid.NodeFromWorldPoint(enemyUnit.transform.position);

        if (startNode != null && targetNode != null && targetNode.walkable)
        {
            var path = pathfinding.FindPath(startNode.worldPosition, targetNode.worldPosition);
            if (path.Count > 0)
            {
                SetAttackTarget(enemyUnit, path, enemyUnit.transform.position);
            }
        }
    }

    private void EngageBuilding(BuildingBase building)
    {
        var boxCollider = building.transform.Find("PlacementTrigger").GetComponent<BoxCollider>();

        if (boxCollider == null)
        {
            Debug.LogError($"Building {building.name} is missing a PlacementTrigger BoxCollider.");
            return;
        }

        var boxCenter = boxCollider.transform.TransformPoint(boxCollider.center);
        var boxSize = boxCollider.size;
        var boxRotation = boxCollider.transform.rotation;

        Vector3 attackPosition;
        var occupiedNodes = new HashSet<Node>();

        if (thisUnit.AttackRange < 1.5f)
        {
            attackPosition = FindClosestWalkableNodeNearBuilding(
                transform.position,
                boxCenter,
                boxSize,
                boxRotation,
                occupiedNodes
            );
        }
        else
        {
            attackPosition = FindPositionAroundBox(
                boxCenter,
                boxSize,
                boxRotation,
                thisUnit.AttackRange,
                occupiedNodes
            );
        }

        Debug.Log(attackPosition);
        var startNode = pathfinding.grid.NodeFromWorldPoint(transform.position);
        var targetNode = pathfinding.grid.NodeFromWorldPoint(attackPosition);

        if (startNode != null && targetNode != null && targetNode.walkable)
        {
            var path = pathfinding.FindPath(startNode.worldPosition, targetNode.worldPosition);
            if (path.Count > 0)
            {
                SetAttackTarget(building, path, attackPosition);
            }
        }
    }

    private Vector3 FindClosestWalkableNodeNearBuilding(Vector3 unitPosition, Vector3 boxCenter, Vector3 boxSize, Quaternion boxRotation, HashSet<Node> occupiedNodes)
    {
        var halfX = boxSize.x / 2;
        var halfZ = boxSize.z / 2;

        var closestDistance = float.MaxValue;
        var closestPosition = unitPosition;

        for (var x = -halfX; x <= halfX; x += 1f)
        {
            for (var z = -halfZ; z <= halfZ; z += 1f)
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

        return closestPosition;
    }

    private Vector3 FindPositionAroundBox(Vector3 center, Vector3 size, Quaternion rotation, float attackRange, HashSet<Node> occupiedNodes)
    {
        var halfX = size.x / 2 + attackRange;
        var halfZ = size.z / 2 + attackRange;

        for (var x = -halfX; x <= halfX; x += 1f)
        {
            for (var z = -halfZ; z <= halfZ; z += 1f)
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
}