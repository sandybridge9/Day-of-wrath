using UnityEngine;

public class BuildingBase : SelectableObject
{
    public float Health;

    public override SelectableObjectType Type { get; } = SelectableObjectType.Building;

    [Header("Cost")]
    public Cost[] Costs;
}
