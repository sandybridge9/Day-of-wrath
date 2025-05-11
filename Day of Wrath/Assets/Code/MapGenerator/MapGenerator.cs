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

        Vector2 hillCenter = new Vector2(Random.Range(resolution * 0.2f, resolution * 0.8f), Random.Range(resolution * 0.2f, resolution * 0.8f));
        float hillRadius = Random.Range(resolution * 0.25f, resolution * 0.35f);

        Vector2 valleyCenter = new Vector2(Random.Range(resolution * 0.2f, resolution * 0.8f), Random.Range(resolution * 0.2f, resolution * 0.8f));
        float valleyRadius = Random.Range(resolution * 0.25f, resolution * 0.35f);

        Vector2 craterCenter = new Vector2(Random.Range(resolution * 0.2f, resolution * 0.8f), Random.Range(resolution * 0.2f, resolution * 0.8f));
        float craterRadius = Random.Range(resolution * 0.25f, resolution * 0.35f);

        float ridgeAngle = Random.Range(0f, Mathf.PI);

        for (int x = 0; x < resolution; x++)
        {
            for (int y = 0; y < resolution; y++)
            {
                float nx = (float)x / resolution;
                float ny = (float)y / resolution;
                Vector2 pos = new Vector2(x, y);

                float noise = 1f;
                if (usePerlin)
                    noise = Mathf.PerlinNoise(x * 0.03f + offsetX, y * 0.03f + offsetY);
                else if (useSimplex)
                    noise = (SimplexNoise.Noise(x * 0.03f + offsetX, y * 0.03f + offsetY) + 1f) / 2f;

                float height = 0f;
                float blendSum = 0f;

                // Hill
                if (hill)
                {
                    float dist = Vector2.Distance(pos, hillCenter);
                    float shape = Mathf.Clamp01(1f - dist / hillRadius);
                    shape = Mathf.Pow(shape, 2.5f); // steeper
                    height += shape * hillIntensity * noise;
                    blendSum += shape;
                }

                // Valley
                if (valley)
                {
                    float dist = Vector2.Distance(pos, valleyCenter);
                    float shape = Mathf.Clamp01(1f - dist / valleyRadius);
                    shape = Mathf.Pow(shape, 2.5f);
                    height -= shape * valleyIntensity * noise;
                    blendSum += shape;
                }

                // Crater
                if (crater)
                {
                    float dist = Vector2.Distance(pos, craterCenter);
                    float shape = Mathf.Clamp01(1f - dist / craterRadius);
                    shape = Mathf.Pow(shape, 2.0f);
                    float ring = Mathf.Abs(Mathf.Sin(dist / craterRadius * Mathf.PI));
                    height -= ring * craterIntensity * noise;
                    blendSum += shape;
                }

                // Mountain Ridge
                if (mountains)
                {
                    float projection = nx * Mathf.Cos(ridgeAngle) + ny * Mathf.Sin(ridgeAngle);
                    float ridge = Mathf.Sin(projection * 10f);
                    float fade = Mathf.Clamp01(1f - Mathf.Abs(ny - 0.5f) * 2f);
                    float shape = Mathf.Pow(Mathf.Clamp01(ridge), 2.5f) * fade;
                    height += shape * mountainIntensity * noise;
                    blendSum += shape;
                }

                // Normalize blend
                if (blendSum > 1f)
                    height /= blendSum;

                heights[x, y] = Mathf.Clamp01(0.5f + height); // baseline at 0.5
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
