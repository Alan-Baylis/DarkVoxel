using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCEnemyAI : MonoBehaviour
{
    public Transform Player;

    public float AttackRange;
    public float MoveInToMeleeRange;
    public float ChaseRange;
    public float BackAwayAndBlockRange;
    public float GoToStartRange;

    public bool CanAttack = true;

    public float TimeBetweenCombos = 1.5f;

    public int HitCounter = 0;

    private Animator _enemyNPCAC;
    private CharacterController _characterController;

    private Animator _playerAC;

    private GameManager _gameManager;

    private Vector3 _startingPosition;
    private Vector3 _lastPlayerPosition;

    private float _distanceToPlayer;
    private float _currentTime;

    private float _playerMoveTimer = 1.0f;
    private float _hitTimer = 2.0f;
    private float _playerShortBlockTimer = 3.0f;
    private float _playerLongBlockTimer = 7.0f;

    private float _currentPlayerMoveLeftTime;
    private float _currentPlayerMoveRightTime;
    private float _currentHitTime;
    private float _currentPlayerShortBlockTime;
    private float _currentPlayerLongBlockTime;    

    private bool _playerPositionAquired = false;

    private void Start ( )
    {
        _enemyNPCAC = GetComponent<Animator> ();
        _characterController = GetComponent<CharacterController> ();
        _gameManager = GameManager.instance;
        _playerAC = Player.GetComponent<Animator> ();

        _startingPosition = transform.position;
        _currentTime = TimeBetweenCombos;

        _currentPlayerMoveLeftTime = _playerMoveTimer;
        _currentPlayerMoveRightTime = _playerMoveTimer;
        _currentHitTime = _hitTimer;
        _currentPlayerShortBlockTime = _playerShortBlockTimer;
        _currentPlayerLongBlockTime = _playerLongBlockTimer;
    }

    private void Update ( )
    {
        if (!CanAttack)
        {
            if (_currentTime > 0.0f)
            {
                _currentTime -= Time.deltaTime;
            }
            else
            {
                CanAttack = true;
                _currentTime = TimeBetweenCombos;
            }
        }

        if (!_characterController.isGrounded)
        {
            _characterController.Move (new Vector3 (0.0f, -_gameManager.Gravity * Time.deltaTime, 0.0f));
        }

        _distanceToPlayer = Vector3.Distance (transform.position, Player.position);

        if (_distanceToPlayer <= AttackRange)
        {
            transform.LookAt (Player);

            if (!_enemyNPCAC.GetBool ("IsHeavyAttacking"))
            {
                if (_playerAC.GetBool ("IsBlocking"))
                {                    
                    if (_currentPlayerLongBlockTime > 0.0f)
                    {
                        _currentPlayerLongBlockTime -= Time.deltaTime;
                    }
                    else
                    {
                        if (CanAttack)
                        {
                            _enemyNPCAC.SetTrigger ("HeavyAttack");
                            _enemyNPCAC.SetBool ("IsBlocking", false);
                            _currentPlayerLongBlockTime = _playerLongBlockTimer;
                            _enemyNPCAC.SetFloat ("Vertical", 0.0f);
                        }
                    }

                    if (_currentPlayerShortBlockTime > 0.0f)
                    {
                        if (!_enemyNPCAC.GetBool ("IsBlocking"))
                        {
                            _enemyNPCAC.SetFloat ("Horizontal", 0.0f);
                            _enemyNPCAC.SetFloat ("Vertical", 0.0f);
                            _currentPlayerShortBlockTime -= Time.deltaTime;
                        }
                    }
                    else
                    {
                        _enemyNPCAC.SetBool ("IsBlocking", true);
                        _currentPlayerShortBlockTime = _playerShortBlockTimer;
                        _enemyNPCAC.SetFloat ("Horizontal", 0.0f);
                        _enemyNPCAC.SetFloat ("Vertical", 0.0f);
                    }                    
                }
                else
                {
                    if (CanAttack && !_enemyNPCAC.GetBool("IsAttacking"))
                    {
                        _enemyNPCAC.applyRootMotion = false;
                        _enemyNPCAC.SetTrigger ("Attack");
                        _enemyNPCAC.SetBool ("IsBlocking", false);
                        _enemyNPCAC.SetFloat ("Vertical", 0.0f);
                        _currentPlayerLongBlockTime = _playerLongBlockTimer;
                        _currentPlayerShortBlockTime = _playerShortBlockTimer;
                    }
                }

                if (_playerAC.GetFloat ("Horizontal") > 0.0f)
                {
                    _currentPlayerMoveLeftTime = _playerMoveTimer;

                    if (_currentPlayerMoveRightTime > 0.0f)
                    {
                        _currentPlayerMoveRightTime -= Time.deltaTime;
                    }
                    else
                    {
                        _enemyNPCAC.applyRootMotion = true;
                        _enemyNPCAC.SetFloat ("Horizontal", 1.0f);
                        _enemyNPCAC.SetFloat ("Vertical", 0.0f);
                    }
                }
                else if (_playerAC.GetFloat ("Horizontal") < 0.0f)
                {
                    if (_currentPlayerMoveLeftTime > 0.0f)
                    {
                        _currentPlayerMoveLeftTime -= Time.deltaTime;
                    }
                    else
                    {
                        _enemyNPCAC.applyRootMotion = true;
                        _enemyNPCAC.SetFloat ("Horizontal", -1.0f);
                        _enemyNPCAC.SetFloat ("Vertical", 0.0f);
                    }
                }
            }           
        }

        if (_distanceToPlayer > AttackRange && _distanceToPlayer <= MoveInToMeleeRange && !_enemyNPCAC.GetBool ("IsAttacking") && !_enemyNPCAC.GetBool ("IsHeavyAttacking"))
        {
            transform.LookAt (Player);
            _enemyNPCAC.applyRootMotion = true;
            _enemyNPCAC.SetBool ("Running", false);
            _enemyNPCAC.SetBool ("LockedOn", true);
            _enemyNPCAC.SetFloat ("Vertical", 1.0f);
            _enemyNPCAC.SetFloat ("Horizontal", 0.0f);
        }

        if (_distanceToPlayer > MoveInToMeleeRange && _distanceToPlayer <= ChaseRange)
        {
            _playerPositionAquired = false;

            transform.LookAt (Player);

            _enemyNPCAC.SetFloat ("Vertical", 0.0f);
            _enemyNPCAC.SetFloat ("Horizontal", 0.0f);
            _enemyNPCAC.SetBool ("LockedOn", false);
            _enemyNPCAC.SetBool ("IsBlocking", false);
            _enemyNPCAC.SetBool ("Running", true);
        }

        if (_distanceToPlayer > ChaseRange && _distanceToPlayer <= BackAwayAndBlockRange)
        {
            if (!_playerPositionAquired)
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
        if (_distanceToPlayer > BackAwayAndBlockRange && _distanceToPlayer <= GoToStartRange)
        {
            _playerPositionAquired = false;

            if (transform.position != _startingPosition)
            {
                transform.LookAt (_startingPosition);
                _enemyNPCAC.SetFloat ("Vertical", 1.0f);
                _enemyNPCAC.SetFloat ("Horizontal", 0.0f);
                _enemyNPCAC.SetBool ("IsBlocking", false);
            }
        }

        if (transform.position.y <= _startingPosition.y + 0.5f && transform.position.y >= _startingPosition.y - 0.5f && transform.position.x <= _startingPosition.x + 0.5f && transform.position.x >= _startingPosition.x - 0.5f)
        {
            _enemyNPCAC.SetFloat ("Vertical", 0.0f);
        }

        if (_currentHitTime > 0.0f)
        {
            if (HitCounter == 1)
            {
                _currentHitTime -= Time.deltaTime;
            }
            else if (HitCounter == 2)
            {
                _enemyNPCAC.SetBool ("IsBlocking", true);
                _currentHitTime = _hitTimer;
            }
        }
        else
        {
            _currentHitTime = _hitTimer;
            _enemyNPCAC.SetBool ("IsBlocking", false);
            HitCounter = 0;
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
