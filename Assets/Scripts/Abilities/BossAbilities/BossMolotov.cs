using System.Collections;
using UnityEngine;

public class BossMolotov: MonoBehaviour
{
    public float lifeTime;
    public float detonateDelayTime;
    public float rotateSpeed;
    public float speed;

    //the layers of objects this object is allowed to apply physics to
    public LayerMask targetLayers;

    public float burnTickPercentage;
    public float range;

    private CircleCollider2D burnCollider;

    private void Update()
    {
        ContinuouslyRotate();
        burnCollider.radius += .01f;
        burnCollider.radius -= .01f;
    }

    private void OnEnable()
    {
        burnCollider = GetComponent<CircleCollider2D>();
        burnCollider.enabled = false;
        burnCollider.radius = range;

        foreach(SpriteRenderer child in gameObject.GetComponentsInChildren
            <SpriteRenderer>())
        {
            Color tempRend = child.color;
            tempRend.a = 1f;
            child.color = tempRend;
        }

        StartCoroutine(StartDetonateCountdown());
    }

    private void ContinuouslyRotate()
    {
        Vector3 newRotation = transform.eulerAngles;
        newRotation.z += Time.fixedDeltaTime * rotateSpeed;
        transform.eulerAngles = newRotation;
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

                //apply tick damage
                enemy.TakeDamage(Time.deltaTime * burnTickPercentage);
            }
        }
    }

    protected virtual IEnumerator StartDetonateCountdown()
    {
        yield return new WaitForSecondsRealtime(detonateDelayTime);
        burnCollider.enabled = true;
        GetComponent<Rigidbody2D>().linearVelocity = Vector3.zero;

        foreach (SpriteRenderer child in gameObject.GetComponentsInChildren
           <SpriteRenderer>())
        {
            Color tempRend = child.color;
            tempRend.a = 0f;
            child.color = tempRend;
        }

        yield return StartLifetimeCountdown();
    }

    protected virtual IEnumerator StartLifetimeCountdown()
    {
        yield return new WaitForSecondsRealtime(lifeTime);
        ObjectPoolingManager.ReturnObjectToPool(gameObject,
            ObjectPoolingManager.PoolType.Bullet);
    }
}
