using UnityEngine;

public class RPG : WeaponBaseClass
{
    [SerializeField] private float newDamageRadius;

    public override void UpgradeWeapon(int level)
    {
        base.UpgradeWeapon(level);
        newDamageRadius += .5f;
    }
    public override void Fire()
    {
        //play muzzle lash animation
        muzzleFlash.SetTrigger("Fire");

        //loads in and fires bullet
        GameObject bulletObjCopy = ObjectPoolingManager.SpawnObject(bulletObj, fireOffset.position,
            fireOffset.rotation, ObjectPoolingManager.PoolType.Bullet);

        //sets the stats of the bullet
        ProjectileBaseClass projectile = bulletObjCopy.GetComponent<ProjectileBaseClass>();
        projectile.SetAllStats(attack, projectileSpeed, projectileSize,
            projectilePenetration, projectileLifetime);

        RPGRocket rocketProj = projectile as RPGRocket;
        rocketProj.damageRadius = newDamageRadius;

        //grab the bullet cone of the weapon and set the bullet's random
        //direction
        float spread = Random.Range(-projectileSpread, projectileSpread);
        Vector3 direction = transform.right + transform.up * spread;

        //keep consistent speed
        direction.Normalize();

        Vector3 force = direction * projectile.GetSpeed();
        projectile.GetRigidbody().AddForce(force, ForceMode2D.Impulse);
    }

}
