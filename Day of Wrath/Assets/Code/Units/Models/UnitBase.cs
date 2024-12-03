using UnityEngine;

public class UnitBase : SelectableObject
{
    public float Health = 100f;
    public float MovementSpeed = 5f;

    private bool IsMoving;
    private Vector3 destination;

    public override SelectableObjectType Type { get; } = SelectableObjectType.Unit;

    protected override void Update()
    {
        base.Update();

        Move();
    }

    public virtual void BeginMoving(Vector3 newDestination)
    {
        if(transform.position != newDestination)
        {
            IsMoving = true;
            destination = newDestination;

            return;
        }
    }

    public virtual void Move()
    {
        if(!IsMoving || transform.position == destination)
        {
            IsMoving = false;
            destination = transform.position;

            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, destination, MovementSpeed * Time.deltaTime);
    }

    public virtual void CancelMovement()
    {
        IsMoving = false;
        destination = transform.position;
    }
}

