using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossLittleBuddy: AbilityBaseClass
{
    [Header("Little Buddy Variables")]
    public float damage;
    public float damageIncreaseAmnt;

    public float fireRate;
    public float fireCooldown;
    public float fireCooldownDecreaseAmnt;

    public float range;
    public float rangeIncreaseAmnt;

    public GameObject littleBuddyBulletObj;

    private int curBurstsBeforeRetarget = 0;
    public int maxBurstsBeforeRetarget;

    [Header("Bob Up/Down Variables")]
    private Vector3 startLocalPos;
    public float frequency;
    public float amplitude;

    //as long as speed is lower than player velocity- how much the camera
    //"lags" behind
    public float followSpeed;

    public float lookOffset;

    private Transform target;
    private PlayerController targetedEnemy;
    private bool isOnFireCooldown = false;
    private Transform firepoint;

    private Animator muzzleFlash;

    private void Start()
    {
        target = boss.littleBuddyPivotPoint.transform;
        firepoint = GetComponentInChildren<Transform>();
        startLocalPos = transform.localPosition;
        muzzleFlash = GetComponentInChildren<Animator>();
    }

    protected override void Update()
    {
        BobUpDown();

        if (targetedEnemy.gameObject.activeSelf)
            LookAt(targetedEnemy.transform.position, lookOffset);
        else
            targetedEnemy = PlayerController.i;

        if (targetedEnemy.gameObject.activeSelf && isOnFireCooldown == false)
            StartCoroutine(Shoot());
    }

    public override void SetUp()
    {
        base.SetUp();
        targetedEnemy = PlayerController.i;
    }

    public override void UpgradeAbility(int level)
    {
        base.UpgradeAbility(level);
        damage *= damageIncreaseAmnt;
        range += rangeIncreaseAmnt;
        fireCooldown /= fireCooldownDecreaseAmnt;
    }

    private IEnumerator Shoot()
    {
        isOnFireCooldown = true;

        //wait a little bit so it has time to look at the enemy before shooting
        yield return new WaitForSecondsRealtime(1f);

        //burst fire
        for (int i = 0; i < 3; i++)
        {
            //play muzzle lash animation
            muzzleFlash.SetTrigger("Fire");

            //loads in and fires bullet
            GameObject bulletObjCopy = ObjectPoolingManager.SpawnObject(
                littleBuddyBulletObj, firepoint.position,
            firepoint.localRotation, ObjectPoolingManager.PoolType.Bullet);

            //sets the speed and damage of the bullet
            EnemyBullet littleBuddyBullet = bulletObjCopy.
                GetComponent<EnemyBullet>();

            littleBuddyBullet.SetDamage(damage);

            //adjust the angle to account for initial z offset
            Vector3 force = Quaternion.Euler(0, 0, 30f) * firepoint.transform.right
                * littleBuddyBullet.GetSpeed();

            littleBuddyBullet.GetRigidbody().AddForce(-force, ForceMode2D.Impulse);
            curBurstsBeforeRetarget++;
            yield return new WaitForSeconds(fireRate);
        }
        yield return StartFireCooldown();
    }

    private IEnumerator StartFireCooldown()
    {
        yield return new WaitForSeconds(fireCooldown);
        isOnFireCooldown = false;

        //if we've locked on to an enemy for 2 full bursts or
        //if we killed the targeted enemy
        if(curBurstsBeforeRetarget >= maxBurstsBeforeRetarget ||
            targetedEnemy.isActiveAndEnabled == false)
        {
            curBurstsBeforeRetarget = 0;
            targetedEnemy = PlayerController.i;
        }
    }

    private void LookAt(Vector3 target, float offset)
    {
        //find angle between the player and target
        float lookAngle = AngleBetweenTwoPoints(transform.position, target) + offset;

        //apply target rotation on the z axis
        transform.eulerAngles = new Vector3(0, 0, lookAngle);
    }

    private float AngleBetweenTwoPoints(Vector3 point1, Vector3 point2)
    {
        return Mathf.Atan2(point1.y - point2.y, point1.x - point2.x) * Mathf.Rad2Deg;
    }

    private void BobUpDown()
    {
        //float up/down with a Sin()
        Vector3 pos = startLocalPos;
        pos.y += Mathf.Sin(Time.time * Mathf.PI * frequency) * amplitude;

        transform.localPosition = pos;
    }
}
