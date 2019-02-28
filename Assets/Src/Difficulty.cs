public static class DifficultyUtils
{
    public static int Multiply(Difficulty d, int value)
    {
        return value * GetModifier(d);
    }
    public static float Multiply(Difficulty d, float value)
    {
        return value * GetModifier(d);
    }

    public static int GetModifier(Difficulty d)
    {
        return ((int)d + 1);
    }
}
public enum Difficulty
{
    Easy,
    Normal,
    Hard
}