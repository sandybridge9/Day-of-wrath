using UnityEngine;

public class ResourceProducerBuilding : BuildingBase
{
    private ResourceProducer resourceProducer;

    protected override void Start()
    {
        base.Start();

        resourceProducer = GetComponent<ResourceProducer>();
    }

    public override void OnBuildingPlaced()
    {
        base.OnBuildingPlaced();

        if (resourceProducer != null)
        {
            resourceProducer.StartProduction();
        }
    }

    public override void OnBuildingDestroyed()
    {
        base.OnBuildingDestroyed();

        if (resourceProducer != null)
        {
            resourceProducer.StopProduction();
        }
    }
}