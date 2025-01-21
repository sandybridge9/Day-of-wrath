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
