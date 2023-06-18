using UnityEngine;

public static class Vector3Extensions
{
    public static Vector2 GetVector2InspectorAnglesFromEulerAngles(this Vector3 eulerAngles)
    {
        Debug.Log(eulerAngles);

        float xAngle;
        float yAngle;

        if (eulerAngles.x <= 180f)
        {
            xAngle = eulerAngles.x;
        }
        else
        {
            xAngle = eulerAngles.x - 360f;
        }

        if (eulerAngles.y <= 180f)
        {
            yAngle = eulerAngles.y;
        }
        else
        {
            yAngle = eulerAngles.y - 360f;
        }

        return new Vector2(xAngle, yAngle);
    }
}
