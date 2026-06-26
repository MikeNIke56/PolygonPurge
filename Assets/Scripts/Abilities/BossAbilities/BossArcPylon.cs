using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossArcPylon: AbilityBaseClass
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
    private Vector3 tempPos;
    public float frequency;
    public float amplitude;

    //as long as speed is lower than player velocity- how much the camera
    //"lags" behind
    public float followSpeed;

    private Transform followTarget;
    private bool isOnFireCooldown = false;

    private void Awake()
    {
        followTarget = PlayerController.i.arcPylonPivotPoint.transform;
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

        foreach(EnemyBaseClass chainedEnemy in chainedEnemies)
            chainedEnemy.TakeDamage(Time.deltaTime * damage);
    }

    private IEnumerator StartFireCooldown()
    {
        isOnFireCooldown = true;

        yield return new WaitForSecondsRealtime(fireCooldown);

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
        tempPos = transform.localPosition;
        tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;

        transform.localPosition = tempPos;
    }
}
