using UnityEngine;

public struct Hex2
{
    public readonly int x;
    public readonly int z;

    public Hex2(int x, int z)
    {
        this.x = x;
        this.z = z;
    }

    public static Hex2 FromOffsetCoordinates(int x, int z)
    {
        return new Hex2(x, z);
    }
    public static Vector3 ToWorld(Hex2 hex)
    {
        return new Vector3();
    }
}
