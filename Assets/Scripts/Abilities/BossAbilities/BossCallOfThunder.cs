using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossCallOfThunder: AbilityBaseClass
{
    public float damage;
    public float damageIncreaseAmnt;

    public float range;
    public float rangeIncreaseAmnt;

    public float delayBetweenStrikes;

    private float curCastCooldown;
    public float castCooldown;
    private bool isCurrentlyCasting = false;

    public int numOfLightingStrikes = 1;

    //lightning strike visual to spawn at position
    public GameObject lightingObject;
    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
    }

    protected override void Update()
    {
        //only countdown when casting is on cooldown
        if(isCurrentlyCasting == false)
        {
            curCastCooldown -= Time.deltaTime;

            if (curCastCooldown <= 0)
            {
                curCastCooldown = castCooldown;
                StartCoroutine(CastLightning());
            }
        }
    }

    public override void SetUp()
    {
        base.SetUp();
        curCastCooldown = castCooldown;
        lightingObject.GetComponent<LightningObj>().damage = damage;
        lightingObject.GetComponent<CircleCollider2D>().radius = range;
        CastLightning();
    }

    public override void UpgradeAbility(int level)
    {
        base.UpgradeAbility(level);
        damage *= damageIncreaseAmnt;
        range *= rangeIncreaseAmnt;
        numOfLightingStrikes++;
        lightingObject.GetComponent<LightningObj>().damage = damage;
        lightingObject.GetComponent<CircleCollider2D>().radius = range;
    }

    private IEnumerator CastLightning()
    {
        isCurrentlyCasting = true;

        //convert screen resolution to in-game world positions
        Vector2 screenMin = cam.ViewportToWorldPoint(new Vector2(0, 0));
        Vector2 screenMax = cam.ViewportToWorldPoint(new Vector2(1, 1));

        List<EnemyBaseClass> allEnemies = EnemyManager.i.enemyList;
        List<EnemyBaseClass> allEnemiesOnScreen = new List<EnemyBaseClass>();

        foreach(EnemyBaseClass enemy in allEnemies)
        {
            if(enemy.transform.position.x < screenMax.x &&
                enemy.transform.position.x > screenMin.x &&
                enemy.transform.position.y < screenMax.y &&
                enemy.transform.position.y > screenMin.y)
            {
                allEnemiesOnScreen.Add(enemy);
            }
        }

        for (int i = 0; i < numOfLightingStrikes; i++)
        {
            int randomNum = UnityEngine.Random.Range(0, allEnemiesOnScreen.Count);
            EnemyBaseClass selectedEnemy = allEnemiesOnScreen[randomNum];

            //loads in lightning object
            GameObject lightningObjCopy = ObjectPoolingManager.SpawnObject(
                lightingObject, selectedEnemy.transform.position,
                Quaternion.identity, 
                ObjectPoolingManager.PoolType.Ability);

            yield return new WaitForSecondsRealtime(delayBetweenStrikes);

            Debug.Log("lightning");
        }

        isCurrentlyCasting = false;
    }
}
