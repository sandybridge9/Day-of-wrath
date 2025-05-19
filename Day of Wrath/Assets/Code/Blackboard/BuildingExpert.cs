using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingExpert : MonoBehaviour
{
    private ResourceController resourceController;

    public GameObject farmPrefab;
    public GameObject woodcutterPrefab;
    public GameObject minePrefab;
    public GameObject marketPrefab;
    public GameObject warehousePrefab;

    public Vector3 startPos = new Vector3(10, 0, 10);
    public float buildInterval = 0.1f;
    public float placementRadius = 30f;
    public int maxPlacementAttempts = 20;

    private Queue<BuildingType> buildQueue = new();
    private bool isPlacing = false;
    private int buildCount = 0;

    public void SetDependencies(ResourceController controller)
    {
        resourceController = controller;
    }

    public void BuildFor(ResourceType resource)
    {
        BuildingType type = GetBuildingForResource(resource);
        GameObject prefab = GetPrefab(type);
        if (prefab == null) return;

        var baseComp = prefab.GetComponent<BuildingBase>();
        if (baseComp == null || !resourceController.CanAfford(baseComp.Costs)) return;

        buildQueue.Enqueue(type);
        if (!isPlacing) StartCoroutine(ProcessBuildQueue());
    }

    public void BuildWarehouse()
    {
        GameObject prefab = GetPrefab(BuildingType.Warehouse);
        if (prefab == null) return;

        var baseComp = prefab.GetComponent<BuildingBase>();
        if (baseComp == null || !resourceController.CanAfford(baseComp.Costs)) return;

        buildQueue.Enqueue(BuildingType.Warehouse);
        if (!isPlacing) StartCoroutine(ProcessBuildQueue());
    }

    private IEnumerator ProcessBuildQueue()
    {
        isPlacing = true;

        while (buildQueue.Count > 0)
        {
            BuildingType type = buildQueue.Dequeue();
            GameObject prefab = GetPrefab(type);
            if (prefab == null) continue;

            var baseComp = prefab.GetComponent<BuildingBase>();
            if (baseComp == null || !resourceController.CanAfford(baseComp.Costs)) continue;

            resourceController.SpendResources(baseComp.Costs);
            Vector3 pos = CalculateOrganicPosition(prefab);
            GameObject building = Instantiate(prefab, pos, Quaternion.identity);

            var facility = building.GetComponent<ResourceFacilityBuilding>();
            if (facility != null)
            {
                facility.ManualInit();
                facility.OnBuildingPlaced();
            }

            SimulationLogger.Instance?.RegisterBuilding();

            buildCount++;
            yield return new WaitForSeconds(buildInterval);
        }

        isPlacing = false;
    }

    private Vector3 CalculateOrganicPosition(GameObject prefab)
    {
        float dynamicRadius = Mathf.Sqrt(buildCount + 1) * 5f;
        for (int i = 0; i < maxPlacementAttempts; i++)
        {
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float distance = Random.Range(2f, dynamicRadius);
            Vector3 candidate = startPos + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * distance;
            if (IsSpotFree(candidate, prefab)) return candidate;
        }
        return startPos;
    }

    private bool IsSpotFree(Vector3 position, GameObject prefab)
    {
        Collider col = prefab.GetComponentInChildren<Collider>();
        if (col == null) return true;

        Vector3 size = col.bounds.size;
        Vector3 halfExtents = size / 2f;
        Vector3 offset = col.bounds.center - prefab.transform.position;

        Collider[] hits = Physics.OverlapBox(position + offset, halfExtents, Quaternion.identity, LayerMask.GetMask("Building"));
        return hits.Length == 0;
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

    public GameObject GetPrefab(BuildingType type)
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

    public List<GameObject> GetAllPrefabs()
    {
        return new List<GameObject> { farmPrefab, woodcutterPrefab, minePrefab, marketPrefab, warehousePrefab };
    }
}