using NUnit.Framework;
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
    private BossRicochetRoundObj round;

    public override void SetUp()
    {
        base.SetUp();
        SpawnNewRound();
    }

    public override void UpgradeAbility(int level)
    {
        base.UpgradeAbility(level);
        roundSpeed += roundSpeedIncreaseAmnt;
        roundDamage += roundDamageIncreaseAmnt;
        round.speed = roundSpeed;
        round.damage = roundDamage;
    }

    private void SpawnNewRound()
    {
        //spawns in round object
        GameObject roundObjCopy = Instantiate(roundObj, transform);
        BossRicochetRoundObj ricRound = roundObjCopy.GetComponent<
            BossRicochetRoundObj>();

        ricRound.damage = roundDamage;

        //launch round in random direction
        Vector3 randomDir = Random.insideUnitCircle.normalized;
        Vector3 force = randomDir * roundSpeed;
        ricRound.GetRigidbody().AddForce(force, ForceMode2D.Impulse);

        //increases stats of all round
        ricRound.speed = roundSpeed;
        ricRound.damage = roundDamage;
        round = ricRound;
    }
}
