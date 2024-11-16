using UnityEngine;

public class UnitMovementController : MonoBehaviour
{
    private UnitBase unitBase;
    private Vector3 targetPosition;
    private bool isMoving = false;

    [SerializeField]
    private float groundOffset = 0.1f; // Ensures the unit stays slightly above the ground

    private void Start()
    {
        unitBase = GetComponent<UnitBase>();
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
        targetPosition = AdjustForGroundOffset(position);
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

    private Vector3 AdjustForGroundOffset(Vector3 position)
    {
        // Adjust the target position to include the ground offset
        return new Vector3(position.x, position.y + groundOffset, position.z);
    }
}
