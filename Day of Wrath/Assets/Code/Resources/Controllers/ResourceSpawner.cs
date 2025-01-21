using System.Collections.Generic;
using UnityEngine;

public class ResourceSpawner : MonoBehaviour
{
    [Header("Resource Prefabs")]
    public GameObject woodPrefab;
    public GameObject rockPrefab;

    [Header("Spawn Settings")]
    public Vector2 mapSize = new Vector2(100, 100);
    public int defaultWoodCount = 20;
    public int defaultRockCount = 20;
    public float clusterRadius = 5f;
    public int clusterSize = 5;

    private LayerMask blockingLayers;

    private List<GameObject> spawnedResources = new List<GameObject>();

    private void Start()
    {
        blockingLayers = LayerManager.ResourceSpawningBlockingLayers;
    }

    public void SpawnResourcesInClusters(int woodCount, int rockCount)
    {
        ClearResources();

        SpawnResourceClusters(woodPrefab, woodCount);
        SpawnResourceClusters(rockPrefab, rockCount);
    }

    public void ClearResources()
    {
        foreach (var resource in spawnedResources)
        {
            Destroy(resource);
        }
        spawnedResources.Clear();
    }

    private void SpawnResourceClusters(GameObject prefab, int totalCount)
    {
        int spawned = 0;

        while (spawned < totalCount)
        {
            // Choose a random cluster center
            Vector3 clusterCenter = new Vector3(
                Random.Range(-mapSize.x / 2, mapSize.x / 2),
                0,
                Random.Range(-mapSize.y / 2, mapSize.y / 2)
            );

            for (int i = 0; i < clusterSize && spawned < totalCount; i++)
            {
                Vector3 randomOffset = new Vector3(
                    Random.Range(-clusterRadius, clusterRadius),
                    0,
                    Random.Range(-clusterRadius, clusterRadius)
                );

                Vector3 spawnPosition = clusterCenter + randomOffset;

                // Generate random scale and rotation
                float randomScale = Random.Range(0.8f, 1.5f);
                Quaternion randomRotation = RandomRotation();

                // Use the same scale and rotation for validity check and spawning
                if (IsValidSpawnPosition(spawnPosition, prefab, randomScale, randomRotation))
                {
                    GameObject resource = Instantiate(prefab, spawnPosition, randomRotation);
                    resource.transform.localScale = Vector3.one * randomScale; // Apply scale
                    spawnedResources.Add(resource);
                    spawned++;
                }
            }
        }
    }

    public void SpawnResourcesInGrid(int woodCount, int rockCount)
    {
        ClearResources();

        int gridSize = Mathf.CeilToInt(Mathf.Sqrt(woodCount + rockCount));
        float cellSize = Mathf.Min(mapSize.x / gridSize, mapSize.y / gridSize);

        int spawnedWoods = 0;
        int spawnedRocks = 0;

        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                // Stop if both resource limits are reached
                if (spawnedWoods >= woodCount && spawnedRocks >= rockCount) return;

                Vector3 position = new Vector3(
                    -mapSize.x / 2 + x * cellSize + Random.Range(-cellSize / 2, cellSize / 2),
                    0,
                    -mapSize.y / 2 + z * cellSize + Random.Range(-cellSize / 2, cellSize / 2)
                );

                // Generate random scale and rotation
                float randomScale = Random.Range(0.8f, 1.5f);
                Quaternion randomRotation = RandomRotation();

                // Alternate between woods and rocks
                if (spawnedWoods < woodCount && IsValidSpawnPosition(position, woodPrefab, randomScale, randomRotation))
                {
                    GameObject wood = Instantiate(woodPrefab, position, randomRotation);
                    wood.transform.localScale = Vector3.one * randomScale;
                    spawnedResources.Add(wood);
                    spawnedWoods++;
                }
                else if (spawnedRocks < rockCount && IsValidSpawnPosition(position, rockPrefab, randomScale, randomRotation))
                {
                    GameObject rock = Instantiate(rockPrefab, position, randomRotation);
                    rock.transform.localScale = Vector3.one * randomScale;
                    spawnedResources.Add(rock);
                    spawnedRocks++;
                }
            }
        }
    }

    public void SpawnResourcesRadial(int woodCount, int rockCount)
    {
        ClearResources();

        Vector3 center = Vector3.zero; // Center point for radial distribution (e.g., the map's center)
        float maxRadius = Mathf.Min(mapSize.x, mapSize.y) / 2;
        int totalCount = woodCount + rockCount;

        // Divide resources into rings
        int ringCount = Mathf.CeilToInt(Mathf.Sqrt(totalCount));
        float ringSpacing = maxRadius / ringCount;

        int spawnedWoods = 0;
        int spawnedRocks = 0;

        for (int ring = 1; ring <= ringCount; ring++)
        {
            int resourcesInRing = Mathf.CeilToInt((float)totalCount / ringCount);
            float radius = ring * ringSpacing;

            for (int i = 0; i < resourcesInRing; i++)
            {
                if (spawnedWoods >= woodCount && spawnedRocks >= rockCount) return;

                // Calculate position on the ring
                float angle = (360f / resourcesInRing) * i;
                Vector3 position = center + new Vector3(
                    Mathf.Cos(angle * Mathf.Deg2Rad) * radius,
                    0,
                    Mathf.Sin(angle * Mathf.Deg2Rad) * radius
                );

                // Generate random scale and rotation
                float randomScale = Random.Range(0.8f, 1.5f);
                Quaternion randomRotation = RandomRotation();

                if (spawnedWoods < woodCount && IsValidSpawnPosition(position, woodPrefab, randomScale, randomRotation))
                {
                    GameObject wood = Instantiate(woodPrefab, position, randomRotation);
                    wood.transform.localScale = Vector3.one * randomScale;
                    spawnedResources.Add(wood);
                    spawnedWoods++;
                }
                else if (spawnedRocks < rockCount && IsValidSpawnPosition(position, rockPrefab, randomScale, randomRotation))
                {
                    GameObject rock = Instantiate(rockPrefab, position, randomRotation);
                    rock.transform.localScale = Vector3.one * randomScale;
                    spawnedResources.Add(rock);
                    spawnedRocks++;
                }
            }
        }
    }

    private bool IsValidSpawnPosition(Vector3 position, GameObject prefab, float scale, Quaternion rotation)
    {
        BoxCollider prefabCollider = prefab.GetComponentInChildren<BoxCollider>();
        if (prefabCollider == null)
        {
            Debug.LogWarning($"Prefab {prefab.name} is missing a BoxCollider.");
            return false;
        }

        // Adjust for scale and rotation
        Vector3 adjustedCenter = position + rotation * Vector3.Scale(prefabCollider.center, Vector3.one * scale);
        Vector3 adjustedSize = Vector3.Scale(prefabCollider.size, Vector3.one * scale);

        Collider[] colliders = Physics.OverlapBox(
            adjustedCenter,
            adjustedSize / 2, // OverlapBox uses half-extents
            rotation,
            blockingLayers
        );

        return colliders.Length == 0;
    }

    private Quaternion RandomRotation()
    {
        return Quaternion.Euler(0, Random.Range(0f, 360f), 0);
    }

    private void ApplyRandomScale(GameObject resource)
    {
        float randomScale = Random.Range(0.8f, 1.5f); // Adjust scale range as needed
        resource.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
    }
}
