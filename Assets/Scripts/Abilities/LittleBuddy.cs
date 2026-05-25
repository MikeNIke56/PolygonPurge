using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LittleBuddy : AbilityBaseClass
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
    private Vector3 tempPos;
    public float frequency;
    public float amplitude;

    //as long as speed is lower than player velocity- how much the camera
    //"lags" behind
    public float followSpeed;

    public float lookOffset;

    private Transform target;
    private EnemyBaseClass targetedEnemy;
    private bool isOnFireCooldown = false;
    private Transform firepoint;

    private void Awake()
    {
        target = PlayerController.i.littleBuddyPivotPoint.transform;
    }

    private void Start()
    {
        firepoint = GetComponentInChildren<Transform>();
    }

    protected override void Update()
    {
        BobUpDown();

        if (targetedEnemy.gameObject.activeSelf)
            LookAt(targetedEnemy.transform.position, lookOffset);
        else
            targetedEnemy = GetTargetEnemy();

        if (targetedEnemy.gameObject.activeSelf && isOnFireCooldown == false)
            StartCoroutine(Shoot());
    }

    protected override void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, target.position, 
            followSpeed * Time.deltaTime);
    }

    public override void SetUp()
    {
        base.SetUp();
        targetedEnemy = GetTargetEnemy();
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
            //loads in and fires bullet
            GameObject bulletObjCopy = ObjectPoolingManager.SpawnObject(
                littleBuddyBulletObj, firepoint.position,
            firepoint.localRotation, ObjectPoolingManager.PoolType.Bullet);

            //sets the speed and damage of the bullet
            LittleBuddyBullet littleBuddyBullet = bulletObjCopy.
                GetComponent<LittleBuddyBullet>();

            littleBuddyBullet.SetDamage(damage);

            //adjust the angle to account for initial z offset
            Vector3 force = Quaternion.Euler(0, 0, 30f) * firepoint.transform.right
                * littleBuddyBullet.GetSpeed();

            littleBuddyBullet.GetRigidbody().AddForce(-force, ForceMode2D.Impulse);
            curBurstsBeforeRetarget++;
            yield return new WaitForSecondsRealtime(fireRate);
        }
        yield return StartFireCooldown();
    }

    private IEnumerator StartFireCooldown()
    {
        yield return new WaitForSecondsRealtime(fireCooldown);
        isOnFireCooldown = false;

        //if we've locked on to an enemy for 2 full bursts or
        //if we killed the targeted enemy
        if(curBurstsBeforeRetarget >= maxBurstsBeforeRetarget ||
            targetedEnemy.isActiveAndEnabled == false)
        {
            curBurstsBeforeRetarget = 0;
            targetedEnemy = GetTargetEnemy();
        }
    }

    /**
     * Grade enemies based on their rank value and distance from the player.
     * lower rank and lower distance to the player will be prioritized
     */
    private EnemyBaseClass GetTargetEnemy()
    {
        List<EnemyBaseClass> allEnemies = EnemyManager.i.enemyList;
        List<EnemyBaseClass> allEnemiesInRange = new List<EnemyBaseClass>();

        //first get all enemies within range
        foreach (EnemyBaseClass enemy in allEnemies)
        {
            if(enemy.gameObject.activeSelf)
            {
                if (Vector2.Distance(transform.position, enemy.transform.position) <=
                range)
                    allEnemiesInRange.Add(enemy);
            }
        }

        //then choose target from enemies within range based
        //on distance and rank
        EnemyBaseClass selectedEnemy = null;
        float lowestScore = 4 * range;

        foreach (EnemyBaseClass enemy in allEnemiesInRange)
        {
            float enemyScore = Vector2.Distance(transform.position,
                enemy.transform.position) / enemy.GetRank();

            if(enemyScore < lowestScore)
            {
                lowestScore = enemyScore;
                selectedEnemy = enemy;
            }
        }
        return selectedEnemy;
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
        tempPos = transform.localPosition;
        tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;

        transform.localPosition = tempPos;
    }
}
