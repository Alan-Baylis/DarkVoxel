using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePlayer : MonoBehaviour
{
    public AudioSource ShieldHit;

    private EnemyStats _enemyStats;
    private PlayerStats _playerStats;
    private ShieldSparkEmitter _playerShieldSparkEmitter;
    private PlayerAnimationEvents _playerAnimationEvents;

    private Animator _playerAC;
    private Animator _fayEnemyAC;

    private void Start ( )
    {
        _enemyStats = GetComponentInParent<EnemyStats> ();
        _playerStats = PlayerStats.instance;
        _playerShieldSparkEmitter = _enemyStats.PlayerAC.gameObject.GetComponent<ShieldSparkEmitter> ();
        _playerAC = _enemyStats.PlayerAC;
        _fayEnemyAC = GetComponentInParent<Animator> ();
        _playerAnimationEvents = _playerStats.gameObject.GetComponent<PlayerAnimationEvents> ();
    }

    private void OnTriggerEnter ( Collider other )
    {
        if (other.transform.root.CompareTag ("Player") && other.gameObject.CompareTag ("Shield") && _enemyStats.DamageEnabled && !_enemyStats.DamagedPlayerThisAttack)
        {
            if (_playerAnimationEvents.CanGetDamaged)
            {
                if (!_fayEnemyAC.GetBool ("IsHeavyAttacking"))
                {
                    if (_enemyStats.PlayerAC.GetBool ("IsBlocking") && _enemyStats.PlayerAC.GetBool ("ShieldEquipped"))
                    {
                        if (_playerStats.CurrentStamina >= _enemyStats.AttackDamage)
                        {
                            _enemyStats.DamagedPlayerThisAttack = true;
                            _playerShieldSparkEmitter.EmitShieldSparks ();
                            _playerStats.TakeDamageWithStamina (_enemyStats.AttackDamage);
                            _fayEnemyAC.SetTrigger ("Stagger");
                            _fayEnemyAC.SetBool ("IsBlocking", true);
                        }
                        else
                        {
                            _playerShieldSparkEmitter.EmitShieldSparks ();

                            _enemyStats.DamagedPlayerThisAttack = true;
                            float damage = _enemyStats.AttackDamage - _playerStats.CurrentStamina;
                            _playerStats.TakeDamageWithStamina (_enemyStats.AttackDamage);
                            _playerStats.TakeDamage ((int) damage);
                            _fayEnemyAC.SetTrigger ("Stagger");
                            _fayEnemyAC.SetBool ("IsBlocking", true);
                        }
                    }
                    else
                    {
                        _enemyStats.DamagedPlayerThisAttack = true;
                        _playerStats.TakeDamage (_enemyStats.AttackDamage);
                    }
                }
                else
                {
                    _enemyStats.DamagedPlayerThisAttack = true;
                    _playerStats.TakeDamage (_enemyStats.HeavyAttackDamage);
                    _playerAC.SetTrigger ("Stagger");
                }

                _enemyStats.PlayerAC.SetTrigger ("Hit");
            }
        }
        else if (other.gameObject.CompareTag ("Player") && _enemyStats.DamageEnabled && !_enemyStats.DamagedPlayerThisAttack)
        {
            if (_playerAnimationEvents.CanGetDamaged)
            {
                _enemyStats.DamagedPlayerThisAttack = true;

                _playerStats.TakeDamage (_enemyStats.AttackDamage);

                _enemyStats.PlayerAC.SetTrigger ("Hit");
            }
        }
    }
}
