using NUnit.Framework;
using UnityEngine;

public class ResourceControllerTests
{
    // WHITE-BOX (STRUCTURAL)
    [Test]
    public void IncreaseCapacity_WhenCalled_IncreasesCapacityCorrectly()
    {
        var gameObject = new GameObject();
        var resourceController = gameObject.AddComponent<ResourceController>();
        resourceController.Start();

        resourceController.IncreaseCapacity(ResourceType.Gold, 500);

        Assert.AreEqual(2500, resourceController.GetCapacity(ResourceType.Gold));
    }

    [Test]
    public void DecreaseCapacity_WhenCalled_DecreasesCapacityAndClampsResource()
    {
        var gameObject = new GameObject();
        var resourceController = gameObject.AddComponent<ResourceController>();
        resourceController.Start();

        resourceController.DecreaseCapacity(ResourceType.Gold, 1500);

        Assert.AreEqual(500, resourceController.GetCapacity(ResourceType.Gold));
        Assert.AreEqual(500, resourceController.GetResourceAmount(ResourceType.Gold));
    }

    [Test]
    public void AddResource_WhenCalled_ClampsToCapacity()
    {
        var gameObject = new GameObject();
        var resourceController = gameObject.AddComponent<ResourceController>();
        resourceController.Start();

        resourceController.AddResource(ResourceType.Gold, 1500);

        Assert.AreEqual(2000, resourceController.GetResourceAmount(ResourceType.Gold));
    }

    [Test]
    public void SpendResources_WithSufficientResources_ReturnsTrueAndDeductsResources()
    {
        var gameObject = new GameObject();
        var resourceController = gameObject.AddComponent<ResourceController>();
        resourceController.Start();

        var cost = new Cost[] { new() { resourceType = ResourceType.Gold, amount = 500 } };

        bool result = resourceController.SpendResources(cost);

        Assert.IsTrue(result);
        Assert.AreEqual(500, resourceController.GetResourceAmount(ResourceType.Gold));
    }

    [Test]
    public void SpendResources_WithMultipleCosts_SucceedsAndDeductsResources()
    {
        var gameObject = new GameObject();
        var resourceController = gameObject.AddComponent<ResourceController>();
        resourceController.Start();

        var costs = new Cost[]
        {
            new() { resourceType = ResourceType.Gold, amount = 300 },
            new() { resourceType = ResourceType.Wood, amount = 200 }
        };

        bool result = resourceController.SpendResources(costs);

        Assert.IsTrue(result);
        Assert.AreEqual(700, resourceController.GetResourceAmount(ResourceType.Gold));
        Assert.AreEqual(300, resourceController.GetResourceAmount(ResourceType.Wood));
    }

    // BLACK-BOX (FUNCTIONAL)
    [Test]
    public void IncreaseCapacity_IncreasesCapacity()
    {
        var gameObject = new GameObject();
        var resourceController = gameObject.AddComponent<ResourceController>();
        resourceController.Start();

        resourceController.IncreaseCapacity(ResourceType.Wood, 300);

        Assert.AreEqual(1300, resourceController.GetCapacity(ResourceType.Wood));
    }

    [Test]
    public void AddResource_AddsResourceUpToCapacity()
    {
        var gameObject = new GameObject();
        var resourceController = gameObject.AddComponent<ResourceController>();
        resourceController.Start();

        resourceController.AddResource(ResourceType.Wood, 600);

        Assert.AreEqual(1000, resourceController.GetResourceAmount(ResourceType.Wood));
    }

    [Test]
    public void CanAfford_ReturnsTrueWhenResourcesAreEnough()
    {
        var gameObject = new GameObject();
        var resourceController = gameObject.AddComponent<ResourceController>();
        resourceController.Start();

        var costs = new Cost[] { new() { resourceType = ResourceType.Stone, amount = 100 } };

        Assert.IsTrue(resourceController.CanAfford(costs));
    }
}
