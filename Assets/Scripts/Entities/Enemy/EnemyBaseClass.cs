using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/**
* base class driver for all enemies
*/
public class EnemyBaseClass : EntityBaseClass
{
    public enum EnemyStates
    {
        Chasing,
        Shooting,
        Death,
        Dodging,
        CollisionAttackCooldown,
        ProjectileAttackCooldown,
        Retreating,
        Charging,
        ChargingAttackCooldown,
        KnockedBack
    }

    [Header("Enemy Base Variables")]
    [SerializeField] protected List<EnemyStates> currentEnemyStates;
    [SerializeField] protected float collisionAttack;
    [SerializeField] protected float projectileAttack;
    [SerializeField] protected Collider2D hitbox;
    [SerializeField] protected float maxCollisionAttackCooldown;
    [SerializeField] protected float maxProjectileCooldown;
    [SerializeField] protected int percentChanceToSpawn;
    [SerializeField] protected int lowerChance;
    [SerializeField] protected int upperChance;
    [SerializeField] protected int rank;
    [SerializeField] protected int exp;
    [SerializeField] protected float bulletSize;
    [SerializeField] protected float bulletSpeed;
    [SerializeField] protected float bulletLifetime;

    private float curCollisionAttackCooldown;
    private float curProjectileCooldown;

    protected Vector3 separationForce;

    //the layers of objects this object is allowed to apply physics to
    public LayerMask bodyCollisionTargetLayers;

    protected PlayerController player;

    protected virtual void OnEnable()
    {
        Setup(player);
    }

    public virtual void Setup(PlayerController player)
    {
        currentEnemyStates.Clear();
        curHealth = maxHealth;
        curCollisionAttackCooldown = maxCollisionAttackCooldown;
        curProjectileCooldown = maxProjectileCooldown;
        this.player = player;
        rb = GetComponent<Rigidbody2D>();
        GetComponent<SpriteRenderer>().color = baseColor;
    }

    public virtual void RunBehavior()
    {
        if (currentEnemyStates.Contains(EnemyStates.CollisionAttackCooldown))
            ReduceCollisionAttackCooldown();
        if (currentEnemyStates.Contains(EnemyStates.ProjectileAttackCooldown))
            ReduceProjectileAttackCooldown();

        if(changeSpriteColor == true)
        {
            ResetSpriteColor();
            if (GetComponent<SpriteRenderer>().color == baseColor)
                changeSpriteColor = false;
        }
    }

    public virtual void HandleMovement()
    {

    }

    protected virtual void ChasePlayer()
    {
        if(player && rb)
        {
            LookAt(player.transform.position, 180);
            rb.linearVelocity = (moveSpeed * Time.fixedDeltaTime * transform.right) + separationForce;
        }
    }

    protected virtual void Shoot()
    {
        
    }

    protected virtual void Shoot(Transform firepoint)
    {

    }

    /**
     * applies force to itself to space itself from other enemies
     */
    public void ApplySeparation(Vector2 force)
    {
        separationForce = force;
    }

    /**
     * knocks enemy back a bit
     */
    public IEnumerator ApplyKnockback(GameObject source, float knockbackAmnt)
    {
        currentEnemyStates.Add(EnemyStates.KnockedBack);

        //direction from the hit source to the enemy
        Vector2 direction = (transform.position -
            source.transform.position).normalized;

        //reset velocity so existing movement doesn't cancel knockback
        rb.linearVelocity = Vector2.zero;

        rb.AddForce(direction * knockbackAmnt, ForceMode2D.Impulse);

        yield return new WaitForSecondsRealtime(.5f);

        currentEnemyStates.Remove(EnemyStates.KnockedBack);
    }

    public override void TakeDamage(float damage)
    {
        curHealth -= damage;
        GetComponent<SpriteRenderer>().color = damageColor;
        changeSpriteColor = true;

        if (curHealth <= 0)
        {
            Die();
        }
    }

    public override void Die()
    {
        Debug.Log(name + " died");
        LevelSystem.i.AddXP(exp);
        ObjectPoolingManager.ReturnObjectToPool(gameObject, 
            ObjectPoolingManager.PoolType.Enemy);
        EnemyManager.i.curNumOfEnemies--;
    }

    /**
     * called specifically when enemy needs to be despawned for boss round
     */
    public void Despawn()
    {
        ObjectPoolingManager.ReturnObjectToPool(gameObject,
             ObjectPoolingManager.PoolType.Enemy);
        EnemyManager.i.curNumOfEnemies--;
    }

    /**
     * collsion methods to check if player collided with us and if we are still
     * actively colliding
     */
    protected void OnCollisionEnter2D(Collision2D collision)
    {
        //if we're not on collision cooldown...
        if(!currentEnemyStates.Contains(EnemyStates.CollisionAttackCooldown))
        {
            //if the collided object is on a layer we should interact with...
            if (LayerMaskChecker.i.
                IsInLayerMask(collision.gameObject, bodyCollisionTargetLayers))
            {
                GameObject entity = collision.gameObject;

                //and if it implements the IDamageable interface
                if (entity.TryGetComponent(out IDamageable myInterface))
                {
                    //cause damage
                    entity.GetComponent<EntityBaseClass>().TakeDamage(collisionAttack);
                    currentEnemyStates.Add(EnemyStates.CollisionAttackCooldown);
                }

            }
        } 
    }
    protected void OnCollisionStay2D(Collision2D collision)
    {
        //if we're not on collision cooldown...
        if (!currentEnemyStates.Contains(EnemyStates.CollisionAttackCooldown))
        {
            //if the collided object is on a layer we should interact with...
            if (LayerMaskChecker.i.
                IsInLayerMask(collision.gameObject, bodyCollisionTargetLayers))
            {
                GameObject entity = collision.gameObject;

                //and if it implements the IDamageable interface
                if (entity.TryGetComponent(out IDamageable myInterface))
                {
                    //cause damage
                    entity.GetComponent<EntityBaseClass>().TakeDamage(collisionAttack);
                    currentEnemyStates.Add(EnemyStates.CollisionAttackCooldown);
                }

            }
        }
    }

    /**
     * prevents continuous collision detection
     */
    private void ReduceCollisionAttackCooldown()
    {
        curCollisionAttackCooldown -= Time.deltaTime;

        if (curCollisionAttackCooldown <= 0)
        {
            currentEnemyStates.Remove(EnemyStates.CollisionAttackCooldown);
            curCollisionAttackCooldown = maxCollisionAttackCooldown;
        }
    }

    /**
     * acts as the fire rate of enemies that shoot
     */
    protected void ReduceProjectileAttackCooldown()
    {
        curProjectileCooldown -= Time.deltaTime;

        if (curProjectileCooldown <= 0)
        {
            currentEnemyStates.Remove(EnemyStates.ProjectileAttackCooldown);
            curProjectileCooldown = maxProjectileCooldown;
        }
    }

    public int GetChanceToSpawn()
    {
        return percentChanceToSpawn;
    }
    public int GetLowerChance()
    {
        return lowerChance;
    }
    public int GetUpperChance()
    {
        return upperChance;
    }
    public int GetRank()
    {
        return rank;
    }

    public void SetLowerChance(int chance)
    {
        lowerChance = chance;
    }
    public void SetUpperChance(int chance)
    {
        upperChance = chance;
    }
    public void SetPercentChanceToSpawn(int chance)
    {
        lowerChance = chance;
    }
}
