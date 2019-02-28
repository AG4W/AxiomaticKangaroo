using UnityEngine;

public static class FormationManager
{
    static int[] _formationDensity = new int[] { 10, 20, 30 };
    static FormationType _type = FormationType.Arrow;
    static FormationDensity _density = FormationDensity.Medium;

    public static void SetType(FormationType type)
    {
        _type = type;
    }
    public static void SetDensity(FormationDensity density)
    {
        _density = density;
    }

    public static Vector3[] Get(Vector3 origin, Vector3 forward, int shipCount)
    {
        switch (_type)
        {
            case FormationType.Line:
                return Line(origin, forward, shipCount);
            case FormationType.File:
                return File(origin, forward, shipCount);
            case FormationType.Tower:
                return Tower(origin, forward, shipCount);
            case FormationType.Arrow:
                return Arrow(origin, forward, shipCount);
            default:
                return Line(origin, forward, shipCount);
        }
    }
    static Vector3[] Line(Vector3 origin, Vector3 forward, int shipCount)
    {
        Vector3[] r = new Vector3[shipCount];

        for (int i = 0; i < shipCount; i++)
        {
            r[i] = origin + (-forward.Perpendicular(Vector3.up).normalized * (i * GetFormationDensity()));
            r[i].y = origin.y;
        }

        return r;
    }
    static Vector3[] File(Vector3 origin, Vector3 forward, int shipCount)
    {
        Vector3[] r = new Vector3[shipCount];

        for (int i = 0; i < shipCount; i++)
        {
            r[i] = origin + (-forward.normalized * (i * GetFormationDensity()));
            r[i].y = origin.y;
        }

        return r;
    }
    static Vector3[] Tower(Vector3 origin, Vector3 forward, int shipCount)
    {
        Vector3[] r = new Vector3[shipCount];

        for (int i = 0; i < shipCount; i++)
            r[i] = origin + (forward.Perpendicular(Vector3.right).normalized * (i * GetFormationDensity()));

        return r;
    }
    static Vector3[] Arrow(Vector3 origin, Vector3 forward, int shipCount)
    {
        int step = 1;
        bool toLeft;

        Vector3[] r = new Vector3[shipCount];

        r[0] = origin;

        for (int i = 1; i < shipCount; i++)
        {
            //check which side to spawn on
            toLeft = i % 2 != 0;

            r[i] = origin;
            //add sideways offset
            r[i] += (toLeft ? -forward : forward).Perpendicular(Vector3.up).normalized * (step * GetFormationDensity());
            //add backwards offset
            r[i] += -forward.normalized * (step * GetFormationDensity());
            r[i].y = origin.y;

            //increment step after every right offset
            if (!toLeft)
                step++;
        }

        return r;
    }

    static int GetFormationDensity()
    {
        return _formationDensity[(int)_density];
    }
}
public enum FormationType
{
    Line,
    File,
    Tower,
    Arrow
}
public enum FormationDensity
{
    Tight,
    Medium,
    Loose
}
