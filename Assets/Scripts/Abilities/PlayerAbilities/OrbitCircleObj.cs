using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class OrbitCircleObj : MonoBehaviour
{
    public float damage;
    public float knockBackAmnt;

    //the layers of objects this object is allowed to apply physics to
    public LayerMask targetLayers;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        //if the collided object is on a layer we should interact with...
        if (LayerMaskChecker.i.IsInLayerMask(collider.gameObject, targetLayers))
        {
            GameObject entity = collider.gameObject;

            //and if it implements the IDamageable interface
            if (entity.TryGetComponent(out IDamageable myInterface))
            {
                EnemyBaseClass enemy = entity.GetComponent<EnemyBaseClass>();

                //cause damage and apply knockback
                enemy.TakeDamage(damage);
                StartCoroutine(enemy.ApplyKnockback(gameObject, knockBackAmnt));              
            }
            //enemy projectile
            else
            {
                ProjectileBaseClass enemyBullet = 
                    entity.GetComponent<ProjectileBaseClass>();

                ObjectPoolingManager.ReturnObjectToPool(enemyBullet.gameObject,
                    ObjectPoolingManager.PoolType.Bullet);
            }    
        }
    }
}
