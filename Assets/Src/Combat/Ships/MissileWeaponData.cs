using UnityEngine;

using System.Collections;

[CreateAssetMenu(menuName =("Data/Parts/Weapons/Missile Weapon"))]
public class MissileWeaponData : WeaponData
{
    [SerializeField]float _lifetime;

    public float lifetime { get { return _lifetime; } }

    public override ShipComponent Instantiate()
    {
        return new MissileWeapon(this);
    }
}
public class MissileWeapon : Weapon
{
    float _lifetime;

    public MissileWeapon(MissileWeaponData mwd) : base(mwd)
    {
        _lifetime = mwd.lifetime;

        base.range = _lifetime * base.speed;
    }

    public override void AttemptFire(ShipEntity target, ShipEntity shooter)
    {
        if (target == null || shooter == null)
            return;

        //on hit
        CoroutineSurrogate.getInstance.StartCoroutine(MoveMissile(shooter.transform.position, target));

        //activate cooldown
        base.StartCooldown();
    }

    IEnumerator MoveMissile(Vector3 start, ShipEntity target)
    {
        GameObject missile = Object.Instantiate(base.shotVFX, start, Quaternion.identity, null);

        float t = 0f;

        while (t <= _lifetime && target != null)
        {
            if (Vector3.Distance(missile.transform.position, target.transform.position) <= 1f)
            {
                Vector3 hitpos = (missile.transform.position - target.transform.position).normalized;
                base.SpawnShieldVFX(hitpos, hitpos * 5f, target);
                target.ApplyDamage(GetDamage());
                break;
            }

            t += Time.deltaTime;
            missile.transform.position = Vector3.Lerp(start, target.transform.position, t / _lifetime);
            missile.transform.LookAt(Vector3.Lerp(start, target.transform.position, (t + .1f) / _lifetime));
            yield return null;
        }

        Object.Destroy(Object.Instantiate(base.detonationVFX, missile.transform.position, Random.rotation, null), 3f);
        Object.Destroy(missile);
    }

    public override string GetSummary()
    {
        string s = base.GetSummary();
        s += "\n\n";

        s += "Lifetime: " + _lifetime;

        return s;
    }
    public override void DrawVisualization()
    {
        int segments = 80;
        Vector3[] positions = new Vector3[segments];

        float angle = 90f;

        for (int i = 0; i < segments; i++)
        {
            positions[i] = new Vector3(Mathf.Sin(Mathf.Deg2Rad * angle) * base.range, 0, Mathf.Cos(Mathf.Deg2Rad * angle) * base.range);
            angle += (360f / segments);
        }

        Visualizer.getInstance.DrawLine(base.owner.gameObject, positions, Color.red, true);
    }
}
