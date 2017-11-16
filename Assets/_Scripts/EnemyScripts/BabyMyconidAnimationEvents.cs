using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BabyMyconidAnimationEvents : MonoBehaviour
{
    public GameObject Rock;

    public Transform RockPosition;

    private BabyMyconidAI _babyMyconidAI;

    private Animator _enemyAC;
    private EnemyStats _enemyStats;
    private Projectile _projectile;

    private GameObject _spawnedRock;

    private void Start ( )
    {
        _babyMyconidAI = GetComponent<BabyMyconidAI> ();
        _enemyAC = GetComponent<Animator> ();
        _enemyStats = GetComponent<EnemyStats> ();        
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
       _spawnedRock = Instantiate (Rock, RockPosition);        
    }

    public void ThrowRock()
    {
        if (_projectile != null)
        {
            _projectile = _spawnedRock.GetComponent<Projectile> ();
            _projectile.EnemyTransform = transform;
            _spawnedRock.transform.parent = null;
            Rigidbody rb = _spawnedRock.AddComponent<Rigidbody> ();
            rb.useGravity = false;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            _projectile.enabled = true;
            _projectile.IsFlying = true;
        }
    }
}


