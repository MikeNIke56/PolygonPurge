using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossArcShockwaves: AbilityBaseClass
{
    [Header("Arc Shockwave Variables")]
    public float damage;
    public float damageIncreaseAmnt;

    private float curCastTime;
    public float maxCastTime;

    public GameObject arcShockwaveObj;

    private void Start()
    {
        
    }

    protected override void Update()
    {
        curCastTime -= Time.deltaTime;

        if (curCastTime <= 0f)
        {
            curCastTime = maxCastTime;
            EmitShockwave();
        }
    }

    public override void SetUp()
    {
        base.SetUp();
        curCastTime = maxCastTime;
    }

    public override void UpgradeAbility(int level)
    {
        base.UpgradeAbility(level);
        damage *= damageIncreaseAmnt;
    }

    private void EmitShockwave()
    {
        //loads in shockwave
        GameObject arcShockwaveCopy = ObjectPoolingManager.SpawnObject(
            arcShockwaveObj, boss.transform.position, Quaternion.identity,
            ObjectPoolingManager.PoolType.Ability);

        //sets the damage of the shockwave
        BossArcShockwaveObj shockwave = arcShockwaveCopy.
            GetComponent<BossArcShockwaveObj>();
        shockwave.SetDamage(damage);
    }
}
