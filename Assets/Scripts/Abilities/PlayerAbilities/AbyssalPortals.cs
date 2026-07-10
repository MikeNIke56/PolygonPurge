using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbyssalPortals : AbilityBaseClass
{
    public float damage;
    public float damageIncreaseAmnt;

    public float range;
    public float rangeIncreaseAmnt;

    public float minSpawnDistance;
    public float maxSpawnDistance;

    public float pullStrength;
    public float pullStrengthIncreaseAmnt;

    public float delayBetweenSummons;

    private float curCastCooldown;
    public float castCooldown;
    private bool isCurrentlyCasting = false;

    public int numOfBlackHoles = 1;

    //blackhole visual to spawn at position
    public GameObject blackHoleObject;
    private Camera cam;


    private void Awake()
    {
        cam = Camera.main;
    }

    protected override void Update()
    {
        //only countdown when casting is on cooldown
        if (isCurrentlyCasting == false)
        {
            curCastCooldown -= Time.deltaTime;

            if (curCastCooldown <= 0)
            {
                curCastCooldown = castCooldown;
                StartCoroutine(SummonBlackHole());
            }
        }
    }

    public override void SetUp()
    {
        base.SetUp();
        curCastCooldown = castCooldown;

        AbyssalPortalBlackHoleObj blackHole = blackHoleObject.
            GetComponent<AbyssalPortalBlackHoleObj>();

        blackHole.damage = damage;
        blackHole.pullStrength = pullStrength;
        blackHoleObject.GetComponent<CircleCollider2D>().radius = range;
        SummonBlackHole();
    }

    public override void UpgradeAbility(int level)
    {
        base.UpgradeAbility(level);
        damage *= damageIncreaseAmnt;
        range *= rangeIncreaseAmnt;
        pullStrength *= pullStrengthIncreaseAmnt;
        numOfBlackHoles++;

        AbyssalPortalBlackHoleObj blackHole = blackHoleObject.
            GetComponent<AbyssalPortalBlackHoleObj>();

        blackHole.damage = damage;
        blackHole.pullStrength = pullStrength;
        blackHoleObject.GetComponent<CircleCollider2D>().radius = range;
    }

    private IEnumerator SummonBlackHole()
    {
        isCurrentlyCasting = true;
        PlayerController player = PlayerController.i;

        //get random position on screen
        int[] infrontOrBehind = new int[2];
        infrontOrBehind[0] = 1;
        infrontOrBehind[1] = -1;

        for (int i = 0; i < numOfBlackHoles; i++)
        {
            //grab a random point within range of the player
            //then run a 50/50 for whether to make it negative
            int xNegPos = infrontOrBehind[Random.Range(0, infrontOrBehind.Length)];
            int yNegPos = infrontOrBehind[Random.Range(0, infrontOrBehind.Length)];

            Vector3 randSpawnPoint = new Vector3(
            Random.Range(player.transform.position.x + (minSpawnDistance *
            xNegPos),
            player.transform.position.x + (maxSpawnDistance * xNegPos)),

            Random.Range(player.transform.position.y + (minSpawnDistance *
            yNegPos),
            player.transform.position.y + (maxSpawnDistance * yNegPos)),

            0);


            //loads in blackHole object at that position
            GameObject blackHoleObjCopy = ObjectPoolingManager.SpawnObject(
                blackHoleObject, randSpawnPoint, Quaternion.identity,
                ObjectPoolingManager.PoolType.Ability);

            yield return new WaitForSeconds(delayBetweenSummons);

            Debug.Log("black hole");
        }

        isCurrentlyCasting = false;
    }
}
