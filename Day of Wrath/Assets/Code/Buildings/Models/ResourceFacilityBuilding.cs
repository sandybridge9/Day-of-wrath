using Codice.Client.BaseCommands.Filters;

public class ResourceFacilityBuilding : BuildingBase
{
    private ResourceProducer resourceProducer;
    private CapacityBooster capacityBooster;

    private bool manualInit = false;

    protected override void Start()
    {
        base.Start();

        resourceProducer = GetComponent<ResourceProducer>();
        capacityBooster = GetComponent<CapacityBooster>();
    }

    public override void OnBuildingPlaced()
    {
        base.OnBuildingPlaced();

        if (manualInit)
        {
            if (capacityBooster != null)
            {
                capacityBooster.ManualInit();
            }

            if (resourceProducer != null)
            {
                resourceProducer.ManualInit();
            }
        }

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

    public void ManualInit()
    {
        manualInit = true;

        resourceProducer = GetComponent<ResourceProducer>();
        capacityBooster = GetComponent<CapacityBooster>();
    }
}
