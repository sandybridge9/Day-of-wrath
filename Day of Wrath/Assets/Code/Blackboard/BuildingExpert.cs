using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingExpert : MonoBehaviour
{
    private ResourceController resourceController;

    [Header("Building Prefabs")]
    public GameObject farmPrefab;
    public GameObject woodcutterPrefab;
    public GameObject minePrefab;
    public GameObject marketPrefab;
    public GameObject warehousePrefab;

    [Header("Placement Settings")]
    public Vector3 startPos = new Vector3(10, 0, 10);
    public float placementRadius = 30f;
    public int maxPlacementAttempts = 20;

    [Header("Grid & Timing (deprecated unless grid is reenabled)")]
    public int gridWidth = 5;
    public float spacing = 8f;
    public float buildInterval = 3f;

    private int buildCount = 0;
    private Queue<BuildingType> buildQueue = new();
    private bool isPlacing = false;

    public void SetDependencies(ResourceController controller)
    {
        resourceController = controller;
    }

    public void BuildFor(ResourceType resource)
    {
        BuildingType type = GetBuildingForResource(resource);
        GameObject prefab = GetPrefab(type);

        if (prefab == null)
        {
            Debug.LogWarning($"No prefab for building type {type}");
            return;
        }

        var buildingBase = prefab.GetComponent<BuildingBase>();
        if (buildingBase == null)
        {
            Debug.LogWarning($"Prefab {prefab.name} missing BuildingBase");
            return;
        }

        if (!resourceController.CanAfford(buildingBase.Costs))
        {
            Debug.Log($"Can't afford {type}. Skipping.");
            return;
        }

        buildQueue.Enqueue(type);
        if (!isPlacing)
            StartCoroutine(ProcessBuildQueue());
    }

    public void BuildWarehouse()
    {
        GameObject prefab = GetPrefab(BuildingType.Warehouse);

        if (prefab == null)
        {
            Debug.LogWarning("Warehouse prefab not assigned.");
            return;
        }

        var buildingBase = prefab.GetComponent<BuildingBase>();
        if (buildingBase == null || !resourceController.CanAfford(buildingBase.Costs))
        {
            Debug.Log("Can't afford Warehouse.");
            return;
        }

        buildQueue.Enqueue(BuildingType.Warehouse);
        if (!isPlacing)
            StartCoroutine(ProcessBuildQueue());
    }

    private IEnumerator ProcessBuildQueue()
    {
        isPlacing = true;

        while (buildQueue.Count > 0)
        {
            BuildingType type = buildQueue.Dequeue();
            GameObject prefab = GetPrefab(type);

            if (prefab == null) continue;

            var buildingBase = prefab.GetComponent<BuildingBase>();
            if (buildingBase == null || !resourceController.CanAfford(buildingBase.Costs))
            {
                Debug.Log($"Skipped build of {type} — can't afford or missing base.");
                continue;
            }

            // Spend resources
            resourceController.SpendResources(buildingBase.Costs);

            Vector3 pos = CalculateOrganicPosition(prefab);
            GameObject building = Instantiate(prefab, pos, Quaternion.identity);

            var resourceFacilityBuilding = building.GetComponent<ResourceFacilityBuilding>();
            if (resourceFacilityBuilding != null)
            {
                resourceFacilityBuilding.ManualInit();
                resourceFacilityBuilding.OnBuildingPlaced();
            }

            buildCount++;
            yield return new WaitForSeconds(buildInterval);
        }

        isPlacing = false;
    }

    private GameObject GetPrefab(BuildingType type)
    {
        return type switch
        {
            BuildingType.Farm => farmPrefab,
            BuildingType.Woodcutter => woodcutterPrefab,
            BuildingType.Mine => minePrefab,
            BuildingType.Market => marketPrefab,
            BuildingType.Warehouse => warehousePrefab,
            _ => null
        };
    }

    private BuildingType GetBuildingForResource(ResourceType type)
    {
        return type switch
        {
            ResourceType.Food => BuildingType.Farm,
            ResourceType.Wood => BuildingType.Woodcutter,
            ResourceType.Stone => BuildingType.Mine,
            ResourceType.Iron => BuildingType.Mine,
            ResourceType.Gold => BuildingType.Market,
            _ => BuildingType.Farm
        };
    }

    private Vector3 CalculateOrganicPosition(GameObject prefab)
    {
        int localAttemptCount = 0;
        float dynamicRadius = Mathf.Sqrt(buildCount + 1) * 5f; // spiral-like growth

        while (localAttemptCount < maxPlacementAttempts)
        {
            localAttemptCount++;

            float angle = Random.Range(0f, Mathf.PI * 2f);
            float distance = Random.Range(2f, dynamicRadius);
            Vector3 candidate = startPos + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * distance;

            if (IsSpotFree(candidate, prefab))
                return candidate;
        }

        Debug.LogWarning("⚠️ Couldn't find valid placement spot after max attempts.");
        return startPos + new Vector3(Random.Range(0, 5), 0, Random.Range(0, 5)); // fallback
    }


    private bool IsSpotFree(Vector3 position, GameObject prefab)
    {
        Collider collider = prefab.GetComponentInChildren<Collider>();
        if (collider == null)
        {
            Debug.LogWarning($"No collider found on prefab {prefab.name}");
            return true;
        }

        Vector3 size = collider.bounds.size;
        Vector3 halfExtents = size / 2f;
        Vector3 centerOffset = collider.bounds.center - prefab.transform.position;

        Collider[] hits = Physics.OverlapBox(position + centerOffset, halfExtents, Quaternion.identity, LayerMask.GetMask("Building"));

#if UNITY_EDITOR
        Debug.DrawLine(position + Vector3.up * 0.1f, position + Vector3.up * 2f, hits.Length == 0 ? Color.green : Color.red, 1f);
#endif

        return hits.Length == 0;
    }
}
