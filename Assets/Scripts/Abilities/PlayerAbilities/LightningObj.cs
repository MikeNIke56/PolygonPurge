using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class LightningObj : MonoBehaviour
{
    public float damage;
    public float lifeTime;
    public float damageDelay;
    public float range;

    public GameObject lightningVFX;
    public float lightningVFXSize;
    private CircleCollider2D lightningCollider;

    //the layers of objects this object is allowed to apply physics to
    public LayerMask targetLayers;
    private bool effectSpawned = false;
    private void Start()
    {
        lightningCollider = GetComponent<CircleCollider2D>();
        lightningCollider.enabled = false;
        lightningCollider.radius = range;

        StartCoroutine(StartLifetimeCountdown());
    }

    private void OnEnable()
    {
        lightningCollider = GetComponent<CircleCollider2D>();
        lightningCollider.enabled = false;
        lightningCollider.radius = range;

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
        if (effectSpawned == false)
        {
            effectSpawned = true;
            //loads in lightning
            GameObject lightningCopy = ObjectPoolingManager.SpawnObject(
            lightningVFX, transform.position,
            Quaternion.identity, ObjectPoolingManager.PoolType.VFX);

            //wait a bit before enabling hitbox- gives time so lightning vfx and 
            //hitbox are synced
            yield return new WaitForSeconds(damageDelay);
            lightningCollider.enabled = true;

            lightningCopy.transform.localScale = new Vector3(lightningVFXSize,
                lightningVFXSize, lightningVFXSize);

            lightningCopy.transform.position = new Vector3(
                lightningCopy.transform.position.x,
                lightningCopy.transform.position.y + 2.5f);

            yield return new WaitForSeconds(lifeTime - damageDelay);

            effectSpawned = false;
            lightningCollider.enabled = false;

            ObjectPoolingManager.ReturnObjectToPool(gameObject,
                ObjectPoolingManager.PoolType.Ability);
        }
    }

    public void SetValues(float range, float damage,
       float lightningVFXSize)
    {
        this.range = range;
        this.damage = damage;
        this.lightningVFXSize = lightningVFXSize;

        lightningCollider = GetComponent<CircleCollider2D>();
        lightningCollider.radius = range;
    }
}
