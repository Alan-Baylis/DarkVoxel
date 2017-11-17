using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStats : CharacterStats
{
    public int AttackDamage;

    public int AttackStaminaUsage;
    public int HeavyAttackStaminaUsage;

    public bool HitThisAttack = false;
    public bool DamageEnabled = false;
    public bool DamagedPlayerThisAttack = false;

    public Animator PlayerAC;

    private PlayerAnimationEvents _playerAnimationEvents;
    private PlayerStats _playerStats;

    private void Start ( )
    {
        _playerAnimationEvents = GameObject.Find ("Player").GetComponent<PlayerAnimationEvents> ();
        _playerStats = GameObject.Find ("Player").GetComponent<PlayerStats> ();
        CharacterAC = GetComponent<Animator> ();      
    }

    private void OnTriggerEnter ( Collider other )    
    {
        if (other.gameObject.CompareTag("PlayerWeapon") && !HitThisAttack && _playerAnimationEvents.DamageEnabled)
        {
            HitThisAttack = true;

            _playerAnimationEvents.EnemiesHitThisAttack.Add (this);

            TakeDamage (_playerStats.Damage.GetValue());                       
        }

        if(other.gameObject.CompareTag("Shield") && DamageEnabled && !DamagedPlayerThisAttack)
        {
            if (PlayerAC.GetBool ("IsBlocking") && PlayerAC.GetBool("ShieldEquipped"))
            {
                if (_playerStats.CurrentStamina >= AttackDamage)
                {
                    DamagedPlayerThisAttack = true;

                    _playerStats.TakeDamageWithStamina (AttackDamage);
                }
                else
                {
                    DamagedPlayerThisAttack = true;

                    float damage = AttackDamage - _playerStats.CurrentStamina;
                    _playerStats.TakeDamageWithStamina (AttackDamage);                    
                    _playerStats.TakeDamage ((int)damage);
                }
            }
            else
            {
                DamagedPlayerThisAttack = true;

                _playerStats.TakeDamage (AttackDamage);
            }
        }
        else if (other.gameObject.CompareTag ("Player") && DamageEnabled && !DamagedPlayerThisAttack)
        {           
            DamagedPlayerThisAttack = true;

            _playerStats.TakeDamage (AttackDamage);            
        }
    }   
}
