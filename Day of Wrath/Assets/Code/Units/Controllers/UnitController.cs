using UnityEngine;

public class UnitController : MonoBehaviour
{
    private UnitBase unitBase;

    [Header("Movement control settings")]
    public float avoidanceRadius = 3f;
    public float steeringStrength = 100f;
    private float movementHeightOffset = 0f;
    private Vector3 movementTargetPosition;
    private LayerMask unitMovementObstacleLayers;

    private UnitBase targetEnemy;

    private bool isMoving = false;
    private bool isAttacking = false;
    private bool isChasing = false;

    private void Start()
    {
        unitBase = GetComponent<UnitBase>();
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

        if (unitBase.Health <= 0)
        {
            Die();
        }
    }

    public void SetMovementTargetPosition(Vector3 position)
    {
        if (Physics.Raycast(position + Vector3.up * 5f, Vector3.down, out var hit, 10f, LayerManager.GroundLayers))
        {
            position.y = hit.point.y + movementHeightOffset;
        }

        isChasing = false;
        isAttacking = false;
        targetEnemy = null;

        movementTargetPosition = position;
        isMoving = true;
    }

    public void SetAttackTarget(UnitBase enemy)
    {
        targetEnemy = enemy;
        isMoving = true;
        isChasing = true;
        isAttacking = false;
    }

    private void MoveToTarget()
    {
        var targetPosition = isChasing && targetEnemy != null
            ? targetEnemy.transform.position
            : movementTargetPosition;

        var direction = (targetPosition - transform.position).normalized;

        direction = ApplyObstacleAvoidance(direction);

        var movePosition = transform.position + direction * unitBase.MovementSpeed * Time.deltaTime;
        transform.position = movePosition;

        var distanceThreshold = isChasing
            ? unitBase.AttackRange
            : 0.1f;

        if (Vector3.Distance(transform.position, targetPosition) < distanceThreshold)
        {
            isMoving = false;

            if (isChasing)
            {
                isAttacking = true;
            }
        }
    }

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
        if (targetEnemy == null)
        {
            isAttacking = false;

            return;
        }

        var distance = Vector3.Distance(transform.position, targetEnemy.transform.position);

        if (distance > unitBase.AttackRange)
        {
            isAttacking = false;
            isMoving = true;
            isChasing = true;

            return;
        }

        targetEnemy.Health -= unitBase.Damage * Time.deltaTime;

        if (targetEnemy.Health <= 0)
        {
            targetEnemy = null;
            isAttacking = false;
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
