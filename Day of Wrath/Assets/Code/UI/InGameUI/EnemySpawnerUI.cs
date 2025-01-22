using UnityEngine;
using UnityEngine.UI;

public class EnemySpawnerUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Button spawnEnemyUnitButton;
    public Button spawnEnemyTownhallButton;

    [Header("Spawner Reference")]
    public SpawnEnemyController spawnEnemyController;

    private void Start()
    {
        // Assign button click listeners
        spawnEnemyUnitButton.onClick.AddListener(() => OnSpawnEnemyButtonClicked(spawnEnemyController.enemyUnitPrefab));
        spawnEnemyTownhallButton.onClick.AddListener(() => OnSpawnEnemyButtonClicked(spawnEnemyController.enemyTownhallPrefab));
    }

    private void OnSpawnEnemyButtonClicked(GameObject prefab)
    {
        spawnEnemyController.SetPrefabToSpawn(prefab);
    }
}
