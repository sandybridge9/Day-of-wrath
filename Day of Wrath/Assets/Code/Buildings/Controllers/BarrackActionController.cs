using System;

using UnityEngine;

using Random = UnityEngine.Random;

public class BarrackActionController : MonoBehaviour
{
    [Header("Unit types")]
    public GameObject WarriorPrefab;

    private BoxCollider warriorCollider;

    public float SpawnRadius = 5f;
    public int MaxSpawnAttempts = 100;
    public float RaycastHeight = 10f;

    private BarrackBuilding selectedBarrack;
    private Bounds selectedBarrackBounds;

    private LayerMask unitTrainingBlockingLayers;
    private LayerMask groundLayers;

    private void Start()
    {
        unitTrainingBlockingLayers = LayerManager.UnitTrainingBlockingLayers;
        groundLayers = LayerManager.GroundLayers;

        warriorCollider = WarriorPrefab.GetComponent<BoxCollider>();
    }

    public void SetSelectedBarrack(BarrackBuilding barrack)
    {
        selectedBarrack = barrack;

        var placementTrigger = selectedBarrack.transform.Find("PlacementTrigger");
        
        if(placementTrigger != null && placementTrigger.TryGetComponent<Collider>(out var placementTriggerCollider))
        {
            selectedBarrackBounds = placementTriggerCollider.bounds;

            return;
        }

        if(selectedBarrack.TryGetComponent<Collider>(out var mainCollider))
        {
            selectedBarrackBounds = mainCollider.bounds;

            return;
        }

        throw new Exception("Could not find a collider on this BarrackBuilding neither on PlacementTrigger, nor on the Main GameObject.");
    }

    public void TrainUnit()
    {
        if (selectedBarrack == null)
        {
            return;
        }

        SpawnUnit(WarriorPrefab);
    }

    private void SpawnUnit(GameObject troopPrefab)
    {
        var spawnPosition = FindEmptySpawnLocation(selectedBarrack.GetSpawnPoint());

        if (spawnPosition != Vector3.zero)
        {
            spawnPosition = AdjustPositionToGround(spawnPosition, troopPrefab);

            Instantiate(troopPrefab, spawnPosition, Quaternion.identity);
        }
    }

    private Vector3 FindEmptySpawnLocation(Vector3 spawnPoint)
    {
        var potentialPositions = new Vector3[MaxSpawnAttempts];

        for (int i = 0; i < MaxSpawnAttempts; i++)
        {
            var randomOffset = new Vector3(
                Random.Range(-SpawnRadius, SpawnRadius),
                0,
                Random.Range(-SpawnRadius, SpawnRadius));

            potentialPositions[i] = spawnPoint + randomOffset;
        }

        Array.Sort(potentialPositions, (a, b) => Vector3.Distance(spawnPoint, a).CompareTo(Vector3.Distance(spawnPoint, b)));

        foreach (Vector3 testPosition in potentialPositions)
        {
            if (!selectedBarrackBounds.Contains(testPosition) && IsPositionValid(testPosition))
            {
                return testPosition;
            }
        }

        return Vector3.zero;
    }

    private bool IsPositionValid(Vector3 position)
    {
        var ray = new Ray(position + Vector3.up * RaycastHeight, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, RaycastHeight * 2, unitTrainingBlockingLayers))
        {
            return false;
        }

        if (Physics.CheckSphere(position, 0.5f, unitTrainingBlockingLayers))
        {
            return false;
        }

        return true;
    }

    private Vector3 AdjustPositionToGround(Vector3 position, GameObject prefab)
    {
        var ray = new Ray(position + Vector3.up * RaycastHeight, Vector3.down);

        if (Physics.Raycast(ray, out var raycastHit, RaycastHeight * 2, groundLayers))
        {
            var unitHalfHeight = warriorCollider.size.y / 2;

            position.y = raycastHit.point.y + unitHalfHeight;

            return position;
        }

        return position;
    }
}
