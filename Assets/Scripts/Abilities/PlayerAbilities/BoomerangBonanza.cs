using System.Collections;
using UnityEngine;

public class BoomerangBonanza : AbilityBaseClass
{
    [Header("Missle Pods Variables")]
    public float damage;
    public float damageIncreaseAmnt;

    public float spawnRate;
    public float spawnCooldown;
    public float spawnCooldownDecreaseAmnt;

    public GameObject boomerangObj;

    public int curBoomerangActive = 0;
    public int maxBoomerangs;

    private bool isOnSpawnCooldown = false;


    protected override void Update()
    {
        if (isOnSpawnCooldown == false && curBoomerangActive < maxBoomerangs)
            StartCoroutine(SpawnColonyBug());
    }


    public override void SetUp()
    {
        base.SetUp();
        maxBoomerangs = 1;
    }

    public override void UpgradeAbility(int level)
    {
        base.UpgradeAbility(level);
        damage *= damageIncreaseAmnt;
        maxBoomerangs++;
        spawnCooldown /= spawnCooldownDecreaseAmnt;
    }

    private IEnumerator SpawnColonyBug()
    {
        isOnSpawnCooldown = true;

        for (int i = 0; i < maxBoomerangs; i++)
        {
            /* face the direction the player is moving, 
             * if not moving- face in the last known fire direction if valid
             * if direction isnt valid, just face to the right
             */

            //loads in boomerang
            GameObject boomerangObjCopy = ObjectPoolingManager.SpawnObject(
                boomerangObj, PlayerController.i.transform.position,
                Quaternion.identity, ObjectPoolingManager.PoolType.Bullet);

            //sets the damage of the boomerang
            Boomerang boomerang = boomerangObjCopy.GetComponent<Boomerang>();

            boomerang.SetBoomerangParent(this);
            boomerang.damage = damage;
            curBoomerangActive++;

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
