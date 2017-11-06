using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationEvents : MonoBehaviour
{
    public GameObject Rock;

    public Transform RockPosition;

    private BabyMyconidAI _babyMyconidAI;

    private Animator _enemyAC;
    private EnemyStats _enemyStats;
    private Projectile _projectile;

    private void Start ( )
    {
        _babyMyconidAI = GetComponent<BabyMyconidAI> ();
        _enemyAC = GetComponent<Animator> ();
        _enemyStats = GetComponent<EnemyStats> ();
        _projectile = Rock.GetComponent<Projectile> ();
    }

    public void ResetAttack()
    {
        _babyMyconidAI.IsAttacking = false;
        _enemyStats.DamagedPlayerThisAttack = false;
    }

    public void EnableDamage()
    {
        _enemyStats.DamageEnabled = true;
    }

    public void DisableDamage()
    {
        _enemyStats.DamageEnabled = false;
    }

    public void InstantiateRock()
    {
        Instantiate (Rock, RockPosition);
    }

    public void ThrowRock()
    {
        Rock.transform.SetParent (null);
        _projectile.enabled = true;
        _projectile.IsFlying = true;
    }
}


