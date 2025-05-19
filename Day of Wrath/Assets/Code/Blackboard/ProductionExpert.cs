using System.Collections.Generic;
using UnityEngine;

public class ProductionExpert
{
    private readonly ResourceController controller;
    private readonly List<GameObject> allBuildingPrefabs;

    private const float CapacityThreshold = 0.9f;

    public ProductionExpert(ResourceController controller, List<GameObject> buildingPrefabs)
    {
        this.controller = controller;
        this.allBuildingPrefabs = buildingPrefabs;
    }

    public ResourceType? GetMostUrgentResource()
    {
        foreach (ResourceType rt in System.Enum.GetValues(typeof(ResourceType)))
        {
            int amount = controller.GetResourceAmount(rt);
            int capacity = controller.GetCapacity(rt);
            float ratio = capacity > 0 ? amount / (float)capacity : 0f;

            if (ratio >= CapacityThreshold)
            {
                Debug.Log($"Capacity for {rt} is high ({amount}/{capacity}). Prioritizing storage.");
                return null;
            }
        }

        var production = CalculateTotalProduction();
        var demandWeights = CalculateCostDemandWeights();
        Dictionary<ResourceType, float> scores = new();

        foreach (ResourceType rt in System.Enum.GetValues(typeof(ResourceType)))
        {
            int prod = production.ContainsKey(rt) ? production[rt] : 0;
            int stock = controller.GetResourceAmount(rt);
            float demand = demandWeights.ContainsKey(rt) ? demandWeights[rt] : 1f;

            float score = (prod + 1f) / ((stock + 1f) * demand);
            scores[rt] = score;

            Debug.Log($"[Priority] {rt}: Prod={prod}, Stock={stock}, Demand={demand}, Score={score:F4}");
        }

        ResourceType? weakest = null;
        float lowestScore = float.MaxValue;

        foreach (var kvp in scores)
        {
            if (kvp.Value < lowestScore)
            {
                weakest = kvp.Key;
                lowestScore = kvp.Value;
            }
        }

        return weakest;
    }

    private Dictionary<ResourceType, int> CalculateTotalProduction()
    {
        Dictionary<ResourceType, int> result = new();

        foreach (ResourceType rt in System.Enum.GetValues(typeof(ResourceType)))
            result[rt] = 0;

        foreach (var producer in Object.FindObjectsOfType<ResourceProducer>())
        {
            foreach (var p in producer.ResourceProductions)
                result[p.resourceType] += p.productionAmount;
        }

        return result;
    }

    private Dictionary<ResourceType, float> CalculateCostDemandWeights()
    {
        Dictionary<ResourceType, float> demand = new();

        foreach (ResourceType rt in System.Enum.GetValues(typeof(ResourceType)))
            demand[rt] = 1f;

        foreach (var prefab in allBuildingPrefabs)
        {
            if (prefab == null) continue;

            var buildingBase = prefab.GetComponent<BuildingBase>();
            if (buildingBase == null || buildingBase.Costs == null) continue;

            foreach (var cost in buildingBase.Costs)
            {
                if (!demand.ContainsKey(cost.resourceType))
                    demand[cost.resourceType] = 1f;

                demand[cost.resourceType] += cost.amount;
            }
        }

        return demand;
    }
}
