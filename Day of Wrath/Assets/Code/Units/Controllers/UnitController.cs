using UnityEngine;

public class UnitController : MonoBehaviour
{
    private UnitBase thisUnit;

    [Header("Movement control settings")]
    public float avoidanceRadius = 3f;
    public float steeringStrength = 100f;
    private float movementHeightOffset = 0f;
    private Vector3 movementTargetPosition;
    private LayerMask unitMovementObstacleLayers;

    private SelectableObject targetToAttack;

    private bool isTargetBuilding;
    private Vector3 buildingAttackPosition;

    private bool isMoving = false;
    private bool isAttacking = false;
    private bool isChasing = false;

    private void Start()
    {
        thisUnit = GetComponent<UnitBase>();
        unitMovementObstacleLayers = LayerManager.UnitMovementObstacleLayers;

        Collider unitCollider = GetComponent<Collider>();

        if (unitCollider != null)
        {
            movementHeightOffset = unitCollider.bounds.extents.y;
        }
    }

    private void Update()
    {
        if (isMoving)
        {
            MoveToTarget();
        }

        if (isAttacking)
        {
            AttackTarget();
        }
    }

    public void SetMovementTargetPosition(Vector3 position)
    {
        if (Physics.Raycast(position + Vector3.up * 5f, Vector3.down, out var hit, 10f, LayerManager.GroundLayers))
        {
            position.y = hit.point.y/* + movementHeightOffset*/;
        }

        isChasing = false;
        isAttacking = false;
        targetToAttack = null;
        isTargetBuilding = false;
        buildingAttackPosition = Vector3.zero;

        movementTargetPosition = position;
        isMoving = true;
    }

    public void SetAttackTarget(SelectableObject target)
    {
        if (!target.IsFromDifferentTeam(thisUnit.Team))
        {
            return;
        }

        targetToAttack = target;

        isTargetBuilding = target is BuildingBase;
        isChasing = !isTargetBuilding;

        if (isTargetBuilding)
        {
            var buildingCollider = target.GetComponentInChildren<Collider>();

            if (buildingCollider != null)
            {
                // Raycast from the unit's position toward the building to find a valid attack position
                var directionToBuilding = (buildingCollider.bounds.center - transform.position).normalized;

                if (Physics.Raycast(transform.position, directionToBuilding, out var hit, Mathf.Infinity, LayerManager.UnitMovementObstacleLayers))
                {
                    // Stop at the point where the ray hits the building's collider, adjusted by the attack range
                    buildingAttackPosition = hit.point - directionToBuilding * thisUnit.AttackRange;
                }
                else
                {
                    // Fallback to the building's center if raycast fails
                    buildingAttackPosition = buildingCollider.bounds.center;
                }
            }
            else
            {
                buildingAttackPosition = target.transform.position;
            }
        }

        isMoving = true;
        isAttacking = false;
    }

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
    //            // Calculate the attack position as the closest point on the building's bounds, adjusted by the unit's attack range
    //            var directionToBuilding = (buildingCollider.bounds.center - transform.position).normalized;
    //            buildingAttackPosition = buildingCollider.bounds.center - directionToBuilding * (buildingCollider.bounds.extents.magnitude + thisUnit.AttackRange);
    //        }
    //        else
    //        {
    //            buildingAttackPosition = target.transform.position;
    //        }
    //    }

    //    isMoving = true;
    //    isAttacking = false;
    //}

    private void MoveToTarget()
    {
        var targetPosition = isChasing && targetToAttack != null
            ? targetToAttack.transform.position
            : isTargetBuilding
                ? buildingAttackPosition
                : movementTargetPosition;

        var direction = (targetPosition - transform.position).normalized;
        direction = ApplyObstacleAvoidance(direction);

        var movePosition = transform.position + direction * thisUnit.MovementSpeed * Time.deltaTime;
        transform.position = movePosition;

        // Adjust the distance threshold for buildings to account for their size
        var distanceThreshold = isTargetBuilding
            ? thisUnit.AttackRange
            : 0.1f;

        if (Vector3.Distance(transform.position, targetPosition) < distanceThreshold)
        {
            isMoving = false;

            if (isChasing || isTargetBuilding)
            {
                isAttacking = true;
            }
        }
    }

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

    //    // Adjust the distance threshold for buildings to account for their size
    //    var distanceThreshold = isTargetBuilding
    //        ? thisUnit.AttackRange + 0.5f
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

    private Vector3 ApplyObstacleAvoidance(Vector3 moveDirection)
    {
        if (Physics.Raycast(transform.position, moveDirection, out var hit, avoidanceRadius, unitMovementObstacleLayers))
        {
            var avoidanceDirection = Vector3.Cross(hit.normal, Vector3.up).normalized;
            moveDirection += avoidanceDirection * steeringStrength;
        }

        return moveDirection.normalized;
    }

    private void AttackTarget()
    {
        if (targetToAttack == null)
        {
            isAttacking = false;

            return;
        }

        if (!targetToAttack.IsFromDifferentTeam(thisUnit.Team))
        {
            targetToAttack = null;
            isAttacking = false;

            return;
        }

        var distance = isTargetBuilding
            ? Vector3.Distance(transform.position, buildingAttackPosition)
            : Vector3.Distance(transform.position, targetToAttack.transform.position);

        if (distance > thisUnit.AttackRange)
        {
            isAttacking = false;
            isMoving = true;
            isChasing = !isTargetBuilding;

            return;
        }

        if (isTargetBuilding)
        {
            ((BuildingBase)targetToAttack).TakeDamage(thisUnit.Damage * Time.deltaTime);

            if (((BuildingBase)targetToAttack).Health <= 0)
            {
                targetToAttack = null;
                isAttacking = false;
            }

            return;
        }

        ((UnitBase)targetToAttack).TakeDamage(thisUnit.Damage * Time.deltaTime);

        if (((UnitBase)targetToAttack).Health <= 0)
        {
            targetToAttack = null;
            isAttacking = false;
        }
    }
}
