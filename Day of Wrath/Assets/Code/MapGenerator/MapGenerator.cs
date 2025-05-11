using UnityEngine;

[RequireComponent(typeof(Terrain))]
public class MapGenerator : MonoBehaviour
{
    [Header("Map Settings")]
    public int width = 500;
    public int height = 500;
    public float scale = 20f;
    public float heightMultiplier = 10f;

    [Header("Target Terrain")]
    public Terrain terrain;

    private TerrainData originalTerrainData;     // The asset in the project
    private TerrainData runtimeTerrainData;      // Cloned instance used at runtime

    private void Start()
    {
        if (terrain == null)
            terrain = GetComponent<Terrain>();

        // Backup original data so we can restore after Play mode
        originalTerrainData = terrain.terrainData;

        // Clone the terrain data so Play Mode changes don't persist
        runtimeTerrainData = Instantiate(originalTerrainData);
        terrain.terrainData = runtimeTerrainData;

        // Optionally flatten the runtime terrain at start
        //FlattenTerrain();
    }

    private void OnDestroy()
    {
        // Restore the original terrain data when exiting Play mode
#if UNITY_EDITOR
        if (!Application.isPlaying && terrain != null)
        {
            terrain.terrainData = originalTerrainData;
        }
#endif
    }

    public void GenerateWithPerlin()
    {
        float offsetX = Random.Range(0f, 9999f);
        float offsetY = Random.Range(0f, 9999f);

        float[,] heights = new float[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float xCoord = (float)x / width * scale + offsetX;
                float yCoord = (float)y / height * scale + offsetY;

                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                heights[x, y] = sample;
            }
        }

        ApplyHeights(heights);
    }

    public void GenerateWithSimplex()
    {
        float offsetX = Random.Range(0f, 9999f);
        float offsetY = Random.Range(0f, 9999f);

        float[,] heights = new float[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float xCoord = (float)x / width * scale + offsetX;
                float yCoord = (float)y / height * scale + offsetY;

                float sample = SimplexNoise.Noise(xCoord, yCoord);
                sample = (sample + 1f) / 2f; // Normalize from [-1,1] to [0,1]
                heights[x, y] = sample;
            }
        }

        ApplyHeights(heights);
    }

    private void ApplyHeights(float[,] heights)
    {
        if (runtimeTerrainData == null)
        {
            Debug.LogError("Runtime TerrainData is null. Cannot apply heights.");
            return;
        }

        runtimeTerrainData.heightmapResolution = width + 1;
        runtimeTerrainData.size = new Vector3(width, heightMultiplier, height);
        runtimeTerrainData.SetHeights(0, 0, heights);
    }

    public void FlattenTerrain()
    {
        float[,] flat = new float[width, height];
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                flat[x, y] = 0f;

        ApplyHeights(flat);
    }
}
