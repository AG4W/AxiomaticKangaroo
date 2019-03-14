using UnityEngine;

using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Data/Parts/Weapons/Projectile Weapon")]
public class ProjectileWeaponData : WeaponData
{
    [Range(0f, 1f)][SerializeField]float _accuracy = 1f;
    [SerializeField]float _range;

    [Range(0f, 1f)][SerializeField]float _trackingPenalty;
    [Range(0f, 100f)][SerializeField]float _falloff;

    [Header("Fields of Fire")]
    [Range(1, 180)][SerializeField]float _fofWidth;

    public float accuracy { get { return _accuracy; } }
    public float range { get { return _range; } }

    public float trackingModifier { get { return _trackingPenalty; } }
    public float falloff { get { return _falloff; } }

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

    float _trackingModifier;
    float _falloff;

    public float accuracy { get { return _accuracy; } }

    public float trackingModifier { get { return _trackingModifier; } }
    public float falloff { get { return _falloff; } }

    public float fofWidth { get { return _fofWidth; } }

    public ProjectileWeapon(ProjectileWeaponData pwd) : base(pwd)
    {
        base.range = (pwd.range * base.rarity.modifier);

        _accuracy = Mathf.Clamp((pwd.accuracy * base.rarity.modifier), 0f, 100f);

        _trackingModifier = pwd.trackingModifier / base.rarity.modifier;
        _falloff = pwd.falloff / base.rarity.modifier;

        _fofWidth = Mathf.Clamp((pwd.fofWidth * base.rarity.modifier), 1f, 180f);
    }

    public override void AttemptFire(ShipEntity target, ShipEntity shooter)
    {
        if (target == null || shooter == null)
            return;

        if (Vector3.Distance(target.transform.position, shooter.transform.position) > base.range)
            return;

        if (!IsInsideFieldOfFire(target, shooter, true) && !IsInsideFieldOfFire(target, shooter, false))
            return;

        float fa = _accuracy * 100f;

        //apply target size/speed penalties
        //battleships are considered easy targets, and give neither negative/positive bonuses.
        //larger ships are easier to hit, smaller harder.
        //5% per class in either direction
        fa += ((int)target.ship.size - (int)HullClass.Battleship) * 5f;
        //ship moves faster -> harder to track accurately
        fa -= target.GetVital(VitalType.MovementSpeed).current * _trackingModifier;
        //similar for distance
        fa -= (Vector3.Distance(target.transform.position, shooter.transform.position) / base.range) * _falloff;
        //clamp to sane values, all shots have a .5% to hit, atleast.
        fa = Mathf.Clamp(fa, .5f, 100f);

        if (shooter.teamID == 0)
            Debug.Log(shooter.name + " fired!\n" +
                "Unmodified Accuracy: " + _accuracy * 100f + "%\n" +
                "Size Penalty: " + (((int)target.ship.size - (int)HullClass.Battleship) * 5f).ToString("+0.##;-0.##") + "%\n" +
                "Tracking Penalty: " + (target.GetVital(VitalType.MovementSpeed).current * _trackingModifier).ToString() + "%\n" +
                "Distance Penalty: " + ((Vector3.Distance(target.transform.position, shooter.transform.position) / base.range) * _falloff).ToString() + "%\n" +
                "Final Accuracy: " + fa + "%");

        CoroutineSurrogate.getInstance.StartCoroutine(
            MoveProjectile(
                shooter.transform.position, 
                target, 
                Random.Range(0f, 100f) <= fa));

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
        Vector3 targetPos = hit ? target.transform.position + (base.owner.transform.position - target.transform.position).normalized * (2 + (int)target.ship.size) : target.transform.position + Random.onUnitSphere * 12.5f;
        Vector3 lastKnownPos;

        float d = Vector3.Distance(start, target.transform.position);
        float tt = d / base.speed;
        float t = 0f;

        while (t <= tt)
        {
            t += Time.deltaTime;
            lastKnownPos = targetPos;

            if (target != null)
            {
                if(hit)
                    targetPos = target.transform.position + (start - target.transform.position).normalized * (2 + (int)target.ship.size);

                projectile.transform.position = Vector3.Lerp(start, targetPos, t / tt);
                projectile.transform.LookAt(targetPos);
            }
            else
            {
                projectile.transform.position = Vector3.Lerp(start, lastKnownPos, t / tt);
                projectile.transform.LookAt(lastKnownPos);
            }

            yield return null;
        }

        if(target != null && hit)
        {
            base.SpawnShieldVFX(-projectile.transform.forward.normalized, targetPos, target);
            target.ApplyDamage(base.GetDamage());
        }

        Object.Destroy(Object.Instantiate(detonationVFX, projectile.transform.position, projectile.transform.rotation, null), 3f);
        Object.Destroy(projectile);
    }

    public override string GetSummary()
    {
        string s = base.GetSummary();
        s += "\n\n";

        s += "Fire Arc: " + _fofWidth.ToString("0.##") + "°\n";
        s += "Accuracy: " + (_accuracy * 100f).ToString("0.##") + "%\n";
        s += "Tracking: -" + _trackingModifier.ToString("0.##") + "%\n";
        s += "Falloff: -" + _falloff.ToString("0.##") + "%";

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