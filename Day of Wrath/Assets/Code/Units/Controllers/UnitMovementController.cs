using UnityEngine;

public class UnitMovementController : MonoBehaviour
{
    private UnitBase unitBase;
    private Vector3 targetPosition;
    private bool isMoving = false;

    private float heightOffset = 0f;

    private void Start()
    {
        unitBase = GetComponent<UnitBase>();

        Collider unitCollider = GetComponent<Collider>();

        if (unitCollider != null)
        {
            heightOffset = unitCollider.bounds.extents.y;
        }
    }

    private void Update()
    {
        if (isMoving)
        {
            MoveToTarget();
        }
    }

    public void SetTargetPosition(Vector3 position)
    {
        targetPosition = AdjustForHeightOffset(position);
        isMoving = true;
    }

    private void MoveToTarget()
    {
        float movementSpeed = unitBase.MovementSpeed;

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            isMoving = false;
        }
    }

    private Vector3 AdjustForHeightOffset(Vector3 position)
    {
        return new Vector3(position.x, position.y + heightOffset, position.z);
    }
}
