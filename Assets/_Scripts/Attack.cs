using UnityEngine;

public class Attack : MonoBehaviour
{
    private Animator _animator;

    private void Start ( )
    {
        _animator = GetComponent<Animator> ();
    }

    public void TwoAttackCombo ( )
    {
        int attackCombo= _animator.GetInteger ("AttackCombo");

        if (attackCombo < 1)
        {
            attackCombo++;
            _animator.SetInteger ("AttackCombo", attackCombo);
        }
        else
        {
            attackCombo--;
            _animator.SetInteger ("AttackCombo", attackCombo);
        }
    }

    public void ThreeAttackCombo ( )
    {
        int attackCombo = _animator.GetInteger ("AttackCombo");

        if (attackCombo < 2)
        {
            attackCombo++;
            _animator.SetInteger ("AttackCombo", attackCombo);
        }
        else
        {
            attackCombo--;
            _animator.SetInteger ("AttackCombo", attackCombo);
        }
    }

    public void FourAttackCombo ( )
    {
        int attackCombo = _animator.GetInteger ("AttackCombo");

        if (attackCombo < 3)
        {
            attackCombo++;
            _animator.SetInteger ("AttackCombo", attackCombo);
        }
        else
        {
            attackCombo = 1;
            _animator.SetInteger ("AttackCombo", attackCombo);
        }
    }

    public void ResetAttackCombo ( )
    {
        int attackCombo = 0;
        _animator.SetInteger ("AttackCombo", attackCombo);        
    }
}
