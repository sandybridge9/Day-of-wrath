using UnityEngine;

public class BuildingBase : SelectableObject
{
    public float Health;

    public override SelectableObjectType Type { get; } = SelectableObjectType.Building;

    [Header("Cost")]
    public Cost[] Costs;

    protected override void Start()
    {
        base.Start();
    }

    public virtual void OnBuildingPlaced()
    {

    }

    public virtual void OnBuildingDestroyed()
    {

    }
}
