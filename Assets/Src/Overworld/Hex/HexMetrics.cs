using UnityEngine;

public static class HexMetrics
{
    const int OUTER_RADIUS = 50;
    const float VISIBLE_PERCENTAGE = .975f;

    static Vector3[] _corners =
    {
        new Vector3(0f, 0f, outerRadius),
        new Vector3(innerRadius, 0f, .5f * outerRadius),
        new Vector3(innerRadius, 0f, -.5f * outerRadius),
        new Vector3(0f, 0f, -outerRadius),
        new Vector3(-innerRadius, 0f, -.5f * outerRadius),
        new Vector3(-innerRadius, 0f, .5f * outerRadius),
        new Vector3(0f, 0f, outerRadius),
    };

    public static int outerRadius { get { return OUTER_RADIUS; } }
    public static float innerRadius { get { return (Mathf.Sqrt(3) / 2) * OUTER_RADIUS; } }

    public static float visiblePercentage { get { return VISIBLE_PERCENTAGE; } }

    public static Vector3[] corners { get { return _corners; } }
}
