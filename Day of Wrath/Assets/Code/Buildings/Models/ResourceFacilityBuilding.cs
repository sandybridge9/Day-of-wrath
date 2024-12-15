public class ResourceFacilityBuilding : BuildingBase
{
    private ResourceProducer resourceProducer;
    private CapacityBooster capacityBooster;

    protected override void Start()
    {
        base.Start();

        resourceProducer = GetComponent<ResourceProducer>();
        capacityBooster = GetComponent<CapacityBooster>();
    }

    public override void OnBuildingPlaced()
    {
        base.OnBuildingPlaced();

        if (capacityBooster != null)
        {
            capacityBooster.ApplyCapacityBoost();
        }

        if (resourceProducer != null)
        {
            resourceProducer.StartProduction();
        }
    }

    public override void OnBuildingDestroyed()
    {
        base.OnBuildingDestroyed();

        if (capacityBooster != null)
        {
            capacityBooster.RevertCapacityBoost();
        }

        if (resourceProducer != null)
        {
            resourceProducer.StopProduction();
        }
    }
}
