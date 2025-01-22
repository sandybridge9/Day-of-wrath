using UnityEngine;

public class SpawnEnemyController : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject enemyUnitPrefab;
    public GameObject enemyTownhallPrefab;

    private GameObject prefabToSpawn;
    private bool spawningEnabled = false; // Tracks whether spawning mode is active

    [Header("Layer Settings")]
    public LayerMask terrainLayer; // Layer mask for terrain

    private void Update()
    {
        if (spawningEnabled && Input.GetMouseButtonDown(0)) // Left-click to spawn
        {
            SpawnAtMousePosition();
        }

        if (Input.GetMouseButtonDown(1)) // Right-click to cancel spawning
        {
            DisableSpawning();
        }
    }

    public void SetPrefabToSpawn(GameObject prefab)
    {
        prefabToSpawn = prefab;
        spawningEnabled = true; // Enable spawning mode
        Debug.Log($"Spawning mode enabled for: {prefab.name}");
    }

    private void SpawnAtMousePosition()
    {
        if (prefabToSpawn == null) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, terrainLayer))
        {
            Instantiate(prefabToSpawn, hitInfo.point, Quaternion.identity);
            Debug.Log($"Spawned {prefabToSpawn.name} at {hitInfo.point}");
        }
        else
        {
            Debug.LogWarning("Click position is not on the terrain.");
        }
    }

    private void DisableSpawning()
    {
        spawningEnabled = false; // Disable spawning mode
        prefabToSpawn = null;
        Debug.Log("Spawning mode disabled.");
    }
}
