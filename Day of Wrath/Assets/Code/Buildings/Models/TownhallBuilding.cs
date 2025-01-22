using UnityEngine;

public class TownhallBuilding : BuildingBase
{
    public Transform SpawnPoint;

    public Vector3 GetSpawnPoint() => SpawnPoint != null
        ? SpawnPoint.position
        : transform.position;

    public override void OnBuildingPlaced()
    {
        if(Team == Team.Friendly)
        {
            var buildingPlacementController = FindObjectOfType<BuildingPlacementController>();

            buildingPlacementController.HasTownhallBeenBuilt = true;
            buildingPlacementController.HasTownhall = true;

            base.OnBuildingPlaced();
        }
        else
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

                pathfindingGrid.UpdateNodesForBuilding(boxCollider, false);
            }
        }
    }

    public override void OnBuildingDestroyed()
    {
        if(Team == Team.Enemy)
        {
            var winLoseController = FindObjectOfType<WinLoseController>();

            winLoseController.EnemyHasTownhall = false;
        }
        else
        {
            var buildingPlacementController = FindObjectOfType<BuildingPlacementController>();

            buildingPlacementController.HasTownhall = false;
        }

        base.OnBuildingDestroyed();
    }
}