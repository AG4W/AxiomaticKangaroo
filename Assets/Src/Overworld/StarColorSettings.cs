using UnityEngine;

[CreateAssetMenu(menuName ="Misc/Star Color Settings")]
public class StarColorSettings : ScriptableObject
{
    [SerializeField]Gradient _surfaces;
    [SerializeField]Gradient _reflections;

    public Color GetSurface(float index)
    {
        return _surfaces.Evaluate(index);
    }
    public Color GetReflection(float index)
    {
        return _reflections.Evaluate(index);
    }
}
