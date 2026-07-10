using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class LightningObj : MonoBehaviour
{
    public float damage;
    public float lifeTime;

    //the layers of objects this object is allowed to apply physics to
    public LayerMask targetLayers;

    private void Start()
    {
        StartCoroutine(StartLifetimeCountdown());
    }

    private void OnEnable()
    {
        StartCoroutine(StartLifetimeCountdown());
    }

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

                //cause damage
                enemy.TakeDamage(damage);
            }
        }
    }

    private IEnumerator StartLifetimeCountdown()
    {
        yield return new WaitForSeconds(lifeTime);
        ObjectPoolingManager.ReturnObjectToPool(gameObject,
            ObjectPoolingManager.PoolType.Ability);
    }
}
