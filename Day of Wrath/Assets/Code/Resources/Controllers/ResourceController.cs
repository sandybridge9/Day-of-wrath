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

    private Dictionary<ResourceType, int> resources = new Dictionary<ResourceType, int>();

    private void Start()
    {
        resources[ResourceType.Gold] = startingGold;
        resources[ResourceType.Wood] = startingWood;
        resources[ResourceType.Stone] = startingStone;
        resources[ResourceType.Iron] = startingIron;
        resources[ResourceType.Food] = startingFood;
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

    public void AddResource(ResourceType type, int amount)
    {
        if (resources.ContainsKey(type))
        {
            resources[type] += amount;
            Debug.Log($"{amount} {type} added. New total: {resources[type]}");
        }
    }

    public int GetResourceAmount(ResourceType type)
    {
        return resources.ContainsKey(type) ? resources[type] : 0;
    }
}
