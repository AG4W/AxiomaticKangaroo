using UnityEngine;

using System.Linq;

[CreateAssetMenu(menuName = "Data/Parts/Utilities/Resource Scanner")]
public class ResourceScannerData : UtilityData
{
    [Range(500f, 10000f)][SerializeField]float _range = 2000f;
    public float range { get { return _range; } }

    public override ShipComponent Instantiate()
    {
        return new ResourceScanner(this);
    }
}
public class ResourceScanner : Utility
{
    float _range;
    public float range { get { return _range; } }

    public ResourceScanner(ResourceScannerData rsd) : base(rsd)
    {
        _range = rsd.range;
    }

    public override void DrawVisualization()
    {
        int segments = 80;
        Vector3[] positions = new Vector3[segments];

        float angle = 20f;

        for (int i = 0; i < segments; i++)
        {
            positions[i] = new Vector3(Mathf.Sin(Mathf.Deg2Rad * angle) * _range, 0, Mathf.Cos(Mathf.Deg2Rad * angle) * _range);
            angle += (360f / segments);
        }

        Visualizer.getInstance.DrawLine(base.owner.gameObject, positions, Color.yellow, true);
    }
}
