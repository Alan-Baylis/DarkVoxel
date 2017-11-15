using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCEnemyAI : MonoBehaviour
{
    public Transform Player;

    public float AttackRange;
    public float MoveInToMeleeRange;
    public float ChaseRange;
    public float BackAwayAndBlockRange;
    public float GoToStartRange;

    private Animator _enemyNPCAC;

    private Vector3 _startingPosition;

    private float _distanceToPlayer;

    private void Start ( )
    {
        _enemyNPCAC = GetComponent<Animator> ();

        _startingPosition = transform.position;
    }

    private void Update ( )
    {
        _distanceToPlayer = Vector3.Distance (transform.position, Player.position);

        if(_distanceToPlayer <= AttackRange)
        {
            _enemyNPCAC.SetTrigger ("Attack");
        }
    }

    private void OnDrawGizmosSelected ( )
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere (transform.position, AttackRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere (transform.position, MoveInToMeleeRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere (transform.position, ChaseRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere (transform.position, BackAwayAndBlockRange);

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere (transform.position, GoToStartRange);
    }
}
