using System.Collections.Generic;
using UnityEngine;

public class ResourceController : MonoBehaviour
{
    [Header("Initial Resources")]
    public int startingGold = 1000;
    public int startingWood = 500;
    public int startingStone = 300;
    public int startingIron = 200;
    public int startingFood = 100;

    [Header("Initial Capacities")]
    public int startingGoldCapacity = 2000;
    public int startingWoodCapacity = 1000;
    public int startingStoneCapacity = 1000;
    public int startingIronCapacity = 500;
    public int startingFoodCapacity = 500;

    private Dictionary<ResourceType, int> resources = new Dictionary<ResourceType, int>();
    private Dictionary<ResourceType, int> capacities = new Dictionary<ResourceType, int>();

    public void Start()
    {
        // Initialize resources
        resources[ResourceType.Gold] = startingGold;
        resources[ResourceType.Wood] = startingWood;
        resources[ResourceType.Stone] = startingStone;
        resources[ResourceType.Iron] = startingIron;
        resources[ResourceType.Food] = startingFood;

        // Initialize capacities
        capacities[ResourceType.Gold] = startingGoldCapacity;
        capacities[ResourceType.Wood] = startingWoodCapacity;
        capacities[ResourceType.Stone] = startingStoneCapacity;
        capacities[ResourceType.Iron] = startingIronCapacity;
        capacities[ResourceType.Food] = startingFoodCapacity;
    }

    public void IncreaseCapacity(ResourceType type, int amount)
    {
        if (capacities.ContainsKey(type))
        {
            capacities[type] += amount;
            Debug.Log($"{type} capacity increased by {amount}. New capacity: {capacities[type]}");
        }
    }

    public void DecreaseCapacity(ResourceType type, int amount)
    {
        if (capacities.ContainsKey(type))
        {
            capacities[type] -= amount;
            Debug.Log($"{type} capacity decreased by {amount}. New capacity: {capacities[type]}");

            ClampResourceToCapacity(type);
        }
    }

    public void AddResource(ResourceType type, int amount)
    {
        if (resources.ContainsKey(type))
        {
            int newAmount = Mathf.Min(resources[type] + amount, capacities[type]);
            int actualAdded = newAmount - resources[type];
            resources[type] = newAmount;

            Debug.Log($"{actualAdded} {type} added. New total: {resources[type]} / {capacities[type]}");
        }
    }

    public bool SpendResources(Cost[] costs)
    {
        if (!CanAfford(costs))
        {
            return false;
        }

        foreach (var cost in costs)
        {
            resources[cost.resourceType] -= cost.amount;

            Debug.Log($"{cost.amount} {cost.resourceType} spent. New total: {resources[cost.resourceType]}");
        }

        return true;
    }

    public bool CanAfford(Cost[] costs)
    {
        foreach (var cost in costs)
        {
            if (!resources.ContainsKey(cost.resourceType) || resources[cost.resourceType] < cost.amount)
            {
                Debug.LogWarning($"Not enough {cost.resourceType}. Required: {cost.amount}, Available: {resources[cost.resourceType]}");

                return false;
            }

            Debug.LogWarning($"Enough {cost.resourceType}. Required: {cost.amount}, Available: {resources[cost.resourceType]}");
        }

        return true;
    }

    public int GetResourceAmount(ResourceType type)
    {
        return resources.ContainsKey(type) ? resources[type] : 0;
    }

    public int GetCapacity(ResourceType type)
    {
        return capacities.ContainsKey(type) ? capacities[type] : 0;
    }

    private void ClampResourceToCapacity(ResourceType type)
    {
        if (resources.ContainsKey(type))
        {
            resources[type] = Mathf.Min(resources[type], capacities[type]);

            Debug.Log($"{type} clamped to {resources[type]} due to capacity limit: {capacities[type]}");
        }
    }
}
