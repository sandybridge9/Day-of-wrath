using UnityEngine;

public class BarrackActionController : MonoBehaviour
{
    public GameObject UnitPrefab;

    public float SpawnRadius = 2f;
    public int MaxSpawnAttempts = 10;

    private BarrackBuilding selectedBarrack;
    private LayerMask unitLayer;

    private void Start()
    {
        unitLayer = LayerManager.UnitLayer;
    }

    public void SetSelectedBarrack(BarrackBuilding barrack)
    {
        selectedBarrack = barrack;
        Debug.Log($"TroopTrainingController: Selected Barrack is {barrack.name}");
    }

    public void TrainUnit()
    {
        if (selectedBarrack == null)
        {
            Debug.LogWarning("No Barrack selected for training.");
            return;
        }

        SpawnUnit(UnitPrefab);
    }

    private void SpawnUnit(GameObject troopPrefab)
    {
        Vector3 spawnPosition = FindEmptySpawnLocation(selectedBarrack.GetSpawnPoint());
        if (spawnPosition != Vector3.zero)
        {
            Instantiate(troopPrefab, spawnPosition, Quaternion.identity);
            Debug.Log($"Troop {troopPrefab.name} spawned at {spawnPosition}.");
        }
        else
        {
            Debug.LogWarning($"TroopTrainingController: No valid spawn location found near {selectedBarrack.name}.");
        }
    }

    private Vector3 FindEmptySpawnLocation(Vector3 spawnPoint)
    {
        for (int attempt = 0; attempt < MaxSpawnAttempts; attempt++)
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-SpawnRadius, SpawnRadius),
                0,
                Random.Range(-SpawnRadius, SpawnRadius));

            Vector3 testPosition = spawnPoint + randomOffset;

            if (!Physics.CheckSphere(testPosition, 0.5f, unitLayer))
            {
                return testPosition; // Valid position
            }
        }

        return Vector3.zero; // No valid position
    }
}
