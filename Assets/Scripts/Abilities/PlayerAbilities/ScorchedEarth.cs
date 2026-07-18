using UnityEngine;

public class ScorchedEarth : AbilityBaseClass
{
    public float burnTickPercentage;
    public float burnDamageIncreaseAmnt;

    public float range;
    public float burnVFXSize;
    public float rangeIncreaseAmnt;

    public float lifetime;
    public float lifetimeIncreaseAmnt;

    public int rounds;

    private float curCastCooldown;
    public float castCooldown;

    public GameObject molotovObj;

    protected override void Update()
    {
        curCastCooldown -= Time.deltaTime;

        if (curCastCooldown <= 0)
        {
            curCastCooldown = castCooldown;

            float newangle = 0f;
            for (int i = 0; i < rounds; i++)
            {
                ThrowMolotovs(newangle);
                newangle += 60;
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
        burnTickPercentage *= burnDamageIncreaseAmnt;
        range *= rangeIncreaseAmnt;
        burnVFXSize *= rangeIncreaseAmnt;
        lifetime += lifetimeIncreaseAmnt;
    }

    private void ThrowMolotovs(float angle)
    {
        //loads in and fires bullet
        GameObject molotovObjCopy = ObjectPoolingManager.SpawnObject(molotovObj,
            transform.position, Quaternion.identity,
            ObjectPoolingManager.PoolType.Bullet);

        //sets the stats of the molotov
        Molotov molotov = molotovObjCopy.GetComponent<Molotov>();
        molotov.SetValues(burnTickPercentage, range, lifetime, burnVFXSize);

        Vector3 force = Quaternion.Euler(0, 0, angle) * transform.right *
            molotov.speed;

        molotov.GetComponent<Rigidbody2D>().
            AddForce(force, ForceMode2D.Impulse);
    }
}
