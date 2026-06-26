using UnityEngine;

public class BodyBuckshots : AbilityBaseClass
{
    public float damage;
    public float damageIncreaseAmnt;

    public int rounds;

    private float curCastCooldown;
    public float castCooldown;

    public GameObject bulletObj;

    protected override void Update()
    {
        curCastCooldown -= Time.deltaTime;

        if (curCastCooldown <= 0)
        {
            curCastCooldown = castCooldown;

            float newangle = 0f;
            for (int i = 0; i < rounds; i++)
            {
                Shoot(newangle);
                newangle += 45;
            }
        }
    }

    public override void SetUp()
    {
        base.SetUp();
        curCastCooldown = castCooldown;
    }

    public override void UpgradeAbility(int level)
    {
        base.UpgradeAbility(level);
        damage *= damageIncreaseAmnt;
    }

    private void Shoot(float angle)
    {
        //loads in and fires bullet
        GameObject bulletObjCopy = ObjectPoolingManager.SpawnObject(bulletObj,
            transform.position, Quaternion.identity, 
            ObjectPoolingManager.PoolType.Bullet);

        //sets the speed and damage of the bullet
        ProjectileBaseClass projectile = bulletObjCopy.GetComponent<ProjectileBaseClass>();
        projectile.SetDamage(damage);

        Vector3 force = Quaternion.Euler(0, 0, angle) * transform.right * 
            projectile.GetSpeed();

        projectile.GetRigidbody().AddForce(force, ForceMode2D.Impulse);
    }
}
