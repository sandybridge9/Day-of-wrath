using UnityEngine;

public class BuildingBase : SelectableObject
{
    [Header("Building Properties")]
    public float Health = 200f;

    [Header("Cost")]
    public Cost[] Costs;

    [SerializeField]
    private Team team = Team.Friendly;

    public BuildingType BuildingType;

    public BuildingType[] AllowedCollisionBuildingTypes;

    public override Team Team
    {
        get => team;
        set => team = value;
    }

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
        UpdatePathfindingGridOnDestroy();
    }

    public void TakeDamage(float damage)
    {
        Health -= damage;

        if (Health <= 0)
        {
            Destroy();
        }
    }

    public void Destroy()
    {
        OnBuildingDestroyed();

        Destroy(gameObject);
    }

    private void UpdatePathfindingGridOnDestroy()
    {
        var pathfindingGrid = FindObjectOfType<PathfindingGrid>();
        var buildingColliders = transform.Find("PlacementTrigger").GetComponents<Collider>();

        foreach (var buildingCollider in buildingColliders)
        {
            var boxCollider = buildingCollider as BoxCollider;
            if (boxCollider == null)
            {
                continue;
            }

            pathfindingGrid.UpdateNodesForBuilding(boxCollider, true);
        }
    }
}
