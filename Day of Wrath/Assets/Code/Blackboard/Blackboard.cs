using System.Collections.Generic;
using UnityEngine;

public class Blackboard : MonoBehaviour
{
    public List<GameObject> allBuildingPrefabs;

    public ResourceController resourceController;

    private ProductionExpert productionExpert;
    public BuildingExpert buildingExpert;

    [Header("Decision Parameters")]
    public float checkInterval = 10f;

    private void Start()
    {
        if (resourceController == null) resourceController = FindObjectOfType<ResourceController>();

        productionExpert = new ProductionExpert(resourceController, allBuildingPrefabs);

        if (buildingExpert == null) buildingExpert = FindObjectOfType<BuildingExpert>();
        buildingExpert.SetDependencies(resourceController);

        InvokeRepeating(nameof(EvaluateAndAct), 2f, checkInterval);
    }
    private void EvaluateAndAct()
    {
        var urgent = productionExpert.GetMostUrgentResource();

        if (urgent.HasValue)
        {
            buildingExpert.BuildFor(urgent.Value);
        }
        else
        {
            // Special signal: we're full on something, build warehouse
            buildingExpert.BuildWarehouse();
        }
    }
}
