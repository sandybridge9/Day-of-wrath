using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceProducer : MonoBehaviour
{
    [Header("Production Settings")]
    public List<ResourceProduction> ResourceProductions = new List<ResourceProduction>();
    public float ProductionIntervalInSeconds = 5f;

    private ResourceController resourceController;
    private Coroutine productionCoroutine;

    private void Start()
    {
        resourceController = FindObjectOfType<ResourceController>();

        if (resourceController == null)
        {
            Debug.LogError("ResourceController not found in the scene!");

            return;
        }
    }

    public void StartProduction()
    {
        Debug.Log("Starting production");
        productionCoroutine ??= StartCoroutine(ProduceResources());
    }

    public void StopProduction()
    {
        if (productionCoroutine != null)
        {
            StopCoroutine(productionCoroutine);
            productionCoroutine = null;
        }
    }

    private IEnumerator ProduceResources()
    {
        while (true)
        {
            yield return new WaitForSeconds(ProductionIntervalInSeconds);

            foreach (var production in ResourceProductions)
            {
                resourceController.AddResource(production.resourceType, production.productionAmount);

                Debug.Log($"{production.productionAmount} {production.resourceType} produced by {gameObject.name}");
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
