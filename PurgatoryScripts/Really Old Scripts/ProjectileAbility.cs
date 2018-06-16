using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Abilities/ProjectileAbility")]
public class ProjectileAbility : AbilityPew {

    public float projectileForce = 500f;
    public Rigidbody projectile;
    public int projDamage;

    private ProjectileShoot launcher;

    public override void Initialize(GameObject obj)
    {
        launcher = obj.GetComponent<ProjectileShoot>();
        launcher.projectileForce = projectileForce;
        launcher.projectile = projectile;
        launcher.projDamage = projDamage;
    }

    public override void TriggerAbility()
    {
        launcher.Launch();
    }
}
