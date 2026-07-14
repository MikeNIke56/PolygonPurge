using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbyssalPortals : AbilityBaseClass
{
    public float damage;
    public float damageIncreaseAmnt;

    public float range;
    public float blackholeVFXSize;
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
        blackHole.blackholeVFXSize = blackholeVFXSize;
        SummonBlackHole();
    }

    public override void UpgradeAbility(int level)
    {
        base.UpgradeAbility(level);
        damage *= damageIncreaseAmnt;
        range *= rangeIncreaseAmnt;
        blackholeVFXSize *= rangeIncreaseAmnt;
        pullStrength *= pullStrengthIncreaseAmnt;
        numOfBlackHoles++;

        AbyssalPortalBlackHoleObj[] blackHoles = FindObjectsByType<
            AbyssalPortalBlackHoleObj>(FindObjectsInactive.Include);

        foreach (AbyssalPortalBlackHoleObj blackHole in blackHoles)
        {
            blackHole.damage = damage;
            blackHole.pullStrength = pullStrength;
            blackHole.blackholeVFXSize = blackholeVFXSize;
            blackHoleObject.GetComponent<CircleCollider2D>().radius = range;
        }  
    }

    private IEnumerator SummonBlackHole()
    {
        isCurrentlyCasting = true;

        for (int i = 0; i < numOfBlackHoles; i++)
        {
            //loads in blackHole object at that position
            GameObject blackHoleObjCopy = ObjectPoolingManager.SpawnObject(
                blackHoleObject, FindRandomSpawnPointInArena(), Quaternion.identity,
                ObjectPoolingManager.PoolType.Ability);

            blackHoleObjCopy.GetComponent<
                AbyssalPortalBlackHoleObj>().blackholeVFXSize = blackholeVFXSize;

            yield return new WaitForSeconds(delayBetweenSummons);

            Debug.Log("black hole");
        }

        isCurrentlyCasting = false;
    }
}
