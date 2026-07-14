using UnityEngine;

public class HexagonEnemy : EnemyBaseClass
{
    public float rotateSpeed;
    public Transform[] firepoints;

    //each weapon's respective bullet object
    public GameObject bulletObj;

    private Animator[] muzzleFlashObjs;

    protected override void OnEnable()
    {
        Setup(player);
        currentEnemyStates.Add(EnemyStates.Chasing);
        currentEnemyStates.Add(EnemyStates.Shooting);
    }

    public override void Setup(PlayerController player)
    {
        base.Setup(player);
        muzzleFlashObjs = GetComponentsInChildren<Animator>();
        currentEnemyStates.Add(EnemyStates.Chasing);
        currentEnemyStates.Add(EnemyStates.Shooting);
    }

    public override void RunBehavior()
    {
        base.RunBehavior();

        if (currentEnemyStates.Contains(EnemyStates.Shooting) &&
            !currentEnemyStates.Contains(EnemyStates.ProjectileAttackCooldown))
        {
            currentEnemyStates.Add(EnemyStates.ProjectileAttackCooldown);
            foreach (Transform firepoint in firepoints)
               Shoot(firepoint);

            //play muzzle lash animation
            foreach (Animator anim in muzzleFlashObjs)
                anim.SetTrigger("Fire");
        }
    }

    public override void HandleMovement()
    {
        ContinuouslyRotate();

        //constantly follow player
        Vector3 direction = (player.transform.position - transform.position).
            normalized;
        rb.linearVelocity = direction * moveSpeed * Time.fixedDeltaTime;
    }

    private void ContinuouslyRotate()
    {
        Vector3 newRotation = transform.eulerAngles;
        newRotation.z += Time.fixedDeltaTime * rotateSpeed;
        transform.eulerAngles = newRotation;
    }

    protected override void Shoot(Transform firepoint)
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
