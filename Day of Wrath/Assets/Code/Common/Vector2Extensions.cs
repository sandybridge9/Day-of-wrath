using UnityEngine;

public static class Vector2Extensions
{
    public static Vector2 SwapXAndY(this Vector2 vector)
        =>  new Vector2(vector.y, vector.x);
}
