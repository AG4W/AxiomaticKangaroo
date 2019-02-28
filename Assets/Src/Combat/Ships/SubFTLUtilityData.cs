using UnityEngine;

using System.Collections;

[CreateAssetMenu(menuName = "Data/Parts/Utilities/Sub-FTL Drive")]
public class SubFTLUtilityData : UtilityData
{
    [Range(1f, 10000f)][SerializeField]float _jumpRange = 100f;
    [SerializeField]float _jumpTime = .1f;
    [SerializeField]float _chargeTime = 3f;

    [SerializeField]GameObject _chargeVFX;
    [SerializeField]GameObject _travelVFX;
    [SerializeField]GameObject _exitVFX;

    public float jumpRange { get { return _jumpRange; } }
    public float jumpTime { get { return _jumpTime; } }
    public float chargeTime { get { return _chargeTime; } }

    public GameObject chargeVFX { get { return _chargeVFX; } }
    public GameObject travelVFX { get { return _travelVFX; } }
    public GameObject exitVFX { get { return _exitVFX; } }

    public override ShipComponent Instantiate()
    {
        return new SubFTLUtility(this);
    }
}
public class SubFTLUtility : Utility
{
    float _jumpRange;
    float _jumpTime;
    float _chargeTime;

    GameObject _chargeVFX;
    GameObject _travelVFX;
    GameObject _exitVFX;

    GameObject _jumpVFXinstance;
    GameObject _travelVFXinstance;
    GameObject _exitVFXinstance;

    public float jumpRange { get { return _jumpRange; } }
    public float jumpTime { get { return _jumpTime; } }
    public float chargeTime { get { return _chargeTime; } }

    public SubFTLUtility(SubFTLUtilityData sftlud) : base(sftlud)
    {
        _jumpRange = sftlud.jumpRange;
        _jumpTime = sftlud.jumpTime;
        _chargeTime = sftlud.chargeTime;

        _chargeVFX = sftlud.chargeVFX;
        _travelVFX = sftlud.travelVFX;
        _exitVFX = sftlud.exitVFX;
    }

    protected override void OnActivation()
    {
        StartChargeUp();
        //start chargeup
        MasterTimer.getInstance.Request(_chargeTime, OnChargeUpComplete);

        //starts cooldown
        base.OnActivation();
    }
    void StartChargeUp()
    {
        //spawn vfx
        _jumpVFXinstance = Object.Instantiate(
            _chargeVFX, 
            base.owner.transform.position, 
            Quaternion.LookRotation(base.owner.transform.forward), 
            base.owner.transform);

        //setup charge
        var charge = _jumpVFXinstance.transform.Find("charge").GetComponent<ParticleSystem>().main;
        var explosion = _jumpVFXinstance.transform.Find("explosion").GetComponent<ParticleSystem>().main;
        var light = _jumpVFXinstance.transform.Find("light").GetComponent<ParticleSystem>().main;

        foreach (var ps in _jumpVFXinstance.GetComponentsInChildren<ParticleSystem>())
            ps.Stop();

        charge.duration = _chargeTime;
        explosion.startDelay = _chargeTime;
        //trace.startDelay = _chargeTime;
        light.duration = _chargeTime;
        light.startLifetime = _chargeTime;

        foreach (var ps in _jumpVFXinstance.GetComponentsInChildren<ParticleSystem>())
            ps.Play();
    }
    void OnChargeUpComplete()
    {
        //de-null to avoid effect following ship
        _jumpVFXinstance.transform.SetParent(null);
        Object.Destroy(_jumpVFXinstance, 1f);

        //spawn travel effect
        if(_travelVFX != null)
        {
            _travelVFXinstance = Object.Instantiate(_travelVFX, owner.transform.position, Quaternion.identity, owner.transform);
            Object.Destroy(_travelVFXinstance, _jumpTime + 1f);

            var ps = _travelVFXinstance.GetComponentInChildren<ParticleSystem>();
            var main = ps.main;

            ps.Stop();
            main.duration = _jumpTime;
            ps.Play();
        }

        base.owner.StartCoroutine(UpdateOwnerPosition());
    }

    IEnumerator UpdateOwnerPosition()
    {
        Vector3 o = base.owner.transform.position;
        Vector3 e = base.owner.transform.position += base.owner.transform.forward * _jumpRange;

        float t = 0f;

        while (t <= _jumpTime)
        {
            t += Time.deltaTime;
            base.owner.transform.position = Vector3.Lerp(o, e, Mathf.SmoothStep(0f, 1f, t / _jumpTime));
            yield return null;
        }

        //spawn exit effect
        if(_exitVFX != null)
        {
            _exitVFXinstance = Object.Instantiate(_exitVFX, base.owner.transform.position, Quaternion.LookRotation(base.owner.transform.forward), base.owner.transform);
            Object.Destroy(_exitVFXinstance, 1f);
        }
    }

    public override string GetSummary()
    {
        string s = base.GetSummary();

        s += "\n\n";
        s += "Range: " + _jumpRange;
        s += "Charge time: " + _chargeTime;

        return s;
    }
    public override void DrawVisualization()
    {
        Vector3[] positions = new Vector3[] { Vector3.zero, Vector3.forward * _jumpRange };
        Visualizer.getInstance.DrawLine(base.owner.gameObject, positions, Color.blue, false);
    }
}
