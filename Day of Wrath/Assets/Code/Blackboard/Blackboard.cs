using UnityEngine;
using System.Diagnostics;

public class Blackboard : MonoBehaviour
{
    public ResourceController resourceController;
    public BuildingExpert buildingExpert;
    public ProductionExpert productionExpert;

    public float checkInterval = 3f;

    private void Start()
    {
        if (resourceController == null) resourceController = FindObjectOfType<ResourceController>();
        if (buildingExpert == null) buildingExpert = FindObjectOfType<BuildingExpert>();
        if (productionExpert == null) productionExpert = new ProductionExpert(resourceController, buildingExpert.GetAllPrefabs());

        buildingExpert.SetDependencies(resourceController);

        InvokeRepeating(nameof(EvaluateAndAct), 1f, checkInterval);
    }

    private void EvaluateAndAct()
    {
        Stopwatch sw = Stopwatch.StartNew();
        var urgent = productionExpert.GetMostUrgentResource();
        sw.Stop();

        SimulationLogger.Instance?.RegisterDecisionTime(sw.ElapsedMilliseconds);

        if (urgent.HasValue) buildingExpert.BuildFor(urgent.Value);
        else buildingExpert.BuildWarehouse();
    }
}