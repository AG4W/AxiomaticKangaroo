using UnityEngine;

using System.Collections.Generic;

public static class Extensions
{
    public static Vector3 GetCenterVector3(Vector3[] array)
    {
        Vector3 c = Vector3.zero;

        for (int i = 0; i < array.Length; i++)
            c += array[i];

        c /= array.Length;

        return c;
    }
    public static Vector3 Perpendicular(this Vector3 direction, Vector3 up)
    {
        return Vector3.Cross(direction, up).normalized;
    }
    public static Vector3 CalculateInterceptCourse(Vector3 aTargetPos, Vector3 aTargetSpeed, Vector3 aInterceptorPos, float aInterceptorSpeed)
    {
        Vector3 targetDir = aTargetPos - aInterceptorPos;
        float iSpeed2 = aInterceptorSpeed * aInterceptorSpeed;
        float tSpeed2 = aTargetSpeed.sqrMagnitude;
        float fDot1 = Vector3.Dot(targetDir, aTargetSpeed);
        float targetDist2 = targetDir.sqrMagnitude;
        float d = (fDot1 * fDot1) - targetDist2 * (tSpeed2 - iSpeed2);
        if (d < 0.1f)  // negative == no possible course because the interceptor isn't fast enough
            return Vector3.zero;
        float sqrt = Mathf.Sqrt(d);
        float S1 = (-fDot1 - sqrt) / targetDist2;
        float S2 = (-fDot1 + sqrt) / targetDist2;
        if (S1 < 0.0001f)
        {
            if (S2 < 0.0001f)
                return Vector3.zero;
            else
                return (S2) * targetDir + aTargetSpeed;
        }
        else if (S2 < 0.0001f)
            return (S1) * targetDir + aTargetSpeed;
        else if (S1 < S2)
            return (S2) * targetDir + aTargetSpeed;
        else
            return (S1) * targetDir + aTargetSpeed;
    }
    public static Vector3 DirectionFromAngle(float angleInDegrees)
    {
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public static T RandomItem<T>(this T[] array)
    {
        return array[Random.Range(0, array.Length)];
    }
    public static int RandomIndex<T>(this T[] array)
    {
        return Random.Range(0, array.Length);
    }
    public static bool IsInRange<T>(this T[] array, int index)
    {
        return index >= 0 && index < array.Length;
    }

    public static T RandomItem<T>(this List<T> list)
    {
        return list[Random.Range(0, list.Count)];
    }

    public static float NextFloat(this System.Random random, float minimum, float maximum)
    {
        return (float)random.NextDouble() * (maximum - minimum) + minimum;
    }

    static List<string> romanNumerals = new List<string>() { "M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I" };
    static List<int> numerals = new List<int>() { 1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1 };

    public static string ToRomanNumeral(this int number)
    {
        var romanNumeral = string.Empty;
        while (number > 0)
        {
            // find biggest numeral that is less than equal to number
            var index = numerals.FindIndex(x => x <= number);
            // subtract it's value from your number
            number -= numerals[index];
            // tack it onto the end of your roman numeral
            romanNumeral += romanNumerals[index];
        }
        return romanNumeral;
    }

    public static string ToRichText(this Color color)
    {
        return "<color=#" + ColorUtility.ToHtmlStringRGB(color) + ">";
    }

    public static ShipComponentRarity GetLootRarity(ShipComponentRarity rarityCap, float roll)
    {
        Debug.Assert(roll <= 1f && roll >= 0f, "Wrong roll values");
        ShipComponentRarity prospect = ShipComponentRarity.Common;

        if (roll <= .01f)
            prospect = ShipComponentRarity.Artifact;
        else if (roll <= .15f)
            prospect = ShipComponentRarity.Exotic;
        else
            prospect = ShipComponentRarity.Common;

        if (prospect >= rarityCap)
            return rarityCap;

        return prospect;
    }
}