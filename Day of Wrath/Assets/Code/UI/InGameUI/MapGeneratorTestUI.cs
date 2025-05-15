using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class MapGeneratorTestUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Button perlinClustersButton;
    public Button perlinGridButton;
    public Button perlinRadialButton;

    public Button simplexClustersButton;
    public Button simplexGridButton;
    public Button simplexRadialButton;

    [Header("Map Generator Reference")]
    public MapGenerator mapGenerator;

    [Header("Resource Spawner Reference")]
    public ResourceSpawner resourceSpawner;

    private const float HeightMultiplier = 10f;
    private const int MapWidth = 512;
    private const int MapLength = 512;

    private const int WoodCount = 512;
    private const int RockCount = 512;

    private void Start()
    {
        perlinClustersButton.onClick.AddListener(GeneratePerlinClusters);
        perlinGridButton.onClick.AddListener(GeneratePerlinGrid);
        perlinRadialButton.onClick.AddListener(GeneratePerlinRadial);

        simplexClustersButton.onClick.AddListener(GenerateSimplexClusters);
        simplexGridButton.onClick.AddListener(GenerateSimplexGrid);
        simplexRadialButton.onClick.AddListener(GenerateSimplexRadial);

        mapGenerator.heightMultiplier = HeightMultiplier;
        mapGenerator.terrainLength = MapLength;
        mapGenerator.terrainWidth = MapWidth;

        resourceSpawner.defaultRockCount = RockCount;
        resourceSpawner.defaultWoodCount = WoodCount;
        resourceSpawner.mapSize = new Vector2(MapWidth, MapLength);
    }

    private void GeneratePerlinClusters()
    {
        var stopwatch = new Stopwatch();

        stopwatch.Start();
        mapGenerator.GenerateWithPerlin();
        var mapGenerationTime = stopwatch.ElapsedMilliseconds;
        stopwatch.Stop();
        stopwatch.Reset();

        stopwatch.Start();
        resourceSpawner.SpawnResourcesInClusters(WoodCount, RockCount);
        var resourceSpawnerTime = stopwatch.ElapsedMilliseconds;
        stopwatch.Stop();

        var resourceCount = resourceSpawner.spawnedResources.Count;

        UnityEngine.Debug.Log($"[Perlin][Clusters] Map generation time: {mapGenerationTime}ms; Resource generation time: {resourceSpawnerTime}ms; Generated resource count: {resourceCount}.");
    }

    private void GeneratePerlinGrid()
    {
        var stopwatch = new Stopwatch();

        stopwatch.Start();
        mapGenerator.GenerateWithPerlin();
        var mapGenerationTime = stopwatch.ElapsedMilliseconds;
        stopwatch.Stop();
        stopwatch.Reset();

        stopwatch.Start();
        resourceSpawner.SpawnResourcesInGrid(WoodCount, RockCount);
        var resourceSpawnerTime = stopwatch.ElapsedMilliseconds;
        stopwatch.Stop();

        var resourceCount = resourceSpawner.spawnedResources.Count;

        UnityEngine.Debug.Log($"[Perlin][Grid] Map generation time: {mapGenerationTime}ms; Resource generation time: {resourceSpawnerTime}ms; Generated resource count: {resourceCount}.");
    }

    private void GeneratePerlinRadial()
    {
        var stopwatch = new Stopwatch();

        stopwatch.Start();
        mapGenerator.GenerateWithPerlin();
        var mapGenerationTime = stopwatch.ElapsedMilliseconds;
        stopwatch.Stop();
        stopwatch.Reset();

        stopwatch.Start();
        resourceSpawner.SpawnResourcesRadial(WoodCount, RockCount);
        var resourceSpawnerTime = stopwatch.ElapsedMilliseconds;
        stopwatch.Stop();

        var resourceCount = resourceSpawner.spawnedResources.Count;

        UnityEngine.Debug.Log($"[Perlin][Radial] Map generation time: {mapGenerationTime}ms; Resource generation time: {resourceSpawnerTime}ms; Generated resource count: {resourceCount}.");
    }

    private void GenerateSimplexClusters()
    {
        var stopwatch = new Stopwatch();

        stopwatch.Start();
        mapGenerator.GenerateWithSimplex();
        var mapGenerationTime = stopwatch.ElapsedMilliseconds;
        stopwatch.Stop();
        stopwatch.Reset();

        stopwatch.Start();
        resourceSpawner.SpawnResourcesInClusters(WoodCount, RockCount);
        var resourceSpawnerTime = stopwatch.ElapsedMilliseconds;
        stopwatch.Stop();

        var resourceCount = resourceSpawner.spawnedResources.Count;

        UnityEngine.Debug.Log($"[Simplex][Clusters] Map generation time: {mapGenerationTime}ms; Resource generation time: {resourceSpawnerTime}ms; Generated resource count: {resourceCount}.");
    }

    private void GenerateSimplexGrid()
    {
        var stopwatch = new Stopwatch();

        stopwatch.Start();
        mapGenerator.GenerateWithSimplex();
        var mapGenerationTime = stopwatch.ElapsedMilliseconds;
        stopwatch.Stop();
        stopwatch.Reset();

        stopwatch.Start();
        resourceSpawner.SpawnResourcesInGrid(WoodCount, RockCount);
        var resourceSpawnerTime = stopwatch.ElapsedMilliseconds;
        stopwatch.Stop();

        var resourceCount = resourceSpawner.spawnedResources.Count;

        UnityEngine.Debug.Log($"[Simplex][Grid] Map generation time: {mapGenerationTime}ms; Resource generation time: {resourceSpawnerTime}ms; Generated resource count: {resourceCount}.");
    }

    private void GenerateSimplexRadial()
    {
        var stopwatch = new Stopwatch();

        stopwatch.Start();
        mapGenerator.GenerateWithSimplex();
        var mapGenerationTime = stopwatch.ElapsedMilliseconds;
        stopwatch.Stop();
        stopwatch.Reset();

        stopwatch.Start();
        resourceSpawner.SpawnResourcesRadial(WoodCount, RockCount);
        var resourceSpawnerTime = stopwatch.ElapsedMilliseconds;
        stopwatch.Stop();

        var resourceCount = resourceSpawner.spawnedResources.Count;

        UnityEngine.Debug.Log($"[Simplex][Radial] Map generation time: {mapGenerationTime}ms; Resource generation time: {resourceSpawnerTime}ms; Generated resource count: {resourceCount}.");
    }

    private void GenerateMapUsingSimplexNoise()
    {
        mapGenerator.GenerateWithSimplex();
    }
}
