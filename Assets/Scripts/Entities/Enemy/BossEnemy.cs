using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemy : EnemyBaseClass
{
    //modes boss will periodically swap between
    public enum BossModes
    {
        None,
        ChargeMode,
        BulletHellMode,
        AbilityCopy
    }
    public BossModes currentMode;

    [Header("Charge Attack Variables")]
    public float maxChaseDistance;
    public float chargeAttackTime;
    public float chargeAttackCooldown;
    public float chargePower;

    [Header("Bullet Hell Variables")]
    public float rotateSpeed;
    public GameObject bulletObj;



    //keeps track of the player's/boss's current abilities
    public List<AbilityBaseClass> currentAbilities;

    public override void Setup(PlayerController player)
    {
        base.Setup(player);

        //copy player's current abilities into boss's
        currentAbilities = new List<AbilityBaseClass>();
        currentAbilities = UpgradesManager.i.currentAbilities;
    }

    public override void RunBehavior()
    {
        base.RunBehavior();
        HandleModeBehaviors();
    }

    public override void HandleMovement()
    {
        float distFromPlayer = Vector2.Distance(transform.position, player.transform.position);

        //if the player is too far from us, then chase to get within range
        Vector3 direction = (player.transform.position - transform.position).
            normalized;
        rb.linearVelocity = direction * moveSpeed * Time.fixedDeltaTime;
    }

    private void HandleModeBehaviors()
    {

    }

    private IEnumerator ChargeAttack()
    {
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSecondsRealtime(chargeAttackTime);
        Vector3 force = transform.right * chargePower;
        rb.AddForce(force, ForceMode2D.Impulse);
        yield return StartChargeAttackCooldown();
    }

    private IEnumerator StartChargeAttackCooldown()
    {
        currentEnemyStates.Remove(EnemyStates.Charging);
        currentEnemyStates.Add(EnemyStates.ChargingAttackCooldown);
        yield return new WaitForSecondsRealtime(chargeAttackCooldown);
        currentEnemyStates.Remove(EnemyStates.ChargingAttackCooldown);
    }

    public override void Die()
    {
        EnemyManager.i.enemyList.Remove(this);
        EnemyManager.i.SetBossRound(false);
        EnemyManager.i.SetWaveCooldown(true);
        base.Die();
    }
}
