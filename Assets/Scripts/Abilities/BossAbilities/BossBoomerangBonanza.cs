using System.Collections;
using UnityEngine;

public class BossBoomerangBonanza: AbilityBaseClass
{
    [Header("Boomerang Bonanza Variables")]
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
        {
            StartCoroutine(SpawnBoomerangs(false));

            if (boss.GetSpectreRounds())
                StartCoroutine(ShootSpectreRounds());
        }
    }


    public override void SetUp()
    {
        base.SetUp();
        maxBoomerangs = 3;
    }

    public override void UpgradeAbility(int level)
    {
        base.UpgradeAbility(level);
        damage *= damageIncreaseAmnt;
        //maxBoomerangs++;
        spawnCooldown /= spawnCooldownDecreaseAmnt;
    }

    private IEnumerator SpawnBoomerangs(bool isSpectreRound)
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
                boomerangObj, boss.transform.position,
                Quaternion.identity, ObjectPoolingManager.PoolType.Bullet);

            //sets the damage of the boomerang
            BossBoomerang boomerang = boomerangObjCopy.GetComponent<BossBoomerang>();

            boomerang.SetBoomerangParent(this);
            boomerang.damage = damage;
            boomerang.isSpectreRound = isSpectreRound;

            if(isSpectreRound == false)
                curBoomerangActive++;

            yield return new WaitForSecondsRealtime(spawnRate);
        }

        yield return StartBoomerangSpawnCooldown();
    }

    private IEnumerator StartBoomerangSpawnCooldown()
    {
        yield return new WaitForSecondsRealtime(spawnCooldown);
        isOnSpawnCooldown = false;
    }
    private IEnumerator ShootSpectreRounds()
    {
        yield return new WaitForSecondsRealtime(boss.GetSpectreRounds().
            delayAfterFirstShot);

        yield return SpawnBoomerangs(true);
    }
}
