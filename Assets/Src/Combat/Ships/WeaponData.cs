using UnityEngine;

public class WeaponData : ShipComponentData
{
    [SerializeField]float _speed;

    [SerializeField]float _minDamage;
    [SerializeField]float _maxDamage;

    [SerializeField]GameObject _shotVFX;
    [SerializeField]GameObject _detonationVFX;

    public float speed { get { return _speed; } }

    public float minDamage { get { return _minDamage; } }
    public float maxDamage { get { return _maxDamage; } }

    public GameObject shotVFX { get { return _shotVFX; } }
    public GameObject detonationVFX { get { return _detonationVFX; } }

    public override ShipComponent Instantiate()
    {
        return new Weapon(this);
    }
}
public class Weapon : ShipComponent
{
    float _speed;
    float _range;

    float _minDamage;
    float _maxDamage;

    GameObject _shotVFX;
    GameObject _detonationVFX;

    public float minDamage { get { return _minDamage; } }
    public float maxDamage { get { return _maxDamage; } }

    public float dps
    {
        get
        {
            float a = _minDamage + _maxDamage;
            a /= 2;
            a /= base.cooldown; 

            return a;
        }
    }

    public float speed { get { return _speed; } }
    public float range { get { return _range; } protected set { _range = value; } }

    public GameObject shotVFX { get { return _shotVFX; } }
    public GameObject detonationVFX { get { return _detonationVFX; } }

    public Weapon(WeaponData wd) : base(wd)
    {
        base.type = ShipComponentType.Weapon;

        _speed = wd.speed;

        _minDamage = wd.minDamage;
        _maxDamage = wd.maxDamage;

        _shotVFX = wd.shotVFX;
        _detonationVFX = wd.detonationVFX;
    }

    public virtual void AttemptFire(ShipEntity target, ShipEntity shooter)
    {
    }

    protected float GetDamage()
    {
        return Random.Range(_minDamage, _maxDamage);
    }
    protected void SpawnShieldVFX(Vector3 shotBackwards, Vector3 hitpos, ShipEntity target)
    {
        if (target.GetVital(VitalType.ShieldPoints).current > 0f)
            Object.Destroy(Object.Instantiate(ModelDB.GetShieldDeflectionVFX(), hitpos, Quaternion.LookRotation(shotBackwards), null), 3f);
    }

    public override string GetSummary()
    {
        string s = base.GetSummary();

        s += "\n\n";
        s += "Damage: <color=red>" + _minDamage + "</color> - <color=green>" + _maxDamage + "</color>\n\n"; 
        s += "Range: " + _range + "\n";
        s += "Speed: " + _speed;

        return s;
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

        Visualizer.getInstance.DrawLine(base.owner.gameObject, positions, Color.red, false);
    }
}