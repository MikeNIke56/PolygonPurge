using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class FlameTrail : AbilityBaseClass
{
    private Trail trail;

    public float trailLifeTime;
    public float trailLifeTimeIncreaseAmnt;

    public float trailDamage;
    public float trailDamageIncreaseAmnt;

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void SetUp()
    {
        base.SetUp();

        trail = GetComponentInChildren<Trail>();
        trail.Setup(trailLifeTime, trailDamage);
    }

    public override void UpgradeAbility(int level)
    {
        base.UpgradeAbility(level);

        trailDamage += trailDamageIncreaseAmnt;
        trailLifeTime += trailLifeTimeIncreaseAmnt;

        trail.gameObject.GetComponent<TrailRenderer>().time = trailLifeTime;
        trail.trailDamage = trailDamage;
    }
}
