using UnityEngine;

public class RingOFire : AbilityBaseClass
{
    //the layers of objects this object is allowed to apply physics to
    public LayerMask targetLayers;

    public float burnTickPercentage;
    public float burnDamageIncreaseAmnt;

    public float range;
    public float rangeIncreaseAmnt;

    private CircleCollider2D burnCollider;

    protected override void Update()
    {
        /*
         * So the OnTriggerStay works correctly, we constantly adjust the 
         * size of the collider. This forces the collider to update
         * its collisions
         */
        burnCollider.radius += .001f;
        burnCollider.radius -= .001f;
    }

    public override void SetUp()
    {
        base.SetUp();
        burnCollider = GetComponent<CircleCollider2D>();
        burnCollider.radius = range;
    }

    public override void UpgradeAbility(int level)
    {
        base.UpgradeAbility(level);
        burnTickPercentage += burnDamageIncreaseAmnt;

        if (currentAbilityLevel < 5)
            range += rangeIncreaseAmnt;
        else
            range += (rangeIncreaseAmnt * 2);

        GetComponent<CircleCollider2D>().radius = range;
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        //if the collided object is on a layer we should interact with...
        if (LayerMaskChecker.i.IsInLayerMask(collider.gameObject, targetLayers))
        {
            GameObject entity = collider.gameObject;

            //and if it implements the IDamageable interface
            if (entity.TryGetComponent(out IDamageable myInterface))
            {
                EnemyBaseClass enemy = entity.GetComponent<EnemyBaseClass>();

                //apply tick damage
                enemy.TakeDamage(Time.deltaTime * burnTickPercentage);  
            }
        }
    }
}
