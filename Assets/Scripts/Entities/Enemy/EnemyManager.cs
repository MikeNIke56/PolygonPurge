using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using System.Linq;
using UnityEngine;

/**
* manager class for all enemies spawned in
*/
public class EnemyManager : MonoBehaviour
{
    [Header("Enemy Spawn Variables")]
    public List<EnemyBaseClass> enemySpawnList;
    private BossEnemy boss;
    public GameObject bossObj;
    [SerializeField] private int maxNumOfEnemies;
    [SerializeField] private int maxNumOfEnemiesPerRound;
    public int curNumOfEnemies = 0;
    public float regularSpawnDelay;
    public float bossSpawnDelay;
    public float minSpawnDistance;
    public float maxSpawnDistance;

    [SerializeField] private bool isNextWaveOnCooldown;
    private float roundTimer;
    [SerializeField] private float maxRoundTimer;
    private int roundsToBossSpawn;
    public int maxRoundsToBossSpawn;
    private bool isBossRound = false;

    public List<List<EnemyBaseClass>> enemyBatchLists;
    private int curEnemyBatch = 0;
    private int curEnemyBatchToTickBehavior = 0;
    private int curEnemyBatchToTickMovement = 0;

    //list of enemy class objects
    public List<EnemyBaseClass> enemyList;

    private PlayerController player;

    [Header("Enemy Spacing Variables")]
    public float separationDistance = 1.5f;
    public float separationStrength = 5f;
    private SpatialGrid spatialGrid;

    public static EnemyManager i { get; private set; }

    private void Awake()
    {
        if (i != null)
        {
            Destroy(gameObject);
        }
        else
        {
            i = this;
            DontDestroyOnLoad(gameObject);
        }

        spatialGrid = GetComponent<SpatialGrid>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spatialGrid.SetGrid(separationDistance);
        player = FindAnyObjectByType<PlayerController>();

        enemyBatchLists = new List<List<EnemyBaseClass>>();
        for(int i = 0; i < maxNumOfEnemies/maxNumOfEnemiesPerRound; i++)
        {
            List<EnemyBaseClass> innerList = new List<EnemyBaseClass>();
            enemyBatchLists.Add(innerList);
        }

        isNextWaveOnCooldown = true;
        roundTimer = maxRoundTimer;
        roundsToBossSpawn = maxRoundsToBossSpawn;

        int totalChance = 0;
        foreach (EnemyBaseClass record in enemySpawnList)
        {
            record.SetLowerChance(totalChance);
            record.SetUpperChance(totalChance + record.GetChanceToSpawn());

            totalChance = totalChance + record.GetChanceToSpawn();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isBossRound == false)
        {
            UpdateBatchedEnemiesBehavior();
            HandleRounds();
        }
        else
        {
            if (boss && boss.gameObject.activeSelf == true)
            {
                if (boss.GetCurHealth() > 0)
                    boss.RunBehavior();
            }
        }
    }

    private void FixedUpdate()
    {
        if (isBossRound == false)
            UpdateBatchedEnemiesMovement();
        else
        {
            if (boss && boss.gameObject.activeSelf == true)
            {
                if (boss.GetCurHealth() > 0)
                    boss.HandleMovement();
            }
        }
    }

    /**
     * updates enemy behavior logic (state changes, triggers, etc.)- to be ran in Update
     */
    private void UpdateBatchedEnemiesBehavior()
    {
        //only tick a batch of enemies at a time
        List<EnemyBaseClass> bucket = enemyBatchLists[curEnemyBatchToTickBehavior];

        for (int i = 0; i < enemyBatchLists[curEnemyBatchToTickBehavior].Count;
            i++)
        {
            if (bucket[i] && bucket[i].gameObject.activeSelf == true)
            {
                if (bucket[i].GetCurHealth() > 0)
                    bucket[i].RunBehavior();
            }      
        }

        if (curEnemyBatchToTickBehavior < (maxNumOfEnemies / 
            maxNumOfEnemiesPerRound) - 1)
            curEnemyBatchToTickBehavior++;
        else
            curEnemyBatchToTickBehavior = 0;
    }

    /**
     * updates enemy movement, because physics-based- to be ran in FixedUpdate
     */
    private void UpdateBatchedEnemiesMovement()
    {
        //rebuild grid each frame
        spatialGrid.Clear();
        foreach (EnemyBaseClass enemy in enemyList)
            spatialGrid.Insert(enemy);

        List<EnemyBaseClass> bucket = enemyBatchLists[curEnemyBatchToTickMovement];

        for (int i = 0; i < enemyBatchLists[curEnemyBatchToTickMovement].Count; i++)
        {
            //only check nearby enemies and space from them
            if(bucket[i] && bucket[i].gameObject.activeSelf == true)
            {
                if(bucket[i].GetCurHealth() > 0)
                {
                    Vector2 separation = Vector2.zero;
                    List<EnemyBaseClass> neighbors = spatialGrid.GetNeighbors(
                        bucket[i].transform.position);

                    foreach (EnemyBaseClass other in neighbors)
                    {
                        if (other == bucket[i]) continue;

                        Vector2 diff = bucket[i].transform.position - 
                            other.transform.position;
                        float dist = diff.magnitude;

                        //closer = stronger push
                        if (dist < separationDistance && dist > 0)
                            separation += diff.normalized / dist;
                    }

                    bucket[i].HandleMovement();
                    bucket[i].ApplySeparation(separation.normalized * 
                        separationStrength);
                }
            }
        }

        if (curEnemyBatchToTickMovement < (maxNumOfEnemies / 
            maxNumOfEnemiesPerRound) - 1)
            curEnemyBatchToTickMovement++;
        else
            curEnemyBatchToTickMovement = 0;
    }

    /**
     * spawns in appropriate enemies from list with delay between each spawn
     */
    private IEnumerator SpawnInEnemies()
    {
        if(isNextWaveOnCooldown == false)
        {
            //find how many enemies we need to spawn per round
            int numOfEnemiesToSpawn = maxNumOfEnemies - curNumOfEnemies;

            if (numOfEnemiesToSpawn > maxNumOfEnemiesPerRound)
                numOfEnemiesToSpawn = maxNumOfEnemiesPerRound;

            int currentNumOfEnemiesSpwnedThisRound = 0;
            while (currentNumOfEnemiesSpwnedThisRound < numOfEnemiesToSpawn)
            {
                //return the first enemy where the value "p" is between their lower
                //and upper spawn chances
                int randVal = Random.Range(0, 101);
                EnemyBaseClass enemyRecorded = enemySpawnList.First(p => randVal 
                >= p.GetLowerChance() && randVal <= p.GetUpperChance());

                //spawn in that enemy
                GameObject enemyGameObjectCopy = ObjectPoolingManager.SpawnObject(
                    enemyRecorded.gameObject, FindRandomSpawnPoint(
                        minSpawnDistance, maxSpawnDistance), 
                    Quaternion.identity, ObjectPoolingManager.PoolType.Enemy);

                EnemyBaseClass enemy = enemyGameObjectCopy.GetComponent<
                    EnemyBaseClass>();
                enemy.Setup(player);
                enemyList.Add(enemy);
                AddEnemyToBucket(enemy, curEnemyBatch);
                currentNumOfEnemiesSpwnedThisRound++;
                curNumOfEnemies++;
                yield return new WaitForSecondsRealtime(regularSpawnDelay);
            }

            if (curEnemyBatch < maxNumOfEnemies / maxNumOfEnemiesPerRound)
                curEnemyBatch++;
            else
                curEnemyBatch = 0;

            isNextWaveOnCooldown = true;
        }   
    }

    /**
     * despawns all enemies, then spawns in boss
     */
    private IEnumerator SpawnInBoss()
    {
        if (isNextWaveOnCooldown == false)
        {
            roundsToBossSpawn = maxRoundsToBossSpawn;

            //despawn every enemy
            foreach (EnemyBaseClass enemy in enemyList)
            {
                if (enemy && enemy.gameObject.activeSelf == true)
                    enemy.Despawn();
            }
            enemyList.Clear();

            //spawn in boss
            GameObject bossGameObjectCopy = ObjectPoolingManager.SpawnObject(
                bossObj, FindRandomSpawnPoint(minSpawnDistance / 3, 
                maxSpawnDistance / 3), Quaternion.identity, 
                ObjectPoolingManager.PoolType.Enemy);

            BossEnemy bossCopy = bossGameObjectCopy.GetComponent<BossEnemy>();
            bossCopy.Setup(player);
            bossCopy.SetAbilities();
            enemyList.Add(bossCopy);
            curNumOfEnemies = 1;
            boss = bossCopy;

            yield return new WaitForSecondsRealtime(bossSpawnDelay);

            isBossRound = true;
        }
        yield return null;
    }

    /**
     * adds enemy to buckets to be updated seperately
     */
    private void AddEnemyToBucket(EnemyBaseClass enemy, int batch)
    {
        if(batch < maxNumOfEnemies / maxNumOfEnemiesPerRound)    
            enemyBatchLists[curEnemyBatch].Add(enemy);
    }

    /**
     * finds random point around character to spawn the enemy
     */
    private Vector3 FindRandomSpawnPoint(float minDist, float maxDist)
    {
        //grab a random point within a "belt" range of the player
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float randDistance = Random.Range(minDist, maxDist);

        Vector3 randSpawnPoint = new Vector3(
            Mathf.Cos(angle) * randDistance,
            Mathf.Sin(angle) * randDistance,
            0);
        return randSpawnPoint;
    }

    /**
     * handles rounds and calling the SpawnInEnemies function
     */
    private void HandleRounds()
    {
        if (isNextWaveOnCooldown == true)
        {
            roundTimer -= Time.deltaTime;

            if (roundTimer <= 0)
            {
                isNextWaveOnCooldown = false;
                roundTimer = maxRoundTimer;
                roundsToBossSpawn--;

                if (roundsToBossSpawn > 0)
                    StartCoroutine(SpawnInEnemies());
                else
                    StartCoroutine(SpawnInBoss());
            }
        }
    }

    public void SetBossRound(bool isBossRound)
    {
        this.isBossRound = isBossRound;
    }
    public void SetWaveCooldown(bool isWaveOnCD)
    {
        isNextWaveOnCooldown = isWaveOnCD;
    }
}
