using System.Collections.Generic;
using UnityEngine;

public class SimulationLogger : MonoBehaviour
{
    public static SimulationLogger Instance;
    private float simulationTime = 0f;
    private float logInterval = 30f;
    private float duration = 180f;

    private int buildingsBuilt = 0;
    private List<float> decisionTimes = new();

    public List<float> B_log = new();
    public List<float> S_log = new();
    public List<float> K_log = new();
    public Dictionary<ResourceType, List<int>> P_log = new();

    private ResourceController resourceController;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        foreach (ResourceType type in System.Enum.GetValues(typeof(ResourceType)))
            P_log[type] = new List<int>();

        resourceController = FindObjectOfType<ResourceController>();
    }

    private void Update()
    {
        simulationTime += Time.deltaTime;
        if (simulationTime >= duration) { enabled = false; return; }
        if (simulationTime % logInterval < Time.deltaTime) LogSnapshot();
    }

    public void RegisterBuilding() => buildingsBuilt++;
    public void RegisterDecisionTime(float ms) => decisionTimes.Add(ms);

    private void LogSnapshot()
    {
        B_log.Add(buildingsBuilt);
        buildingsBuilt = 0;

        float totalUsed = 0, totalCap = 0;
        foreach (ResourceType type in System.Enum.GetValues(typeof(ResourceType)))
        {
            totalUsed += resourceController.GetResourceAmount(type);
            totalCap += resourceController.GetCapacity(type);
        }
        float S_t = (totalCap > 0) ? (totalUsed / totalCap) * 100f : 0f;
        S_log.Add(S_t);

        var prod = CalculateTotalProduction();
        foreach (var kvp in prod) P_log[kvp.Key].Add(kvp.Value);

        K_log.Add(CalculateBalanceCoefficient(prod));
    }

    private Dictionary<ResourceType, int> CalculateTotalProduction()
    {
        Dictionary<ResourceType, int> result = new();
        foreach (ResourceType rt in System.Enum.GetValues(typeof(ResourceType))) result[rt] = 0;
        foreach (var producer in FindObjectsOfType<ResourceProducer>())
            foreach (var p in producer.ResourceProductions)
                result[p.resourceType] += p.productionAmount;
        return result;
    }

    private float CalculateBalanceCoefficient(Dictionary<ResourceType, int> prod)
    {
        int n = prod.Count;
        float total = 0;
        foreach (var p in prod.Values) total += p;
        float ideal = 1f / n, K = 0f;
        foreach (var p in prod.Values)
        {
            float share = (total > 0) ? p / total : 0f;
            K += Mathf.Abs(share - ideal);
        }
        return K / n;
    }

    public float GetAverageDecisionTime()
    {
        if (decisionTimes.Count == 0) return 0f;
        float sum = 0f;
        foreach (var d in decisionTimes) sum += d;
        return sum / decisionTimes.Count;
    }
}