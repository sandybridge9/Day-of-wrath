using UnityEngine;

public class BuildingBase : SelectableObject
{
    [Header("Building Properties")]
    public float Health = 200f;

    [Header("Cost")]
    public Cost[] Costs;

    public override SelectableObjectType Type { get; } = SelectableObjectType.Building;

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

    public void TakeDamage(float damage)
    {
        Health -= damage;

        if (Health <= 0)
        {
            Destroy();
        }
    }

    private void Destroy()
    {
        OnBuildingDestroyed();

        Destroy(gameObject);
    }
}
