using UnityEngine;

[RequireComponent(typeof(Terrain))]
public class MapGenerator : MonoBehaviour
{
    [Header("Map Settings")]
    public int terrainWidth = 500;     // world space
    public int terrainLength = 500;    // world space
    public int resolution = 512;       // heightmap resolution (must be square!)
    public float scale = 20f;
    public float heightMultiplier = 10f;

    [Header("Target Terrain")]
    public Terrain terrain;

    private TerrainData originalTerrainData;
    private TerrainData runtimeTerrainData;

    private void Start()
    {
        if (terrain == null)
            terrain = GetComponent<Terrain>();

        originalTerrainData = terrain.terrainData;

        runtimeTerrainData = Instantiate(originalTerrainData);
        terrain.terrainData = runtimeTerrainData;

        FlattenTerrain();
    }

    private void OnDestroy()
    {
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

        float[,] heights = new float[resolution, resolution];

        for (int x = 0; x < resolution; x++)
        {
            for (int y = 0; y < resolution; y++)
            {
                float xCoord = (float)x / resolution * scale + offsetX;
                float yCoord = (float)y / resolution * scale + offsetY;

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

        float[,] heights = new float[resolution, resolution];

        for (int x = 0; x < resolution; x++)
        {
            for (int y = 0; y < resolution; y++)
            {
                float xCoord = (float)x / resolution * scale + offsetX;
                float yCoord = (float)y / resolution * scale + offsetY;

                float sample = SimplexNoise.Noise(xCoord, yCoord);
                sample = (sample + 1f) / 2f;
                heights[x, y] = sample;
            }
        }

        ApplyHeights(heights);
    }

    public void GenerateFromCheckboxes(
        bool usePerlin, bool useSimplex,
        bool hill, bool valley, bool crater, bool mountains,
        float hillIntensity, float valleyIntensity, float craterIntensity, float mountainIntensity)
    {
        float[,] heights = new float[resolution, resolution];

        float offsetX = Random.Range(0f, 9999f);
        float offsetY = Random.Range(0f, 9999f);

        // Randomized positions for features
        Vector2 hillCenter = new Vector2(Random.Range(resolution * 0.2f, resolution * 0.8f), Random.Range(resolution * 0.2f, resolution * 0.8f));
        float hillRadius = Random.Range(resolution * 0.2f, resolution * 0.35f);

        Vector2 valleyCenter = new Vector2(Random.Range(resolution * 0.2f, resolution * 0.8f), Random.Range(resolution * 0.2f, resolution * 0.8f));
        float valleyRadius = Random.Range(resolution * 0.2f, resolution * 0.35f);

        Vector2 craterCenter = new Vector2(Random.Range(resolution * 0.2f, resolution * 0.8f), Random.Range(resolution * 0.2f, resolution * 0.8f));
        float craterRadius = Random.Range(resolution * 0.2f, resolution * 0.35f);

        float ridgeAngle = Random.Range(0f, Mathf.PI);

        for (int x = 0; x < resolution; x++)
        {
            for (int y = 0; y < resolution; y++)
            {
                float height = 0f;
                float nx = (float)x / resolution;
                float ny = (float)y / resolution;

                // Base noise
                float noise = 1f;
                if (usePerlin)
                    noise = Mathf.PerlinNoise(x * 0.05f + offsetX, y * 0.05f + offsetY);
                else if (useSimplex)
                    noise = (SimplexNoise.Noise(x * 0.05f + offsetX, y * 0.05f + offsetY) + 1f) / 2f;

                Vector2 pos = new Vector2(x, y);

                // Hill
                if (hill)
                {
                    float dist = Vector2.Distance(pos, hillCenter);
                    float normDist = Mathf.Clamp01(dist / hillRadius);
                    float shape = 1f - normDist;
                    height += hillIntensity * shape * shape * noise;
                }

                // Valley
                if (valley)
                {
                    float dist = Vector2.Distance(pos, valleyCenter);
                    float normDist = Mathf.Clamp01(dist / valleyRadius);
                    float shape = normDist;
                    height += valleyIntensity * shape * shape * noise;
                }

                // Crater
                if (crater)
                {
                    float dist = Vector2.Distance(pos, craterCenter);
                    float normDist = Mathf.Clamp01(dist / craterRadius);
                    float shape = normDist * normDist;
                    height += craterIntensity * shape * noise;
                }

                // Mountains
                if (mountains)
                {
                    float projection = nx * Mathf.Cos(ridgeAngle) + ny * Mathf.Sin(ridgeAngle);
                    float ridge = Mathf.Sin(projection * 10f);
                    float fade = 1f - Mathf.Abs(ny - 0.5f) * 2f;
                    height += mountainIntensity * Mathf.Clamp01(ridge * fade * noise);
                }

                // Final clamping/normalization
                if (height > 1f)
                    height /= 1.5f;

                heights[x, y] = Mathf.Clamp01(height);
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

        runtimeTerrainData.heightmapResolution = resolution;
        runtimeTerrainData.size = new Vector3(terrainWidth, heightMultiplier, terrainLength);
        runtimeTerrainData.SetHeights(0, 0, heights);
    }

    public void FlattenTerrain()
    {
        float[,] flat = new float[resolution, resolution];
        for (int x = 0; x < resolution; x++)
            for (int y = 0; y < resolution; y++)
                flat[x, y] = 0f;

        ApplyHeights(flat);
    }
}
