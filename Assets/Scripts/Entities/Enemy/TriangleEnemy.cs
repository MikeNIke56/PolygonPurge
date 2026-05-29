using System.Collections;
using UnityEngine;

public class TriangleEnemy : EnemyBaseClass
{
    public float retreatDistance;
    public float maxChaseDistance;

    //how much to offset the look rotation
    public float offset;

    public Transform firepoint;

    //each weapon's respective bullet object
    public GameObject bulletObj;

    protected override void OnEnable()
    {
        Setup(player);
        currentEnemyStates.Add(EnemyStates.Shooting);
    }

    public override void Setup(PlayerController player)
    {
        base.Setup(player);
        currentEnemyStates.Add(EnemyStates.Shooting);
    }

    public override void RunBehavior()
    {
        base.RunBehavior();

        LookAt(player.transform.position, offset);

        if (currentEnemyStates.Contains(EnemyStates.Shooting) &&
            !currentEnemyStates.Contains(EnemyStates.ProjectileAttackCooldown))
        {
            currentEnemyStates.Add(EnemyStates.ProjectileAttackCooldown);
            Shoot();
        }
    }

    public override void HandleMovement()
    {
        base.HandleMovement();

        float distFromPlayer = Vector2.Distance(transform.position, player.transform.position);

        //if the player is too far from us, then chase to get within range
        if (!currentEnemyStates.Contains(EnemyStates.KnockedBack))
        {
            if (distFromPlayer >= maxChaseDistance)
            {
                if (!currentEnemyStates.Contains(EnemyStates.Chasing))
                {
                    currentEnemyStates.Add(EnemyStates.Chasing);
                    currentEnemyStates.Remove(EnemyStates.Retreating);
                }
                ChasePlayer();
            }
            //if the player is too close to us, then retreat until they are out of retreat range
            else if (distFromPlayer <= retreatDistance)
            {
                if (!currentEnemyStates.Contains(EnemyStates.Retreating))
                {
                    currentEnemyStates.Add(EnemyStates.Retreating);
                    currentEnemyStates.Remove(EnemyStates.Chasing);
                }
                Retreat();
            }
        }     
    }

    protected override void ChasePlayer()
    {
        if(player && rb)
        {
            Vector2 direction = (player.transform.position - transform.position).normalized;
            rb.linearVelocity = direction * moveSpeed * Time.fixedDeltaTime;
        }
    }

    private void Retreat()
    {
        if (player && rb)
        {
            Vector2 direction = (player.transform.position - transform.position).normalized;
            rb.linearVelocity = direction * -moveSpeed * Time.fixedDeltaTime;
        }
    }

    protected override void Shoot()
    {
        //loads in and fires bullet
        GameObject bulletObjCopy = ObjectPoolingManager.SpawnObject(bulletObj, firepoint.position,
            firepoint.localRotation, ObjectPoolingManager.PoolType.Bullet);

        //sets the stats of the bullet
        ProjectileBaseClass projectile = bulletObjCopy.GetComponent<ProjectileBaseClass>();
        projectile.SetDamage(projectileAttack);
        projectile.SetSize(bulletSize);
        projectile.SetSpeed(bulletSpeed);
        projectile.SetLifetime(bulletLifetime);
        Vector3 force = firepoint.right * projectile.GetSpeed();
        projectile.GetRigidbody().AddForce(force, ForceMode2D.Impulse);
    }
}
