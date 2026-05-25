using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MisslePod :ProjectileBaseClass
{
    private MisslePods misslePods;

    private void OnDisable()
    {
        misslePods.curMisslesActive--;
    }

    public float damageRadius;

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        //if the collided object is on a layer we should interact with...
        if (LayerMaskChecker.i.IsInLayerMask(collision.gameObject, targetLayers))
        {
            GameObject entity = collision.gameObject;

            //and if it implements the IDamageable interface
            if (entity.TryGetComponent(out IDamageable myInterface))
            {
                //cause AOE damage
                ApplyRadialDamage();

                if (numEnemiesPenetrated >= penetrationValue)
                {
                    ObjectPoolingManager.ReturnObjectToPool(gameObject,
                    ObjectPoolingManager.PoolType.Bullet);
                }
                else
                    //projectile will "penetrate" a certain # of enemies 
                    numEnemiesPenetrated++;
            }
        }
    }

    /**
     * applies radial damage to enemies inside damage radius
     */
    private void ApplyRadialDamage()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(new Vector3(
            transform.position.x, transform.position.y), damageRadius);

        if (colliders.Length > 0)
        {
            foreach (Collider2D col in colliders)
            {
                if (LayerMaskChecker.i.IsInLayerMask(
                    col.gameObject, targetLayers))
                {
                    float proximity = (transform.position -
                        col.transform.position).magnitude;

                    //normalize the proximity- 1.0 at center, 0.0 at edge
                    float falloff = 1f - Mathf.Clamp01(proximity / damageRadius);

                    float finalDamage = damage * falloff;
                    col.gameObject.GetComponent<EnemyBaseClass>().
                        TakeDamage(finalDamage);
                }
            }
        }
    }

    protected override IEnumerator StartLifetimeCountdown()
    {
        yield return new WaitForSecondsRealtime(lifeTime);

        ApplyRadialDamage();
        ObjectPoolingManager.ReturnObjectToPool(gameObject,
            ObjectPoolingManager.PoolType.Bullet);
    }

    /*
    private void OnDrawGizmos()
    {
        // Set the color with custom alpha.
        Gizmos.color = new Color(1f, 0f, 0f, 1f); // Red with custom alpha

        // Draw the sphere.
        Gizmos.DrawSphere(transform.position, damageRadius);
    }*/

    public void SetMisslePodsParent(MisslePods misslePods)
    {
        this.misslePods = misslePods;
    }
}
