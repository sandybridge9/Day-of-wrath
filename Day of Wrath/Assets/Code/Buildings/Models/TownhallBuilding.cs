using UnityEngine;

public class TownhallBuilding : BuildingBase
{
    public Transform SpawnPoint;

    public Vector3 GetSpawnPoint() => SpawnPoint != null
        ? SpawnPoint.position
        : transform.position;
}