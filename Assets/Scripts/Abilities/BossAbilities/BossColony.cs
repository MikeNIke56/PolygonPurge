using System.Collections;
using UnityEngine;

public class BossColony: AbilityBaseClass
{
    [Header("Missle Pods Variables")]
    public float damage;
    public float damageIncreaseAmnt;

    public float speed;
    public float speedIncreaseAmnt;

    public float spawnRate;
    public float spawnCooldown;
    public float fireCooldownDecreaseAmnt;

    public GameObject colonyBugObj;

    public int curBugsActive = 0;
    public int maxBugs;

    private bool isOnSpawnCooldown = false;


    protected override void Update()
    {
        if (isOnSpawnCooldown == false && curBugsActive < maxBugs)
            StartCoroutine(SpawnColonyBug());
    }


    public override void SetUp()
    {
        base.SetUp();
        maxBugs = 1;
    }

    public override void UpgradeAbility(int level)
    {
        base.UpgradeAbility(level);
        damage *= damageIncreaseAmnt;
        speed += speedIncreaseAmnt;
        //maxBugs += 2;
        spawnCooldown /= fireCooldownDecreaseAmnt;
    }

    private IEnumerator SpawnColonyBug()
    {
        isOnSpawnCooldown = true;

        for (int i = 0; i < maxBugs; i++)
        {
            /* face the direction the player is moving, 
             * if not moving- face in the last known fire direction if valid
             * if direction isnt valid, just face to the right
             */

            //loads in bug
            GameObject bugObjCopy = ObjectPoolingManager.SpawnObject(
                colonyBugObj, boss.transform.position, 
                Quaternion.identity, ObjectPoolingManager.PoolType.Bullet);

            //sets the speed and damage of the bug
            BossColonyBug bug = bugObjCopy.GetComponent<BossColonyBug>();

            bug.SetColonyParent(this);
            bug.damage = damage;
            bug.speed = speed;
            curBugsActive++;

            yield return new WaitForSecondsRealtime(spawnRate);
        }

        yield return StartColonyBugSpawnCooldown();
    }

    private IEnumerator StartColonyBugSpawnCooldown()
    {
        yield return new WaitForSecondsRealtime(spawnCooldown);
        isOnSpawnCooldown = false;
    }
}
