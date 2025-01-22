using UnityEngine;

public class TownhallBuilding : BuildingBase
{
    public Transform SpawnPoint;

    public Vector3 GetSpawnPoint() => SpawnPoint != null
        ? SpawnPoint.position
        : transform.position;

    public override void OnBuildingPlaced()
    {
        var buildingPlacementController = FindObjectOfType<BuildingPlacementController>();

        buildingPlacementController.HasTownhall = true;

        base.OnBuildingPlaced();
    }

    public override void OnBuildingDestroyed()
    {
        var buildingPlacementController = FindObjectOfType<BuildingPlacementController>();

        buildingPlacementController.HasTownhall = false;

        base.OnBuildingDestroyed();
    }
}