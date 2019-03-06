using UnityEngine;

//EVEN-Q vertical layout for grid
public static class HexMetrics
{
    const int SIZE = 25;
    const float VISIBLE_PERCENTAGE = .975f;

    public static int size { get { return SIZE; } }

    public static float width { get { return 2 * SIZE; } }
    public static float height { get { return Mathf.Sqrt(3) * SIZE; } }

    public static float visiblePercentage { get { return VISIBLE_PERCENTAGE; } }
    
    public static Vector3 GetHexagonPoint(int index)
    {
        float d = 60 * index;
        float r = (Mathf.PI / 180) * d;

        return new Vector3(SIZE * Mathf.Cos(r), 0f, SIZE * Mathf.Sin(r));
    }
}
