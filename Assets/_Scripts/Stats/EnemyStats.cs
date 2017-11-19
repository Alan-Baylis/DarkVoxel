using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStats : CharacterStats
{
    public int AttackDamage;
    public int HeavyAttackDamage;

    public int AttackStaminaUsage;
    public int HeavyAttackStaminaUsage;

    public bool HitThisAttack = false;
    public bool DamageEnabled = false;
    public bool DamagedPlayerThisAttack = false;

    public enum EnemyType { BabyMyconid, FayNPC }
    public EnemyType TypeOfEnemy;

    public Animator PlayerAC;

    private Animator _enemyAC;

    private PlayerAnimationEvents _playerAnimationEvents;
    private PlayerStats _playerStats;
    private NPCEnemyAI _npcEnemyAI;

    private void Start ( )
    {
        _playerAnimationEvents = GameObject.Find ("Player").GetComponent<PlayerAnimationEvents> ();
        _playerStats = GameObject.Find ("Player").GetComponent<PlayerStats> ();
        CharacterAC = GetComponent<Animator> ();
        _enemyAC = GetComponent<Animator> ();
        _npcEnemyAI = GetComponent<NPCEnemyAI> ();
    }

    private void OnTriggerEnter ( Collider other )
    {
        switch (TypeOfEnemy)
        {
            case EnemyType.BabyMyconid:

                if (other.gameObject.CompareTag ("PlayerWeapon") && !HitThisAttack && _playerAnimationEvents.DamageEnabled && !Dead)
                {
                    HitThisAttack = true;

                    _playerAnimationEvents.EnemiesHitThisAttack.Add (this);

                    TakeDamage (_playerStats.Damage.GetValue ());
                    HitSound.Play ();
                }

                if (other.gameObject.CompareTag ("Shield") && DamageEnabled && !DamagedPlayerThisAttack)
                {
                    if (PlayerAC.GetBool ("IsBlocking") && PlayerAC.GetBool ("ShieldEquipped"))
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
                            _playerStats.TakeDamage ((int) damage);
                        }
                    }
                    else
                    {
                        DamagedPlayerThisAttack = true;

                        _playerStats.TakeDamage (AttackDamage);
                    }
                    PlayerAC.SetTrigger ("Hit");
                }
                else if (other.gameObject.CompareTag ("Player") && DamageEnabled && !DamagedPlayerThisAttack)
                {
                    DamagedPlayerThisAttack = true;

                    _playerStats.TakeDamage (AttackDamage);

                    PlayerAC.SetTrigger ("Hit");
                }
                break;

            case EnemyType.FayNPC:

                if (other.gameObject.CompareTag ("PlayerWeapon") && !HitThisAttack && _playerAnimationEvents.DamageEnabled && !Dead)
                {
                    if (_enemyAC.GetBool ("IsBlocking"))
                    {
                        ShieldHitSound.Play ();
                        _enemyAC.SetTrigger ("Hit");                       
                        PlayerAC.SetTrigger ("Stagger");
                        _enemyAC.SetBool ("IsBlocking", false);
                    }
                    else
                    {
                        HitThisAttack = true;

                        _playerAnimationEvents.EnemiesHitThisAttack.Add (this);

                        TakeDamage (_playerStats.Damage.GetValue ());
                        _npcEnemyAI.HitCounter++;
                        //Turn healthbar on
                        HitSound.Play ();
                    }
                }
                break;
        }
    }
}
