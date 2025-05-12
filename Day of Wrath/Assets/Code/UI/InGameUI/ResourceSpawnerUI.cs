using UnityEngine;
using UnityEngine.UI;

public class ResourceSpawnerUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider woodSlider;
    public Slider rockSlider;
    public Slider widthSlider;
    public Slider lengthSlider;
    public Button respawnResourcesInClustersButton;
    public Button respawnResourcesInGridButton;
    public Button respawnResourcesRadialButton;
    public Button clearSpawnedResourcesButton;

    [Header("Spawner Reference")]
    public ResourceSpawner resourceSpawner;


    private void Start()
    {
        woodSlider.onValueChanged.AddListener(UpdateSliders);
        rockSlider.onValueChanged.AddListener(UpdateSliders);
        widthSlider.onValueChanged.AddListener(OnWidthChanged);
        lengthSlider.onValueChanged.AddListener(OnHeightChanged);

        respawnResourcesInClustersButton.onClick.AddListener(RespawnResourcesInClusters);
        respawnResourcesInGridButton.onClick.AddListener(RespawnResourcesInGrid);
        respawnResourcesRadialButton.onClick.AddListener(RespawnResourcesRadial);
        clearSpawnedResourcesButton.onClick.AddListener(ClearResources);

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

    private void OnWidthChanged(float value)
    {
        resourceSpawner.mapSize = new Vector2(Mathf.RoundToInt(value), resourceSpawner.mapSize.y);
    }

    private void OnHeightChanged(float value)
    {
        resourceSpawner.mapSize = new Vector2(resourceSpawner.mapSize.x, Mathf.RoundToInt(value));
    }

    private void ClearResources()
    {
        resourceSpawner.ClearResources();
    }
}
