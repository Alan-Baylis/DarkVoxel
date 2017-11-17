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
    private CharacterController _characterController;

    private GameManager _gameManager;

    private Vector3 _startingPosition;
    private Vector3 _lastPlayerPosition;

    private float _distanceToPlayer;

    private bool _playerPositionAquired = false;

    private void Start ( )
    {
        _enemyNPCAC = GetComponent<Animator> ();
        _characterController = GetComponent<CharacterController> ();
        _gameManager = GameManager.instance;

        _startingPosition = transform.position;
    }

    private void Update ( )
    {
        if (!_characterController.isGrounded)
        {
            _characterController.Move (new Vector3 (0.0f, -_gameManager.Gravity * Time.deltaTime, 0.0f));
        }

        _distanceToPlayer = Vector3.Distance (transform.position, Player.position);

        if(_distanceToPlayer <= AttackRange)
        {
            if (!_enemyNPCAC.GetBool ("IsAttacking") && !_enemyNPCAC.GetBool ("IsHeavyAttacking"))
            {
                transform.LookAt (Player);                
                _enemyNPCAC.SetTrigger ("Attack");
            }
        }       

        if(_distanceToPlayer > AttackRange && _distanceToPlayer <= MoveInToMeleeRange)
        {
            transform.LookAt (Player);
            _enemyNPCAC.SetBool ("Running", false);
            _enemyNPCAC.SetBool ("LockedOn", true);
            _enemyNPCAC.SetFloat ("Vertical", 1.0f);
        }

        if(_distanceToPlayer > MoveInToMeleeRange && _distanceToPlayer <= ChaseRange)
        {
            _playerPositionAquired = false;

            transform.LookAt (Player);

            _enemyNPCAC.SetFloat ("Vertical", 0.0f);
            _enemyNPCAC.SetBool ("LockedOn", false);
            _enemyNPCAC.SetBool ("IsBlocking", false);
            _enemyNPCAC.SetBool ("Running", true);
        }

        if(_distanceToPlayer > ChaseRange && _distanceToPlayer <= BackAwayAndBlockRange)
        {
            if(!_playerPositionAquired)
            {
                _lastPlayerPosition = Player.position;
            }

            transform.LookAt (_lastPlayerPosition);

            if (transform.position != _startingPosition)
            {
                Vector3 heading = transform.position - _startingPosition;                
                _enemyNPCAC.SetBool ("Running", false);
                _enemyNPCAC.SetBool ("LockedOn", true);
                _enemyNPCAC.SetBool ("IsBlocking", true);
                _enemyNPCAC.SetFloat ("Vertical", -heading.x);
                _enemyNPCAC.SetFloat ("Horizontal", -heading.y);
            }
        }
        if(_distanceToPlayer > BackAwayAndBlockRange && _distanceToPlayer <= GoToStartRange)
        {
            _playerPositionAquired = false;

            if (transform.position != _startingPosition)
            {
                transform.LookAt (_startingPosition);
                _enemyNPCAC.SetFloat ("Vertical", 1.0f);
                _enemyNPCAC.SetBool ("IsBlocking", false);
            }
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
