using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines.ExtrusionShapes;

public class BossRicochetRounds: AbilityBaseClass
{
    public float roundSpeed;
    public float roundSpeedIncreaseAmnt;

    public float roundDamage;
    public float roundDamageIncreaseAmnt;

    public GameObject roundObj;
    public List<BossRicochetRoundObj> rounds;

    public override void SetUp()
    {
        base.SetUp();
        SpawnNewRound();

        if (boss.GetSpectreRounds())
            StartCoroutine(ShootSpectreRounds());
    }

    public override void UpgradeAbility(int level)
    {
        base.UpgradeAbility(level);
        roundSpeed += roundSpeedIncreaseAmnt;
        roundDamage += roundDamageIncreaseAmnt;

        foreach(BossRicochetRoundObj round in rounds)
        {
            round.speed = roundSpeed;
            round.damage = roundDamage;
        }
    }

    private void SpawnNewRound()
    {
        //spawns in round object
        GameObject roundObjCopy = Instantiate(roundObj, boss.transform);
        BossRicochetRoundObj ricRound = roundObjCopy.GetComponent<
            BossRicochetRoundObj>();

        rounds.Add(ricRound);
        ricRound.damage = roundDamage;

        //launch round in random direction
        Vector3 randomDir = Random.insideUnitCircle.normalized;
        Vector3 force = randomDir * roundSpeed;
        ricRound.GetRigidbody().AddForce(force, ForceMode2D.Impulse);

        //increases stats of all active rounds
        foreach (BossRicochetRoundObj round in rounds)
        {
            round.speed = roundSpeed;
            round.damage = roundDamage;
        }
    }

    private IEnumerator ShootSpectreRounds()
    {
        yield return new WaitForSeconds(boss.GetSpectreRounds().
            delayAfterFirstShot);

        SpawnNewRound();
    }
}
