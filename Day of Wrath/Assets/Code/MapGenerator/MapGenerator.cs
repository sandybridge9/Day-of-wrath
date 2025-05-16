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
        var offsetX = Random.Range(0f, 9999f);
        var offsetY = Random.Range(0f, 9999f);

        var heights = new float[resolution, resolution];

        for (var x = 0; x < resolution; x++)
        {
            for (var y = 0; y < resolution; y++)
            {
                var xCoord = (float)x / resolution * scale + offsetX;
                var yCoord = (float)y / resolution * scale + offsetY;

                var sample = Mathf.PerlinNoise(xCoord, yCoord);
                heights[x, y] = sample;
            }
        }

        ApplyHeights(heights);
    }

    public void GenerateWithSimplex()
    {
        var offsetX = Random.Range(0f, 9999f);
        var offsetY = Random.Range(0f, 9999f);

        var heights = new float[resolution, resolution];

        for (var x = 0; x < resolution; x++)
        {
            for (var y = 0; y < resolution; y++)
            {
                var xCoord = (float)x / resolution * scale + offsetX;
                var yCoord = (float)y / resolution * scale + offsetY;

                var sample = SimplexNoise.Noise(xCoord, yCoord);
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
        var heights = new float[resolution, resolution];

        var offsetX = Random.Range(0f, 9999f);
        var offsetY = Random.Range(0f, 9999f);

        var hillCenter = new Vector2(Random.Range(resolution * 0.2f, resolution * 0.8f), Random.Range(resolution * 0.2f, resolution * 0.8f));
        var hillRadius = Random.Range(resolution * 0.25f, resolution * 0.35f);

        var valleyCenter = new Vector2(Random.Range(resolution * 0.2f, resolution * 0.8f), Random.Range(resolution * 0.2f, resolution * 0.8f));
        var valleyRadius = Random.Range(resolution * 0.25f, resolution * 0.35f);

        var craterCenter = new Vector2(Random.Range(resolution * 0.2f, resolution * 0.8f), Random.Range(resolution * 0.2f, resolution * 0.8f));
        var craterRadius = Random.Range(resolution * 0.25f, resolution * 0.35f);

        var ridgeAngle = Random.Range(0f, Mathf.PI);

        for (var x = 0; x < resolution; x++)
        {
            for (var y = 0; y < resolution; y++)
            {
                var nx = (float)x / resolution;
                var ny = (float)y / resolution;
                var pos = new Vector2(x, y);

                var noise = 1f;

                if (usePerlin)
                    noise = Mathf.PerlinNoise(x * 0.03f + offsetX, y * 0.03f + offsetY);
                else if (useSimplex)
                    noise = (SimplexNoise.Noise(x * 0.03f + offsetX, y * 0.03f + offsetY) + 1f) / 2f;

                var height = 0f;
                var blendSum = 0f;

                if (hill)
                {
                    var dist = Vector2.Distance(pos, hillCenter);
                    var shape = Mathf.Clamp01(1f - dist / hillRadius);
                    shape = Mathf.Pow(shape, 2.5f); // steeper
                    height += shape * hillIntensity * noise;
                    blendSum += shape;
                }

                if (valley)
                {
                    var dist = Vector2.Distance(pos, valleyCenter);
                    var shape = Mathf.Clamp01(1f - dist / valleyRadius);
                    shape = Mathf.Pow(shape, 2.5f);
                    height -= shape * valleyIntensity * noise;
                    blendSum += shape;
                }

                if (crater)
                {
                    var dist = Vector2.Distance(pos, craterCenter);
                    var shape = Mathf.Clamp01(1f - dist / craterRadius);
                    shape = Mathf.Pow(shape, 2.0f);
                    var ring = Mathf.Abs(Mathf.Sin(dist / craterRadius * Mathf.PI));
                    height -= ring * craterIntensity * noise;
                    blendSum += shape;
                }

                // Mountain Ridge
                if (mountains)
                {
                    var projection = nx * Mathf.Cos(ridgeAngle) + ny * Mathf.Sin(ridgeAngle);
                    var ridge = Mathf.Sin(projection * 10f);
                    var fade = Mathf.Clamp01(1f - Mathf.Abs(ny - 0.5f) * 2f);
                    var shape = Mathf.Pow(Mathf.Clamp01(ridge), 2.5f) * fade;
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

        var centerOffsetX = -terrainWidth / 2f;
        var centerOffsetZ = -terrainLength / 2f;
        terrain.transform.position = new Vector3(centerOffsetX, 0, centerOffsetZ);

        runtimeTerrainData.SetHeights(0, 0, heights);
    }

    public void FlattenTerrain()
    {
        var flat = new float[resolution, resolution];

        for (var x = 0; x < resolution; x++)
        {
            for (var y = 0; y < resolution; y++)
            {
                flat[x, y] = 0;
            }
        }

        ApplyHeights(flat);
    }

    public float CalculateUsableTerrainPercentage(float maxSlope = 30f, int resolution = 64)
    {
        var terrainData = terrain.terrainData;
        var terrainSize = terrainData.size;

        var usableCount = 0;
        var totalCount = resolution * resolution;

        for (var x = 0; x < resolution; x++)
        {
            for (var z = 0; z < resolution; z++)
            {
                var normX = (float)x / (resolution - 1);
                var normZ = (float)z / (resolution - 1);

                var normal = terrainData.GetInterpolatedNormal(normX, normZ);
                var slope = Vector3.Angle(normal, Vector3.up);

                if (slope <= maxSlope)
                {
                    usableCount++;
                }
            }
        }

        return (float)usableCount / totalCount * 100f;
    }
}
