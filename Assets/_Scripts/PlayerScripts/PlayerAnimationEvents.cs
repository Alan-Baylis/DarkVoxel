﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{

    public GameObject LevelUpMedalion;
    public GameObject HealParticles;

    public bool DamageEnabled = false;
    public bool CanGetDamaged = true;

    public List<EnemyStats> EnemiesHitThisAttack = new List<EnemyStats> ();

    private Animator _playerAC;
    private PlayerStats _playerStats;
    private GameManager _gameManager;
    private SoundManager _soundManager;

    private int _stepCycle = 0;

    private void Awake ( )
    {
        _playerAC = GetComponent<Animator> ();
        _playerStats = GetComponent<PlayerStats> ();
    }

    private void Start ( )
    {
        _gameManager = GameManager.instance;
        _soundManager = SoundManager.instance;
    }

    public void DisableInput ( )
    {
        _playerAC.SetBool ("InputEnabled", false);      //disables input during rolling
        _playerAC.SetBool ("RollEnabled", false);       //disables rolling input during rolling
    }

    public void EnableInput ( )
    {
        _playerAC.SetBool ("InputEnabled", true);       //enables input 
    }

    public void EnableRolling ( )
    {
        _playerAC.SetBool ("RollEnabled", true);        //enables rolling input
    }

    public void StartRolling ( )
    {
        _playerAC.SetBool ("RollFinished", false);       //disables rolling in the middle of roll animation
        CanGetDamaged = false;
    }

    public void StopRolling ( )
    {
        _playerAC.SetBool ("RollFinished", true);
        CanGetDamaged = true;
    }

    public void ResetRollAttack ( )
    {
        _playerAC.SetBool ("RollAttack", false);
    }

    public void ResetAttack ( )
    {
        _playerAC.SetBool ("Attacking", false);
    }

    public void ResetHeavyAttack ( )
    {
        _playerAC.SetBool ("HeavyAttacking", false);
    }

    public void EnableDamage ( )                          //Enables dealing damage to enemies while certain frames of attack animation are playing
    {
        DamageEnabled = true;
    }

    public void DisableDamage ( )                         //Disables accidental damage to enemies if player isn't attacking
    {
        DamageEnabled = false;

        foreach (EnemyStats enemy in EnemiesHitThisAttack)
        {
            enemy.HitThisAttack = false;
        }
        EnemiesHitThisAttack.Clear ();
    }

    public void EnableStaminaGain ( )
    {
        PlayerStats.instance.CanRegainStamina = true;
    }

    public void DisableStaminaGainOnRoll ( )
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

        _playerStats.CanRegainStamina = false;
    }

    public void DisableStaminaGainOnHeavyAttack ( )
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

    public void ShowMedalion ( )
    {
        LevelUpMedalion.SetActive (true);
        Instantiate (HealParticles, transform.position, Quaternion.identity);
    }

    public void HideMedalion ( )
    {
        LevelUpMedalion.SetActive (false);
        _playerAC.SetBool ("Healing", false);
    }

    public void Heal ( )
    {
        _playerStats.Heal (_playerStats.HealthRecovered);
        _playerStats.CurrentNumberOfRecoveries--;
        _playerAC.SetBool ("Healing", true);
    }

    public void StopHealing ( )
    {
        _playerAC.SetBool ("Healing", false);
    }

    public void PlayFootstepSound ( )
    {
        switch (_gameManager.TypeOfSurface)
        {
            case SurfaceType.grass:
                _soundManager.GrassFootstep.PlayOneShot (_soundManager.GrasFootsteps [_stepCycle]);
                break;

            case SurfaceType.Stone:
                _soundManager.StoneFootstep.PlayOneShot (_soundManager.StoneFootsteps [_stepCycle]);
                break;
        }

        if (_playerAC.GetBool ("IsRunning"))
        {
            if (_stepCycle < 3)
            {
                _stepCycle++;
            }
            else
            {
                _stepCycle = 0;
            }
        }
        else
        {
            if (_stepCycle < 1)
            {
                _stepCycle++;
            }
            else
            {
                _stepCycle = 0;
            }
        }
    }

    public void PlaySwingSound ( )
    {
        _soundManager.SwordSwing.Play ();
    }
}
