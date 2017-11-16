using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FayEnemyNPCAnimationEvents : MonoBehaviour
{
    public bool DamageEnabled = false;

    private Animator _fayEnemyNPCAC;
    private EnemyStats _enemyStats;   

    private void Awake ( )
    {
        _fayEnemyNPCAC = GetComponent<Animator> ();
        _enemyStats = GetComponent<EnemyStats> ();
    }

    public void ResetAttack()
    {
        _fayEnemyNPCAC.SetBool ("IsAttacking", false);
    }

    public void ResetHeavyAttack()
    {
        _fayEnemyNPCAC.SetBool ("HeavyAttacking", false);
    }

    public void EnableDamage()
    {
        DamageEnabled = true;
    }

    public void DisableDamage()
    {
        DamageEnabled = false;
    }

    public void DisableStaminaGainOnAttack()
    {
        _fayEnemyNPCAC.SetBool ("IsAttacking", true);

        if(_enemyStats.CurrentStamina - _enemyStats.AttackStaminaUsage >= 0)
        {
            _enemyStats.CurrentStamina -= _enemyStats.AttackStaminaUsage;
        }
        else
        {
            _enemyStats.CurrentStamina = 0.0f;
        }

        _enemyStats.CanRegainStamina = false;
    }

    public void DisableStaminaGainOnHeavyAttack ()
    {
        _fayEnemyNPCAC.SetBool ("IsHeavyAttacking", true);

        if(_enemyStats.CurrentStamina - _enemyStats.HeavyAttackStaminaUsage >= 0)
        {
            _enemyStats.CurrentStamina -= _enemyStats.HeavyAttackStaminaUsage;
        }
        else
        {

        }
    }
}
