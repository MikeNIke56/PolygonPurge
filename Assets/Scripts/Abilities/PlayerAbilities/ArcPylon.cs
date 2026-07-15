using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcPylon : AbilityBaseClass
{
    //the layers of objects this object is allowed to apply physics to
    public LayerMask targetLayers;

    [Header("Arc Pylon Variables")]
    public float damage;
    public float damageIncreaseAmnt;

    public float fireCooldown;
    public float fireCooldownDecreaseAmnt;

    public float range;
    public float rangeIncreaseAmnt;
    public float chainRange;
    public float chainRangeIncreaseAmnt;

    public float maxChainCount;

    private float curCastTime;
    public float maxCastTime;

    public List<EnemyBaseClass> chainedEnemies;
    private EnemyBaseClass targetedEnemy;

    public GameObject arcChainObj;

    [Header("Bob Up/Down Variables")]
    private Vector3 startLocalPos;
    public float frequency;
    public float amplitude;

    //as long as speed is lower than player velocity- how much the camera
    //"lags" behind
    public float followSpeed;

    private Transform followTarget;
    private bool isOnFireCooldown = false;

    private void Start()
    {
        followTarget = PlayerController.i.arcPylonPivotPoint.transform;
        startLocalPos = transform.localPosition;
    }

    protected override void Update()
    {
        BobUpDown();

        if(chainedEnemies.Count > 0)
        {
            //if any enemy is dead, the chain is broken and must be restarted
            for(int i = 0; i < chainedEnemies.Count; i++)
            {
                if (!chainedEnemies[i] || !chainedEnemies[i].isActiveAndEnabled)
                {
                    chainedEnemies.Clear();
                    targetedEnemy = GetTargetEnemy();
                    chainedEnemies.Add(targetedEnemy);
                }
            }
        } 

        if (targetedEnemy && targetedEnemy.gameObject.activeSelf && 
            isOnFireCooldown == false)
        {
            curCastTime -= Time.deltaTime;

            if (curCastTime <= 0f)
                StartCoroutine(StartFireCooldown());
            else
                Shoot();
        }
    }

    protected override void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, followTarget.position,
            followSpeed * Time.deltaTime);
    }

    public override void SetUp()
    {
        base.SetUp();
        curCastTime = maxCastTime;
        chainedEnemies = new List<EnemyBaseClass>();
        targetedEnemy = GetTargetEnemy();
        chainedEnemies.Add(targetedEnemy);
    }

    public override void UpgradeAbility(int level)
    {
        base.UpgradeAbility(level);
        damage *= damageIncreaseAmnt;
        range += rangeIncreaseAmnt;
        chainRange += chainRangeIncreaseAmnt;
        fireCooldown /= fireCooldownDecreaseAmnt;
        maxChainCount++;
    }

    /**
     * applies tick damage to all chained enemies for a set a duration
     */
    private void Shoot()
    {
        if(chainedEnemies.Count < maxChainCount)
        {
            //get the last chained enemy to chain from
            EnemyBaseClass lastChainedEnemy = chainedEnemies[
                chainedEnemies.Count - 1];

            //get all enemies within chain distance
            Collider2D[] enemiesInChainRange = Physics2D.OverlapCircleAll(
                lastChainedEnemy.transform.position, chainRange, targetLayers);

            //only consider enemies not already chained
            List<EnemyBaseClass> enemiesNotChained = new List<EnemyBaseClass>();

            foreach (Collider2D enemy in enemiesInChainRange)
            {
                if(!chainedEnemies.Contains(enemy.gameObject.
                    GetComponent<EnemyBaseClass>()))
                {
                    enemiesNotChained.Add(enemy.gameObject.
                    GetComponent<EnemyBaseClass>());
                }
            }

            if(enemiesNotChained.Count > 0)
            {
                chainedEnemies.Add(enemiesNotChained[
               Random.Range(0, enemiesNotChained.Count)]);
            }
        }

        GameObject arcChainCopy = null;
        //spawn chain lightning object from player to first enemy

        //loads in lightning chain
        arcChainCopy = ObjectPoolingManager.SpawnObject(
            arcChainObj, transform.position,
            Quaternion.identity, ObjectPoolingManager.PoolType.Ability);

        arcChainCopy.GetComponent<ChainLightning>().SetPosition(transform,
            chainedEnemies[0].transform);

        //spawn chain lightning object between enemies
        for (int i = 0; i < chainedEnemies.Count-1; i++)
        {
            //find the in between position
            Vector3 spawnPos = (chainedEnemies[i].transform.position +
                chainedEnemies[i + 1].transform.position) / 2;

            //loads in lightning chain
            arcChainCopy = ObjectPoolingManager.SpawnObject(
                arcChainObj, transform.position,
                Quaternion.identity, ObjectPoolingManager.PoolType.Ability);

            arcChainCopy.GetComponent<ChainLightning>().SetPosition(
                chainedEnemies[i].transform,
                chainedEnemies[i+1].transform);
        }

        foreach(EnemyBaseClass chainedEnemy in chainedEnemies)
            chainedEnemy.TakeDamage(Time.deltaTime * damage);
    }

    private IEnumerator StartFireCooldown()
    {
        isOnFireCooldown = true;

        yield return new WaitForSeconds(fireCooldown);

        isOnFireCooldown = false;
        curCastTime = maxCastTime;
        chainedEnemies.Clear();
        targetedEnemy = GetTargetEnemy();
        chainedEnemies.Add(targetedEnemy);
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
            if (enemy.gameObject.activeSelf)
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

            if (enemyScore < lowestScore)
            {
                lowestScore = enemyScore;
                selectedEnemy = enemy;
            }
        }
        return selectedEnemy;
    }

    private void BobUpDown()
    {
        //float up/down with a Sin()
        Vector3 pos = startLocalPos;
        pos.y += Mathf.Sin(Time.time * Mathf.PI * frequency) * amplitude;

        transform.localPosition = pos;
    }

    private void AdjustVFXSpawnRotation(ParticleSystem source, Vector3 target, 
        float offset)
    {
        //find angle between the player and target
        float lookAngle = AngleBetweenTwoPoints(source.transform.position, 
            target) + offset;

        //apply target rotation on the z axis
        var tempSource = source.main;
        tempSource.startRotation = Mathf.Deg2Rad * lookAngle;
    }

    private float AngleBetweenTwoPoints(Vector3 point1, Vector3 point2)
    {
        return Mathf.Atan2(point1.y - point2.y, point1.x - point2.x) * 
            Mathf.Rad2Deg;
    }
}
