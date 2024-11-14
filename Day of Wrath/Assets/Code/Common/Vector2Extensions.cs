using UnityEngine;

public static class Vector2Extensions
{
    public static Vector2 SwapXAndY(this Vector2 vector)
        =>  new(vector.y, vector.x);

    public static Vector3 Vector3(this Vector2 vector, float z = 0)
        => new(vector.x, vector.y, z);

    public static bool HasPassedDistanceThreshold(this Vector2 vector1, Vector2 vector2, float threshold)
        => Vector2.Distance(vector1, vector2) > threshold;
}
