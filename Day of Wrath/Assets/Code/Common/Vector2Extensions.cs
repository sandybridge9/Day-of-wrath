using UnityEngine;

public static class Vector2Extensions
{
    public static Vector2 SwapXAndY(this Vector2 vector)
        =>  new Vector2(vector.y, vector.x);

    public static Vector2 Vector3(this Vector2 vector, float z = 0)
        => new Vector3(vector.x, vector.y, z);
}
