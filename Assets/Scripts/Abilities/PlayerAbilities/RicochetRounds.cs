using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines.ExtrusionShapes;

public class RicochetRounds : AbilityBaseClass
{
    public float roundSpeed;
    public float roundSpeedIncreaseAmnt;

    public float roundDamage;
    public float roundDamageIncreaseAmnt;

    public GameObject roundObj;

    private List<RicochetRoundObj> rounds;

    public override void SetUp()
    {
        base.SetUp();
        rounds = new List<RicochetRoundObj>();
        SpawnNewRound();
    }

    public override void UpgradeAbility(int level)
    {
        base.UpgradeAbility(level);
        roundSpeed += roundSpeedIncreaseAmnt;
        roundDamage += roundDamageIncreaseAmnt;
        SpawnNewRound();
    }

    private void SpawnNewRound()
    {
        //spawns in round object
        GameObject roundObjCopy = Instantiate(roundObj, transform);
        RicochetRoundObj ricRound = roundObjCopy.GetComponent<RicochetRoundObj>();

        rounds.Add(ricRound);

        ricRound.damage = roundDamage;

        //launch round in random direction
        Vector3 randomDir = Random.insideUnitCircle.normalized;
        Vector3 force = randomDir * roundSpeed;
        ricRound.GetRigidbody().AddForce(force, ForceMode2D.Impulse);

        //increases stats of all active rounds
        foreach (RicochetRoundObj round in rounds)
        {
            round.speed = roundSpeed;
            round.damage = roundDamage;
        }
    }
}
