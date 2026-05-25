using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MisslePods : AbilityBaseClass
{
    [Header("Missle Pods Variables")]
    public float damage;
    public float damageIncreaseAmnt;

    public float speed;
    public float speedIncreaseAmnt;

    public float fireRate;
    public float fireCooldown;
    public float fireCooldownDecreaseAmnt;

    public float range;
    public float rangeIncreaseAmnt;

    public float lookOffset;

    public GameObject misslePodObj;

    public int curMisslesActive = 0;
    public int maxMissles;

    private bool isOnFireCooldown = false;
    private Vector3 lastFireDirection;
    private Quaternion lastFaceDirection;

    protected override void Update()
    {
        if (isOnFireCooldown == false && curMisslesActive < maxMissles)
            StartCoroutine(SpawnMissle());

        SetFaceDirection();
        SetFireDirection();
    }


    public override void SetUp()
    {
        base.SetUp();
        maxMissles = 1;
    }

    public override void UpgradeAbility(int level)
    {
        base.UpgradeAbility(level);
        damage *= damageIncreaseAmnt;
        range += rangeIncreaseAmnt;
        speed += speedIncreaseAmnt;
        maxMissles+=2;
        fireCooldown /= fireCooldownDecreaseAmnt;
    }

    private IEnumerator SpawnMissle()
    {
        isOnFireCooldown = true;

        yield return new WaitForSecondsRealtime(.2f);

        for(int i = 0; i < maxMissles; i++)
        {
            /* face the direction the player is moving, 
             * if not moving- face in the last known fire direction if valid
             * if direction isnt valid, just face to the right
             */

            //loads in and fires missle
            GameObject missleObjCopy = ObjectPoolingManager.SpawnObject(
                misslePodObj, transform.position, lastFaceDirection, 
            ObjectPoolingManager.PoolType.Bullet);

            //sets the speed and damage of the missle
            MisslePod missle = missleObjCopy.GetComponent<MisslePod>();

            missle.SetDamage(damage);
            missle.SetSpeed(speed);
            missle.SetMisslePodsParent(this);
            curMisslesActive++;

            
            missle.GetRigidbody().AddForce(lastFireDirection * missle.GetSpeed(), 
                ForceMode2D.Impulse);

            yield return new WaitForSecondsRealtime(fireRate);
        }
       
        yield return StartMissleSpawnCooldown();
    }

    private void SetFireDirection()
    {
        /* fire in the direction the player is moving, 
        * if not moving- fire in the last known fire direction if valid
        * if direction isnt valid, just fire to the right
        */
        Vector3 force;
        if (PlayerController.i.GetRigidbody().linearVelocity.magnitude > 0.0f)
        {
            force = PlayerController.i.GetRigidbody().linearVelocity.
                normalized * 1f;
        }
        else if (PlayerController.i.GetRigidbody().
            linearVelocity.magnitude == 0.0f && lastFireDirection.
            magnitude > 0.0f)
        {
            force = lastFireDirection;
        }
        else
            force = transform.right * 1f;

        lastFireDirection = force;
    }

    private void SetFaceDirection()
    {
        Quaternion lookAngle;
        if (PlayerController.i.GetRigidbody().linearVelocity.magnitude > 0.0f)
        {
            lookAngle = GetLookAtRotation(PlayerController.i.GetRigidbody().
                linearVelocity.normalized * 1000f, lookOffset);
        }
        else if (PlayerController.i.GetRigidbody().
            linearVelocity.magnitude == 0.0f && lastFireDirection.
            magnitude > 0.0f)
        {
            lookAngle = lastFaceDirection;
        }
        else
            lookAngle = Quaternion.Euler(90f, 0f, 0f);

        lastFaceDirection = lookAngle;
    }

    private IEnumerator StartMissleSpawnCooldown()
    {
        yield return new WaitForSecondsRealtime(fireCooldown);
        isOnFireCooldown = false;
    }

    private Quaternion GetLookAtRotation(Vector3 target, float offset)
    {
        //find angle between the player and target
        float lookAngle = AngleBetweenTwoPoints(transform.position, target) + offset;

        return Quaternion.Euler(new Vector3(0, 0, lookAngle));
    }

    private float AngleBetweenTwoPoints(Vector3 point1, Vector3 point2)
    {
        return Mathf.Atan2(point1.y - point2.y, point1.x - point2.x) * Mathf.Rad2Deg;
    }
}
