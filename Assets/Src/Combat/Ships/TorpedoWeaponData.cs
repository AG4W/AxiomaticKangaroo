using UnityEngine;

using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = ("Data/Parts/Weapons/Torpedo Weapon"))]
public class TorpedoWeaponData : WeaponData
{
    [SerializeField]float _lifetime;
    [SerializeField]float _detonationRange;

    public float lifetime { get { return _lifetime; } }
    public float detonationRange { get { return _detonationRange; } }

    public override ShipComponent Instantiate()
    {
        return new TorpedoWeapon(this);
    }
}
public class TorpedoWeapon : Weapon
{
    float _lifetime;
    float _detonationRange;

    public float lifetime { get { return _lifetime; } }

    public TorpedoWeapon(TorpedoWeaponData twd) : base(twd)
    {
        _lifetime = (twd.lifetime * base.rarity.modifier);
        _detonationRange = (twd.detonationRange * base.rarity.modifier);

        base.range = _lifetime * base.speed;
    }

    public override void AttemptFire(ShipEntity target, ShipEntity shooter)
    {
        if (shooter == null)
            return;

        //on hit
        CoroutineSurrogate.getInstance.StartCoroutine(MoveTorpedo(shooter));

        //activate cooldown
        base.StartCooldown();
    }

    IEnumerator MoveTorpedo(ShipEntity shooter)
    {
        Vector3 start = shooter.transform.position;
        Vector3 finalPosition = start + (shooter.transform.forward * base.range);
        GameObject torpedo = Object.Instantiate(base.shotVFX, start, Quaternion.identity, null);

        float t = 0f;

        while (t <= _lifetime)
        {
            if (DetonationCheck(torpedo, shooter))
                break;

            t += Time.deltaTime;
            torpedo.transform.position = Vector3.Lerp(start, finalPosition, t / _lifetime);
            torpedo.transform.LookAt(finalPosition);
            yield return null;
        }

        Object.Destroy(Object.Instantiate(base.detonationVFX, torpedo.transform.position, Random.rotation, null), 3f);
        Object.Destroy(torpedo);
    }
    bool DetonationCheck(GameObject torpedo, ShipEntity shooter)
    {
        //Debug.DrawLine(torpedo.transform.position, torpedo.transform.position + (torpedo.transform.up * _detonationRange), Color.red, 2f);
        //Debug.DrawLine(torpedo.transform.position, torpedo.transform.position + (-torpedo.transform.up * _detonationRange), Color.red, 2f);
        //Debug.DrawLine(torpedo.transform.position, torpedo.transform.position + (torpedo.transform.right * _detonationRange), Color.red, 2f);
        //Debug.DrawLine(torpedo.transform.position, torpedo.transform.position + (-torpedo.transform.right * _detonationRange), Color.red, 2f);

        List<ShipEntity> targets = new List<ShipEntity>();

        for (int i = 0; i < GameManager.ships.Count; i++)
        {
            if (GameManager.ships[i] != shooter && Vector3.Distance(torpedo.transform.position, GameManager.ships[i].transform.position) <= _detonationRange)
                targets.Add(GameManager.ships[i]);
        }

        if (targets.Count == 0)
            return false;

        for (int i = 0; i < targets.Count; i++)
        {
            Vector3 hitpos = (torpedo.transform.position - targets[i].transform.position).normalized;

            base.SpawnShieldVFX(hitpos, hitpos * 5f, targets[i]);
            targets[i].ApplyDamage(GetDamage());
        }

        return true;
    }

    public override string GetSummary()
    {
        string s = base.GetSummary();
        s += "\n\n";

        s += "Lifetime: " + _lifetime.ToString("0.##");
        s += "\n";

        s += "Detonation Radius: " + _detonationRange.ToString("0.##");

        return s;
    }
    public override void DrawVisualization()
    {
        Vector3[] positions = new Vector3[] { Vector3.zero, Vector3.forward * base.range };
        Visualizer.getInstance.DrawLine(base.owner.gameObject, positions, Color.red, false);
    }
}
