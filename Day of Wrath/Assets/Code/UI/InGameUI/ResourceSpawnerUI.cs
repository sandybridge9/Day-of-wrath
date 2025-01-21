using UnityEngine;
using UnityEngine.UI;

public class ResourceSpawnerUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider woodSlider;
    public Slider rockSlider;
    public Button respawnResourcesInClustersButton;
    public Button respawnResourcesInGridButton;
    public Button respawnResourcesRadialButton;

    [Header("Spawner Reference")]
    public ResourceSpawner resourceSpawner;

    private void Start()
    {
        woodSlider.onValueChanged.AddListener(UpdateSliders);
        rockSlider.onValueChanged.AddListener(UpdateSliders);
        respawnResourcesInClustersButton.onClick.AddListener(RespawnResourcesInClusters);
        respawnResourcesInGridButton.onClick.AddListener(RespawnResourcesInGrid);
        respawnResourcesRadialButton.onClick.AddListener(RespawnResourcesRadial);

        // Set default values
        woodSlider.value = resourceSpawner.defaultWoodCount;
        rockSlider.value = resourceSpawner.defaultRockCount;
    }

    private void UpdateSliders(float value)
    {
        // Updates the sliders dynamically
        Debug.Log($"Wood Amount: {woodSlider.value}, Rock Amount: {rockSlider.value}");
    }

    private void RespawnResourcesInClusters()
    {
        int woodCount = Mathf.RoundToInt(woodSlider.value);
        int rockCount = Mathf.RoundToInt(rockSlider.value);

        resourceSpawner.SpawnResourcesInClusters(woodCount, rockCount);
    }

    private void RespawnResourcesInGrid()
    {
        int woodCount = Mathf.RoundToInt(woodSlider.value);
        int rockCount = Mathf.RoundToInt(rockSlider.value);

        resourceSpawner.SpawnResourcesInGrid(woodCount, rockCount);
    }

    private void RespawnResourcesRadial()
    {
        int woodCount = Mathf.RoundToInt(woodSlider.value);
        int rockCount = Mathf.RoundToInt(rockSlider.value);

        resourceSpawner.SpawnResourcesRadial(woodCount, rockCount);
    }
}
