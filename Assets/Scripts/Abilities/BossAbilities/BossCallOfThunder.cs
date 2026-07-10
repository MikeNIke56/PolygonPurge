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

    public float minSpawnDistance;
    public float maxSpawnDistance;

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
        lightingObject.GetComponent<BossLightningObj>().damage = damage;
        lightingObject.GetComponent<CircleCollider2D>().radius = range;
        CastLightning();
    }

    public override void UpgradeAbility(int level)
    {
        base.UpgradeAbility(level);
        damage *= damageIncreaseAmnt;
        range *= rangeIncreaseAmnt;
        //numOfLightingStrikes++;
        lightingObject.GetComponent<BossLightningObj>().damage = damage;
        lightingObject.GetComponent<CircleCollider2D>().radius = range;
    }

    private IEnumerator CastLightning()
    {
        isCurrentlyCasting = true;

        //get random position on screen
        int[] infrontOrBehind = new int[2];
        infrontOrBehind[0] = 1;
        infrontOrBehind[1] = -1;

        for (int i = 0; i < numOfLightingStrikes; i++)
        {
            //grab a random point within range of the player
            //then run a 50/50 for whether to make it negative
            int xNegPos = infrontOrBehind[UnityEngine.Random.Range(0, 
                infrontOrBehind.Length)];
            int yNegPos = infrontOrBehind[UnityEngine.Random.Range(0, 
                infrontOrBehind.Length)];

            Vector3 randSpawnPoint = new Vector3(
            UnityEngine.Random.Range(boss.transform.position.x + (minSpawnDistance *
            xNegPos),
            boss.transform.position.x + (maxSpawnDistance * xNegPos)),

            UnityEngine.Random.Range(boss.transform.position.y + (minSpawnDistance *
            yNegPos),
            boss.transform.position.y + (maxSpawnDistance * yNegPos)),

            0);


            //loads in lightning object at that position
            GameObject lightningObjCopy = ObjectPoolingManager.SpawnObject(
                lightingObject, randSpawnPoint, Quaternion.identity,
                ObjectPoolingManager.PoolType.Ability);

            yield return new WaitForSeconds(delayBetweenStrikes);
        }

        isCurrentlyCasting = false;
    }
}
