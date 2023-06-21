using UnityEngine;

public class UnitBase : SelectableObject
{
    public float Health;

    public float MovementSpeed;

    public override SelectableObjectType Type { get; } = SelectableObjectType.Unit;
}

