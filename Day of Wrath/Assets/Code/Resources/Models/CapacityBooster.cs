using UnityEngine;

public class CapacityBooster : MonoBehaviour
{
    [Header("Capacity Boosts")]
    public CapacityBoost[] capacityBoosts;

    private ResourceController resourceController;

    private void Start()
    {
        resourceController = FindObjectOfType<ResourceController>();

        if (resourceController == null)
        {
            Debug.LogError("ResourceController not found in the scene!");
        }
    }

    public void ApplyCapacityBoost()
    {
        if (resourceController != null && capacityBoosts != null)
        {
            foreach (var boost in capacityBoosts)
            {
                resourceController.IncreaseCapacity(boost.resourceType, boost.amount);

                Debug.Log($"{boost.amount} capacity added for {boost.resourceType}");
            }
        }
    }

    public void RevertCapacityBoost()
    {
        if (resourceController != null && capacityBoosts != null)
        {
            foreach (var boost in capacityBoosts)
            {
                resourceController.DecreaseCapacity(boost.resourceType, boost.amount);

                Debug.Log($"{boost.amount} capacity removed for {boost.resourceType}");
            }
        }
    }

    public void ManualInit()
    {
        resourceController = FindObjectOfType<ResourceController>();

        if (resourceController == null)
        {
            Debug.LogError("ResourceController not found in the scene!");
        }
    }
}
