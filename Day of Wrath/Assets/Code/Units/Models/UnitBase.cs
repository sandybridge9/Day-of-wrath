using UnityEngine;

public class UnitBase : SelectableObject
{
    [Header("Basic Properties")]
    public float Health = 100f;
    public float MovementSpeed = 5f;
    public float UnitRadius = 1.5f;

    [Header("Attack Properties")]
    public float Damage = 10f;
    public float AttackRange = 2f;

    [Header("Cost")]
    public Cost[] Costs;

    public override SelectableObjectType Type { get; } = SelectableObjectType.Unit;

    protected override void Update()
    {
        base.Update();
    }
}
