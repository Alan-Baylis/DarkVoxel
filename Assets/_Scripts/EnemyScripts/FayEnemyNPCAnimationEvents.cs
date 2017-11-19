using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FayEnemyNPCAnimationEvents : MonoBehaviour
{
    public bool DamageEnabled = false;

    public AudioSource SwordSwing;
    public AudioSource FootstepSound;

    public List<AudioClip> GrassSounds;
    public List<AudioClip> StoneSounds;

    private Animator _fayEnemyNPCAC;
    private EnemyStats _enemyStats;
    private EnemyController _enemyController;
    private ShieldSparkEmitter _playerShieldSparkEmitter;
    private NPCEnemyAI _npcEnemyAI;

    private int _stepCycle;

    private void Awake ( )
    {
        _fayEnemyNPCAC = GetComponent<Animator> ();
        _enemyStats = GetComponent<EnemyStats> ();
        _enemyController = GetComponent<EnemyController> ();
        _npcEnemyAI = GetComponent<NPCEnemyAI> ();
    }

    public void ResetAttack ( )
    {
        _fayEnemyNPCAC.SetBool ("IsAttacking", false);
        _enemyStats.DamagedPlayerThisAttack = false;
    }

    public void ResetHeavyAttack ( )
    {
        _fayEnemyNPCAC.SetBool ("IsHeavyAttacking", false);
        _enemyStats.DamagedPlayerThisAttack = false;
    }

    public void TwoAttackCombo ( )
    {
        if (_fayEnemyNPCAC.GetInteger ("AttackCombo") < 1)
        {
            _fayEnemyNPCAC.SetInteger ("AttackCombo", 1);
        }
        else
        {
            _fayEnemyNPCAC.SetInteger ("AttackCombo", 0);
            _npcEnemyAI.CanAttack = false;
        }
    }

    public void ResetAttackCombo ( )
    {
        _fayEnemyNPCAC.SetInteger ("AttackCombo", 0);
    }

    public void EnableDamage ( )
    {
        DamageEnabled = true;
        _enemyStats.DamageEnabled = true;
    }

    public void DisableDamage ( )
    {
        DamageEnabled = false;
        _enemyStats.DamageEnabled = false;
    }

    public void DisableStaminaGainOnAttack ( )
    {
        _fayEnemyNPCAC.SetBool ("IsAttacking", true);

        if (_enemyStats.CurrentStamina - _enemyStats.AttackStaminaUsage >= 0)
        {
            _enemyStats.CurrentStamina -= _enemyStats.AttackStaminaUsage;
        }
        else
        {
            _enemyStats.CurrentStamina = 0.0f;
        }

        _enemyStats.CanRegainStamina = false;
    }

    public void DisableStaminaGainOnHeavyAttack ( )
    {
        _fayEnemyNPCAC.SetBool ("IsHeavyAttacking", true);

        if (_enemyStats.CurrentStamina - _enemyStats.HeavyAttackStaminaUsage >= 0)
        {
            _enemyStats.CurrentStamina -= _enemyStats.HeavyAttackStaminaUsage;
        }
        else
        {
            _enemyStats.CurrentStamina = 0.0f;
        }
    }

    public void PlayFootstepSound ( )
    {
        switch (_enemyController.TypeOfSurface)
        {
            case EnemyController.SurfaceType.Grass:
                FootstepSound.PlayOneShot (GrassSounds [_stepCycle]);
                break;

            case EnemyController.SurfaceType.Stone:
                FootstepSound.PlayOneShot (StoneSounds [_stepCycle]);
                break;
        }

        if (_fayEnemyNPCAC.GetBool ("Running"))
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
        SwordSwing.Play ();
    }
}
