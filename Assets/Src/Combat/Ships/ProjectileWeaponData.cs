using UnityEngine;

using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Data/Parts/Weapons/Projectile Weapon")]
public class ProjectileWeaponData : WeaponData
{
    [Range(0f, 1f)][SerializeField]float _accuracy = 1f;
    [SerializeField]float _range;

    [Header("Fields of Fire")]
    [Range(1, 180)][SerializeField]float _fofWidth;

    public float accuracy { get { return _accuracy; } }
    public float range { get { return _range; } }

    public float fofWidth { get { return _fofWidth; } }

    public override ShipComponent Instantiate()
    {
        return new ProjectileWeapon(this);
    }
}
public class ProjectileWeapon : Weapon
{
    float _accuracy;
    float _fofWidth;

    public float accuracy { get { return _accuracy; } }
    public float fofWidth { get { return _fofWidth; } }

    public ProjectileWeapon(ProjectileWeaponData pwd) : base(pwd)
    {
        base.range = pwd.range;

        _accuracy = pwd.accuracy;
        _fofWidth = pwd.fofWidth;
    }

    public override void AttemptFire(ShipEntity target, ShipEntity shooter)
    {
        if (target == null || shooter == null)
            return;

        if (Vector3.Distance(target.transform.position, shooter.transform.position) > base.range)
            return;

        if (!IsInsideFieldOfFire(target, shooter, true) && !IsInsideFieldOfFire(target, shooter, false))
            return;

        float fa = _accuracy;

        //apply target size penalties
        fa -= _accuracy * ((int)target.ship.size * .1f);

        bool hit = Random.Range(0f, 1f) <= fa;

        //on hit
        if (hit)
            CoroutineSurrogate.getInstance.StartCoroutine(MoveProjectile(shooter.transform.position, target, true));
        else
            CoroutineSurrogate.getInstance.StartCoroutine(MoveProjectile(shooter.transform.position, target, false));

        //activate cooldown
        base.StartCooldown();
    }
    bool IsInsideFieldOfFire(ShipEntity target, ShipEntity shooter, bool right)
    {
        Vector3 targetYFlattened = target.transform.position;
        targetYFlattened.y = shooter.transform.position.y;

        bool isInside = Vector3.Angle(right ? -shooter.transform.right : shooter.transform.right, targetYFlattened - shooter.transform.position) <= _fofWidth * .5f;

        //Debug.DrawLine(targetYFlattened, shooter.transform.position, isInside ? Color.green : Color.red, 5f);
        //Debug.Log(isInside);
        return isInside;
    }

    IEnumerator MoveProjectile(Vector3 start, ShipEntity target, bool hit)
    {
        GameObject projectile = Object.Instantiate(base.shotVFX, start, Quaternion.identity, null);
        Vector3 miss = target.transform.position + Random.onUnitSphere * 10f;
        Vector3 hitpos = target.transform.position + (base.owner.transform.position - target.transform.position).normalized * (2 + (int)target.ship.size);

        float d = Vector3.Distance(start, target.transform.position);
        float tt = d / base.speed;
        float t = 0f;

        while (t <= tt && target != null)
        {
            t += Time.deltaTime;

            hitpos = target.transform.position + (start - target.transform.position).normalized * (2 + (int)target.ship.size);
            projectile.transform.position = Vector3.Lerp(start, hit ? hitpos : miss, t / tt);
            projectile.transform.LookAt(hit ? target.transform.position : miss);
            yield return null;
        }

        if(target != null)
        {
            if (hit)
            {
                base.SpawnShieldVFX(-projectile.transform.forward.normalized, hitpos, target);
                target.ApplyDamage(base.GetDamage());
            }
        }

        Object.Destroy(Object.Instantiate(detonationVFX, projectile.transform.position, Random.rotation, null), 3f);
        Object.Destroy(projectile);
    }

    public override string GetSummary()
    {
        string s = base.GetSummary();
        s += "\n\n";

        s += "Fire Arc: " + _fofWidth + "°\n";
        s += "Accuracy: " + (_accuracy * 100f).ToString("0.##") + "%";

        return s;
    }
    public override void DrawVisualization()
    {
        List<Vector3> positions = new List<Vector3>();

        positions.AddRange(DrawArcOfFire(90 - (_fofWidth * .5f), _fofWidth, 20));
        positions.AddRange(DrawArcOfFire(270 - (_fofWidth * .5f), _fofWidth, 20));

        Visualizer.getInstance.DrawLine(base.owner.gameObject, positions.ToArray(), Color.red, true);
    }
    Vector3[] DrawArcOfFire(float startAngle, float arcLength, int segments)
    {
        Vector3[] positions = new Vector3[segments + 1];
        positions[0] = Vector3.zero;
        positions[20] = Vector3.zero;

        float angle = startAngle;

        for (int i = 1; i < segments; i++)
        {
            positions[i] = Extensions.DirectionFromAngle(angle) * base.range;
            angle += (arcLength / segments);
        }

        return positions;
    }

    //public override bool CanTarget(Vector3 target, Vector3 shooter)
    //{
    //    //if in range
    //    if (!base.CanTarget(target, shooter))
    //        return false;

    //    //if in field of fire
    //    Vector3 forward = base.hardpoint.forward;

    //    Vector2 hardpointForward = new Vector2(forward.x, forward.z);
    //    Vector3 heading = target - shooter;

    //    Vector2 hWidth = new Vector2(heading.x, heading.z);

    //    if (Vector2.Angle(hWidth, hardpointForward) > _fofHeight)
    //        return false;

    //    hardpointForward = new Vector2(forward.y, forward.z);
    //    Vector2 hHeight = new Vector2(heading.y, heading.z);

    //    if (Vector2.Angle(hHeight, hardpointForward) > _fofWidth)
    //        return false;

    //    return true;
    //}
}