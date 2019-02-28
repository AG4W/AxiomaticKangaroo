public static class ProgressData
{
    static byte[] _points;

    public static void Reset()
    {
        _points = new byte[System.Enum.GetNames(typeof(ProgressPoint)).Length];

        for (int i = 0; i < _points.Length; i++)
            _points[i] = 0;
    }

    public static bool Evaluate(ProgressPoint pp)
    {
        return _points[(int)pp] == 1;
    }
    public static void Set(ProgressPoint pp, bool value)
    {
        _points[(int)pp] = (byte)(value ? 1 : 0);
    }
}
public enum ProgressPoint
{
    ReceivedStartEvent,
    EnteredWormhole
}
