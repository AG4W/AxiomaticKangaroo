using UnityEngine;

using System.Collections;

[CreateAssetMenu(menuName = "Data/Parts/Utilities/Mining Laser")]
public class ResourceGatheringUtilityData : UtilityData
{
    [SerializeField]float _range = 150f;
    [SerializeField]float _duration = 30f;
    [Range(.001f, 10f)][SerializeField]float _extractionAmount = .1f;

    [SerializeField]GameObject _vfxPrefab;

    public float range { get { return _range; } }
    public float duration { get { return _duration; } }
    public float extractionAmount { get { return _extractionAmount; } }

    public GameObject vfxPrefab { get { return _vfxPrefab; } }

    public override ShipComponent Instantiate()
    {
        return new ResourceGatheringUtility(this);
    }
}
public class ResourceGatheringUtility : Utility
{
    float _range;
    float _duration;
    float _extractionAmount;

    GameObject _vfxPrefab;

    ResourceEntity _target;

    public float range { get { return _range; } }
    public float duration { get { return _duration; } }
    public float extractionAmount { get { return _extractionAmount; } }

    public ResourceGatheringUtility(ResourceGatheringUtilityData mld) : base(mld)
    {
        _range = mld.range;
        _duration = mld.duration;
        _extractionAmount = mld.extractionAmount;

        _vfxPrefab = mld.vfxPrefab;
    }

    public void SetTarget(ResourceEntity target)
    {
        _target = target;
    }
    public override void AttemptActivate()
    {
        //need to check if there are any viable asteroids first
        if (_target == null || Vector3.Distance(_target.transform.position, base.owner.transform.position) > _range)
            return;

        //default cooldown check
        base.AttemptActivate();
    }
    protected override void OnActivation()
    {
        LogManager.getInstance.AddEntry("[" + base.owner.name + "]: Mining operation started.");
        base.owner.StartCoroutine(ExtractResources(_target));
        //start cooldown
        base.OnActivation();
    }

    IEnumerator ExtractResources(ResourceEntity target)
    {
        float extractionRate = _extractionAmount / _duration;
        float sum = 0f;
        float t = 0f;

        GameObject vfx = Object.Instantiate(_vfxPrefab, base.owner.transform);
        GameObject visualTarget = target.GetVisualTarget();

        LineRenderer lr = vfx.GetComponent<LineRenderer>();
        lr.positionCount = 2;

        while (t <= _duration)
        {
            if (target == null || Vector3.Distance(base.owner.transform.position, target.transform.position) > _range)
            {
                LogManager.getInstance.AddEntry("[" + base.owner.name + "]: Mining operation aborted. " + sum + " units of " + FleetVital.Format(target.type) + " acquired.");
                Object.Destroy(vfx);
                yield break;
            }

            t += Time.deltaTime;
            sum += target.Extract(extractionRate * Time.deltaTime);
            PlayerData.fleet.GetVital(target.type).Update(target.Extract(extractionRate * Time.deltaTime));

            lr.SetPosition(0, base.owner.transform.position);
            lr.SetPosition(1, visualTarget.transform.position);
            yield return null;
        }

        LogManager.getInstance.AddEntry("[" + base.owner.name + "]: Mining operation completed. " + sum + " units of " + FleetVital.Format(target.type) + " acquired.");
        Object.Destroy(vfx);
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

        Visualizer.getInstance.DrawLine(base.owner.gameObject, positions, Color.blue, true);
    }
}
