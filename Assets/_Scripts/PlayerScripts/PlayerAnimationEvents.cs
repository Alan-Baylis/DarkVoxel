using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour {

    public bool DamageEnabled = false;
    public bool CanGetDamaged = true;

    public List<EnemyStats> EnemiesHitThisAttack = new List<EnemyStats>();

    private Animator _playerAC;
    private PlayerStats _playerStats;

    private void Awake ( )
    {
        _playerAC = GetComponent<Animator> ();
        _playerStats = GetComponent<PlayerStats> ();
    }

    public void DisableInput( )
    {
        _playerAC.SetBool ("InputEnabled", false);      //disables input during rolling
        _playerAC.SetBool ("RollEnabled", false);       //disables rolling input during rolling
    }

    public void EnableInput ( )
    {
        _playerAC.SetBool ("InputEnabled", true);       //enables input 
    } 
    
    public void EnableRolling()
    {
        _playerAC.SetBool ("RollEnabled", true);        //enables rolling input
    }

    public void StartRolling( )
    {
        _playerAC.SetBool ("RollFinished", false);       //disables rolling in the middle of roll animation
        CanGetDamaged = false;
    }
    
    public void StopRolling( )
    {
        _playerAC.SetBool ("RollFinished", true);
        CanGetDamaged = true;
    }

    public void ResetRollAttack()
    {
        _playerAC.SetBool ("RollAttack", false);
    }

    public void ResetAttack()
    {
        _playerAC.SetBool ("Attacking", false);
    }

    public void ResetHeavyAttack()
    {
        _playerAC.SetBool ("HeavyAttacking", false);
    }

    public void EnableDamage()                          //Enables dealing damage to enemies while certain frames of attack animation are playing
    {
        DamageEnabled = true;
    }

    public void DisableDamage()                         //Disables accidental damage to enemies if player isn't attacking
    {
        DamageEnabled = false;

        foreach (EnemyStats enemy in EnemiesHitThisAttack)
        {
            enemy.HitThisAttack = false;
        }
        EnemiesHitThisAttack.Clear ();
    }

    public void EnableStaminaGain()
    {
        PlayerStats.instance.CanRegainStamina = true;
    }

    public void DisableStaminaGainOnRoll()
    {
        if (_playerStats.CurrentStamina - _playerStats.RollStaminaUsage >= 0)
        {
            _playerStats.CurrentStamina -= _playerStats.RollStaminaUsage;
        }
        else
        {
            _playerStats.CurrentStamina = 0.0f;
        }

        PlayerStats.instance.CanRegainStamina = false;
    }

    public void DisableStaminaGainOnAttack ( )
    {
        _playerAC.SetBool ("Attacking", true);
        _playerAC.SetBool ("AttackQeued", false);

        if (_playerStats.CurrentStamina - _playerStats.AttackStaminaUsage >= 0)
        {
            _playerStats.CurrentStamina -= _playerStats.AttackStaminaUsage;
        }
        else
        {
            _playerStats.CurrentStamina = 0.0f;
        }

        PlayerStats.instance.CanRegainStamina = false;
    }

    public void DisableStaminaGainOnHeavyAttack ()
    {
        _playerAC.SetBool ("HeavyAttacking", true);
        _playerAC.SetBool ("AttackQeued", false);

        if (_playerStats.CurrentStamina - _playerStats.HeavyAttackStaminaUsage >= 0)
        {
            _playerStats.CurrentStamina -= _playerStats.HeavyAttackStaminaUsage;
        }
        else
        {
            _playerStats.CurrentStamina = 0.0f;
        }

        PlayerStats.instance.CanRegainStamina = false;
    }

    public void Heal()
    {
        _playerStats.Heal (_playerStats.HealthRecovered);
    }
}
