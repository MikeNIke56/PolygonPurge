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
    public float burnVFXSize;

    private CircleCollider2D burnCollider;
    public GameObject burnVFX;

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
        yield return new WaitForSeconds(detonateDelayTime);
        burnCollider.enabled = true;
        GetComponent<Rigidbody2D>().linearVelocity = Vector3.zero;

        foreach (SpriteRenderer child in gameObject.GetComponentsInChildren
           <SpriteRenderer>())
        {
            Color tempRend = child.color;
            tempRend.a = 0f;
            child.color = tempRend;
        }

        //loads in flames
        GameObject burnCopy = ObjectPoolingManager.SpawnObject(
            burnVFX, transform.position,
            Quaternion.identity, ObjectPoolingManager.PoolType.VFX);

        burnCopy.transform.localScale = new Vector3(burnVFXSize, burnVFXSize,
           burnVFXSize);

        yield return StartLifetimeCountdown(burnCopy);
    }

    private IEnumerator StartLifetimeCountdown(GameObject burnCopy)
    {
        yield return new WaitForSeconds(lifeTime);

        ObjectPoolingManager.ReturnObjectToPool(burnCopy,
            ObjectPoolingManager.PoolType.VFX);

        ObjectPoolingManager.ReturnObjectToPool(gameObject,
            ObjectPoolingManager.PoolType.Ability);
    }

    public void SetValues(float burn, float range, float lifetime,
        float burnVFXSize)
    {
        burnTickPercentage = burn;
        this.range = range;
        this.lifeTime = lifetime;
        this.burnVFXSize = burnVFXSize;

        burnCollider = GetComponent<CircleCollider2D>();
        burnCollider.radius = range;
    }
}
