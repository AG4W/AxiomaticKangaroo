using UnityEngine;

using System.Collections.Generic;
using System.Linq;

public class NPCInputManager : MonoBehaviour
{
    //float _tickRate = .5f;
    //float _tickTimer = 0;

    //List<ShipEntity> _enemyShips;

    //void Update()
    //{
    //    ManageNPCShips();
    //}

    //void ManageNPCShips()
    //{
    //    _tickTimer += Time.deltaTime;

    //    if (_tickTimer >= _tickRate)
    //        UpdateTargets();
    //}

    //void UpdateTargets()
    //{
    //    for (int i = 0; i < GameManager.ships.Count; i++)
    //    {
    //        ShipEntity s = GameManager.ships[i];

    //        s.UpdateTarget(GetBestTarget(s));
    //    }

    //    UpdateTranslations();
    //}
    //void UpdateTranslations()
    //{
    //    _tickTimer = 0;

    //    for (int i = 0; i < GameManager.ships.Count; i++)
    //    {
    //        ShipEntity s = GameManager.ships[i];

    //        if (!s.isAIControlled)
    //            continue;

    //        if (s.target == null)
    //            continue;
    //        else
    //        {
    //            float d = Vector3.Distance(s.transform.position, s.target.transform.position);

    //            //if outside range, drive towards
    //            if(d > s.GetOptimalMaxRange())
    //                s.UpdateHeading((s.target.transform.position - s.transform.position).normalized);
    //            else
    //            {
    //                Vector3 directionToTarget = (s.target.transform.position - s.transform.position).normalized;
    //                float requiredAdjustment = 0f;

    //                //find optimal heading for maximum firepower towards target
    //                for (int a = 0; a < s.ship.chassi.weapons.Length; a++)
    //                {
    //                    Weapon w = s.ship.chassi.weapons[a];

    //                    if(w.GetType() == typeof(ProjectileWeapon))
    //                    {
    //                        ProjectileWeapon pw = w as ProjectileWeapon;

    //                        //find largest difference that's smaller than 90*, this'd be the weapon requiring the most adjustment
    //                        float angle = Mathf.Abs(Vector3.Angle(directionToTarget, pw.hardpoint.forward));

    //                        if (angle < 90 && angle > requiredAdjustment)
    //                            requiredAdjustment = angle;
    //                    }
    //                }

    //                Vector3 heading = Quaternion.Euler(0, requiredAdjustment, 0) * s.transform.forward;
    //                //Vector3 heading = Vector3.RotateTowards(s.transform.forward, directionToTarget, -requiredAdjustment, 1f);
    //                s.UpdateHeading(heading);
    //            }
    //        }
    //    }
    //}

    //ShipEntity GetBestTarget(ShipEntity ship)
    //{
    //    _enemyShips = ship.targets;

    //    return _enemyShips
    //       .OrderBy(s => Mathf.Abs((int)s.size - (int)ship.size))
    //       .ThenBy(s => Vector3.Distance(s.transform.position, ship.transform.position))
    //       .ElementAt(0);

    //    //.OrderBy(s => ship.GetWeaponsThatCanFireAtTargetCount(s))

    //    //string ds = ship.name + " selecting targets...\n";

    //    //for (int i = 0; i < enemyShips.Count; i++)
    //    //    ds +=
    //    //        enemyShips[i].name +
    //    //        ", weapons capable of targeting: " + ship.GetWeaponsThatCanFireAtTargetCount(enemyShips[i]) +
    //    //        ", distance: " + Vector3.Distance(ship.transform.position, enemyShips[i].transform.position) +
    //    //        ", threat: " + enemyShips[i].baseThreat +
    //    //        "\n";
    //    //ds += enemyShips[0].name;

    //    //Debug.Log(ds);
    //}
}