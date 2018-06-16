using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Abilities/RaycastAbility")]
public class LaserAbility : AbilityPew {

    public int gunDamage = 1;
    public float weaponRange = 50f;
    public float hitForce = 100f;
    public Material matsku;

    private RaycastShoot rcShoot;

    public override void Initialize(GameObject obj)
    {
        rcShoot = obj.GetComponent<RaycastShoot>();
        rcShoot.Initialize();

        rcShoot.gunDamage = gunDamage;
        rcShoot.weaponRange = weaponRange;
        rcShoot.hitForce = hitForce;
        rcShoot.laserLine.material = matsku;
    }

    public override void TriggerAbility()
    {
        rcShoot.Fire();
    }
}
