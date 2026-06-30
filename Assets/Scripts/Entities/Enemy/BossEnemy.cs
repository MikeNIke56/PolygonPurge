using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemy : EnemyBaseClass
{
    //keeps track of the player's/boss's current abilities
    public List<AbilityBaseClass> currentAbilities;

    public override void Setup(PlayerController player)
    {
        base.Setup(player);
    }

    public override void RunBehavior()
    {
        base.RunBehavior();
    }

    public override void HandleMovement()
    {
        float distFromPlayer = Vector2.Distance(transform.position, player.transform.position);

        //if the player is too far from us, then chase to get within range
        Vector3 direction = (player.transform.position - transform.position).
            normalized;
        rb.linearVelocity = direction * moveSpeed * Time.fixedDeltaTime;
    }

    /**
     * sets the boss's abilities from the player's current list of abilities
     */
    public void SetAbilities()
    {
        currentAbilities = new List<AbilityBaseClass>();
        UpgradesManager.i.SpawnBossAbilities(transform, this);
    }

    public override void Die()
    {
        EnemyManager.i.enemyList.Remove(this);
        EnemyManager.i.SetBossRound(false);
        EnemyManager.i.SetWaveCooldown(true);
        base.Die();
    }
}
