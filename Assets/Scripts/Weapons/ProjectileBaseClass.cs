using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

/**
* base class for all projectile bullets
*/
public class ProjectileBaseClass : MonoBehaviour
{
    [Header("Base Variables")]
    protected Rigidbody2D rb;
    protected float damage;
    [SerializeField] protected float speed;
    public float lifeTime;
    protected int numEnemiesPenetrated;
    protected int penetrationValue;

    //the layers of objects this object is allowed to apply physics to
    public LayerMask targetLayers;

    //bullet's previous velocity to restore back to after colliding with 
    //an object
    protected Vector3 savedVelocity;

    protected void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        savedVelocity = rb.linearVelocity;
    }

    protected void OnEnable()
    {
        numEnemiesPenetrated = 0;
        StartCoroutine(StartLifetimeCountdown());
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        //if the collided object is on a layer we should interact with...
        if (LayerMaskChecker.i.IsInLayerMask(collision.gameObject, targetLayers))
        {
            GameObject entity = collision.gameObject;

            //and if it implements the IDamageable interface
            if (entity.TryGetComponent(out IDamageable myInterface))
            {
                //cause damage
                entity.GetComponent<EntityBaseClass>().TakeDamage(damage);

                if(numEnemiesPenetrated == penetrationValue)
                {
                    if (gameObject.activeSelf == false) return;

                    ObjectPoolingManager.ReturnObjectToPool(gameObject, 
                        ObjectPoolingManager.PoolType.Bullet);
                }
                else
                    //projectile will "penetrate" a certain # of enemies 
                    numEnemiesPenetrated++;

                //restore bullet's velocity after hitting target
                rb.linearVelocity = savedVelocity;
            }
        }
    }

    protected virtual IEnumerator StartLifetimeCountdown()
    {
        yield return new WaitForSecondsRealtime(lifeTime);
        ObjectPoolingManager.ReturnObjectToPool(gameObject, 
            ObjectPoolingManager.PoolType.Bullet);
    }

    public virtual float GetSpeed()
    {
        return speed;
    }
    public Rigidbody2D GetRigidbody()
    {
        return rb;
    }

    public void SetAllStats(float damage,  float speed, float size,
        int penetration, float lifetime)
    {
        this.damage = damage;
        this.speed = speed;
        gameObject.transform.localScale = new Vector3(size, size, size);
        penetrationValue = penetration;
        this.lifeTime = lifetime;
    }
    public void SetDamage(float damage)
    {
        this.damage = damage;
    }
    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }
    public void SetSize(float size)
    {
        gameObject.transform.localScale = new Vector3(size, size, size);
    }
    public void SetPenetration(int penetration)
    {
        penetrationValue = penetration;
    }
    public void SetLifetime(float lifetime)
    {
        this.lifeTime = lifetime;
    }
}
