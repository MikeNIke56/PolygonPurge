using System.Collections;
using UnityEngine;

public class AbyssalPortalBlackHoleObj : MonoBehaviour
{
    public float damage;
    public float lifeTime;
    public float pullStrength;

    //the layers of objects this object is allowed to apply physics to
    public LayerMask targetLayers;

    private void Start()
    {
        StartCoroutine(StartLifetimeCountdown());
    }

    private void Update()
    {
        GetComponent<CircleCollider2D>().radius += .01f;
        GetComponent<CircleCollider2D>().radius -= .01f;
    }

    private void OnEnable()
    {
        StartCoroutine(StartLifetimeCountdown());
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
                EntityBaseClass enemy = entity.GetComponent<EntityBaseClass>();

                //cause damage and pull enemy in
                enemy.TakeDamage(Time.deltaTime * damage);
                PullIn(enemy);
            }
        }
    }

    private IEnumerator StartLifetimeCountdown()
    {
        yield return new WaitForSeconds(lifeTime);
        ObjectPoolingManager.ReturnObjectToPool(gameObject,
            ObjectPoolingManager.PoolType.Ability);
    }

    /**
     * pulls in enemies towards center of the object
     */
    private void PullIn(EntityBaseClass enemy)
    {
        Vector2 direction = (transform.position -
            enemy.transform.position).normalized;
        float distance = Vector2.Distance(transform.position,
            enemy.transform.position);

        if (distance < GetComponent<CircleCollider2D>().radius)
            enemy.GetComponent<Rigidbody2D>().AddForce(direction * pullStrength);
    }
}
