using System.Collections.Generic;
using UnityEngine;

public class ResourceSpawner : MonoBehaviour
{
    [Header("Resource Prefabs")]
    public GameObject woodPrefab;
    public GameObject rockPrefab;

    [Header("Spawn Settings")]
    public Vector2 mapSize = new Vector2(500, 500);
    public int defaultWoodCount = 20;
    public int defaultRockCount = 20;
    public float clusterRadius = 5f;
    public int clusterSize = 5;

    [Header("Terrain Reference")]
    public Terrain terrain;

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
        var spawned = 0;

        while (spawned < totalCount)
        {
            var clusterCenter = new Vector3(
                Random.Range(-mapSize.x / 2, mapSize.x / 2),
                0,
                Random.Range(-mapSize.y / 2, mapSize.y / 2)
            );

            for (var i = 0; i < clusterSize && spawned < totalCount; i++)
            {
                var randomOffset = new Vector3(
                    Random.Range(-clusterRadius, clusterRadius),
                    0,
                    Random.Range(-clusterRadius, clusterRadius)
                );

                var spawnPosition = clusterCenter + randomOffset;
                spawnPosition.y = terrain.SampleHeight(spawnPosition) + terrain.transform.position.y;

                var randomScale = Random.Range(0.8f, 3f);
                var randomRotation = RandomRotation();

                if (IsValidSpawnPosition(spawnPosition, prefab, randomScale, randomRotation))
                {
                    var resource = Instantiate(prefab, spawnPosition, randomRotation);
                    resource.transform.localScale = Vector3.one * randomScale;
                    spawnedResources.Add(resource);
                    spawned++;
                }
            }
        }
    }

    public void SpawnResourcesInGrid(int woodCount, int rockCount)
    {
        ClearResources();

        var gridSize = Mathf.CeilToInt(Mathf.Sqrt(woodCount + rockCount));
        var cellSize = Mathf.Min(mapSize.x / gridSize, mapSize.y / gridSize);

        var spawnedWoods = 0;
        var spawnedRocks = 0;

        for (var x = 0; x < gridSize; x++)
        {
            for (var z = 0; z < gridSize; z++)
            {
                if (spawnedWoods >= woodCount && spawnedRocks >= rockCount)
                {
                    return;
                }

                var position = new Vector3(
                    -mapSize.x / 2 + x * cellSize + Random.Range(-cellSize / 2, cellSize / 2),
                    0,
                    -mapSize.y / 2 + z * cellSize + Random.Range(-cellSize / 2, cellSize / 2)
                );

                position.y = terrain.SampleHeight(position) + terrain.transform.position.y;

                var randomScale = Random.Range(0.8f, 3f);
                var randomRotation = RandomRotation();

                if (spawnedWoods < woodCount && IsValidSpawnPosition(position, woodPrefab, randomScale, randomRotation))
                {
                    var wood = Instantiate(woodPrefab, position, randomRotation);
                    wood.transform.localScale = Vector3.one * randomScale;
                    spawnedResources.Add(wood);
                    spawnedWoods++;
                }
                else if (spawnedRocks < rockCount && IsValidSpawnPosition(position, rockPrefab, randomScale, randomRotation))
                {
                    var rock = Instantiate(rockPrefab, position, randomRotation);
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

        var center = Vector3.zero; // Center point for radial distribution (e.g., the map's center)
        var maxRadius = Mathf.Min(mapSize.x, mapSize.y) / 2;
        var totalCount = woodCount + rockCount;

        var ringCount = Mathf.CeilToInt(Mathf.Sqrt(totalCount));
        var ringSpacing = maxRadius / ringCount;

        var spawnedWoods = 0;
        var spawnedRocks = 0;

        for (int ring = 1; ring <= ringCount; ring++)
        {
            var resourcesInRing = Mathf.CeilToInt((float)totalCount / ringCount);
            var radius = ring * ringSpacing;

            for (var i = 0; i < resourcesInRing; i++)
            {
                if (spawnedWoods >= woodCount && spawnedRocks >= rockCount)
                {
                    return;
                }

                var angle = (360f / resourcesInRing) * i;

                var position = center + new Vector3(
                    Mathf.Cos(angle * Mathf.Deg2Rad) * radius,
                    0,
                    Mathf.Sin(angle * Mathf.Deg2Rad) * radius
                );

                position.y = terrain.SampleHeight(position) + terrain.transform.position.y;

                var randomScale = Random.Range(0.8f, 3f);
                var randomRotation = RandomRotation();

                if (spawnedWoods < woodCount && IsValidSpawnPosition(position, woodPrefab, randomScale, randomRotation))
                {
                    var wood = Instantiate(woodPrefab, position, randomRotation);
                    wood.transform.localScale = Vector3.one * randomScale;
                    spawnedResources.Add(wood);
                    spawnedWoods++;
                }
                else if (spawnedRocks < rockCount && IsValidSpawnPosition(position, rockPrefab, randomScale, randomRotation))
                {
                    var rock = Instantiate(rockPrefab, position, randomRotation);
                    rock.transform.localScale = Vector3.one * randomScale;
                    spawnedResources.Add(rock);
                    spawnedRocks++;
                }
            }
        }
    }

    private bool IsValidSpawnPosition(Vector3 position, GameObject prefab, float scale, Quaternion rotation)
    {
        var prefabCollider = prefab.GetComponentInChildren<BoxCollider>();
        if (prefabCollider == null)
        {
            Debug.LogWarning($"Prefab {prefab.name} is missing a BoxCollider.");
            return false;
        }

        var adjustedCenter = position + rotation * Vector3.Scale(prefabCollider.center, Vector3.one * scale);
        var adjustedSize = Vector3.Scale(prefabCollider.size, Vector3.one * scale);

        var colliders = Physics.OverlapBox(
            adjustedCenter,
            adjustedSize / 2,
            rotation,
            blockingLayers
        );

        return colliders.Length == 0;
    }

    private Quaternion RandomRotation()
    {
        return Quaternion.Euler(0, Random.Range(0f, 360f), 0);
    }
}
