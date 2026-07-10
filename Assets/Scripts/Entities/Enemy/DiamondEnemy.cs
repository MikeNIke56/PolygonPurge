using System.Collections;
using UnityEngine;

public class DiamondEnemy : EnemyBaseClass
{
    public float maxChaseDistance;

    //how long to wait before before charging ahead
    public float chargeAttackTime;

    public float chargeAttackCooldown;
    public float chargePower;

    public override void RunBehavior()
    {
        base.RunBehavior();

        LookAt(player.transform.position, 180);
    }

    public override void HandleMovement()
    {
        float distFromPlayer = Vector2.Distance(transform.position, player.transform.position);

        //if the player is too far from us, then chase to get within range
        if(!currentEnemyStates.Contains(EnemyStates.KnockedBack))
        {
            if (distFromPlayer > maxChaseDistance)
            {
                if (!currentEnemyStates.Contains(EnemyStates.Chasing) &&
                    !currentEnemyStates.Contains(EnemyStates.Charging) &&
                    !currentEnemyStates.Contains(EnemyStates.ChargingAttackCooldown))
                {
                    currentEnemyStates.Add(EnemyStates.Chasing);
                }

                if (currentEnemyStates.Contains(EnemyStates.Chasing))
                    ChasePlayer();
            }
            else if (distFromPlayer <= maxChaseDistance)
            {
                //if we're withing charging range
                if (!currentEnemyStates.Contains(EnemyStates.Charging) &&
                    !currentEnemyStates.Contains(EnemyStates.ChargingAttackCooldown))
                {
                    currentEnemyStates.Add(EnemyStates.Charging);
                    currentEnemyStates.Remove(EnemyStates.Chasing);
                    StartCoroutine(ChargeAttack());
                }
            }
        }
    }

    private IEnumerator ChargeAttack()
    {
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(chargeAttackTime);
        Vector3 force = transform.right * chargePower;
        rb.AddForce(force, ForceMode2D.Impulse);
        yield return StartChargeAttackCooldown();
    }

    private IEnumerator StartChargeAttackCooldown()
    {
        currentEnemyStates.Remove(EnemyStates.Charging);
        currentEnemyStates.Add(EnemyStates.ChargingAttackCooldown);
        yield return new WaitForSeconds(chargeAttackCooldown);
        currentEnemyStates.Remove(EnemyStates.ChargingAttackCooldown);
    }
}
