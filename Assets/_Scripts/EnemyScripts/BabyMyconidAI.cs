using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BabyMyconidAI : MonoBehaviour
{
    public Transform Player;

    public float RangedChaseRange = 40.0f;
    public float RangedAttackRange = 25.0f;
    public float FleeRange = 15.0f;
    public float MeleeChaseRange = 10.0f;
    public float MeleeAttackRange = 1.0f;   

    [Space]
    public float FleeRangeModifier = 10.0f;

    public bool IsAttacking = false;   

    private float _distanceToPlayer;
    private float _distanceToLastDeactivator;

    private bool _playerDetected = false;
    private bool _watchOutForPlayer = false;
    private bool _checkDistanceToDeactivator = false;    

    private Vector3 _directionToPlayer;
    private Vector3 _startingPosition;

    private Collider _lastDeactivator;

    private NavMeshAgent _agent;
    private Animator _myconidAC;

    private void Start ( )
    {
        Player = GameObject.Find ("Player").transform;
        _agent = GetComponent<NavMeshAgent> ();
        _myconidAC = GetComponent<Animator> ();
        _startingPosition = transform.position;        
    }

    private void Update ( )
    {        
        _distanceToPlayer = Vector3.Distance (transform.position, Player.position);
        _directionToPlayer = (Player.transform.position - (transform.position + transform.up));
<<<<<<< HEAD
                
=======

        if(_distanceToPlayer <= MeleeAttackRange && !IsAttacking)
        {            
            _myconidAC.SetTrigger ("Attack");
            IsAttacking = true;
        }

        if (_distanceToPlayer <= RangedChaseRange)
        {
            if (_distanceToPlayer <= RangedAttackRange)
            {
                if(_distanceToPlayer <= FleeRange)
                {
                    if(_distanceToPlayer <= MeleeChaseRange)
                    {
                        if (_playerDetected)
                        {
                            _agent.isStopped = false;
                            _agent.SetDestination (Player.position);
                        }
                    }
                    else
                    {
                        Vector3 fleePosition = transform.position + Player.forward * FleeRangeModifier;

                        NavMeshHit hit;

                        NavMesh.SamplePosition (fleePosition, out hit, FleeRangeModifier, 1 << NavMesh.GetAreaFromName ("Walkable"));

                        _agent.isStopped = false;
                        _agent.SetDestination (hit.position);
                    }                   
                }
                else
                {
                    if (!_playerDetected)
                    {
                        _playerDetected = true;
                    }
                    else
                    {
                        if (!IsAttacking)
                        {
                            _agent.isStopped = true;
                            _myconidAC.SetTrigger ("RangedAttack");
                            IsAttacking = true;
                        }

                        transform.rotation = Quaternion.LookRotation (_directionToPlayer);
                    }
                }              
            }
            else
            {
                if(_playerDetected)
                {
                    _agent.isStopped = false;
                    _agent.SetDestination (Player.position);
                }
            }
        }
        else
        {
            _playerDetected = false;
        }        

>>>>>>> parent of 405ee07... Plugins added
        if(_agent.velocity.magnitude > 0.0f)
        {
            _myconidAC.SetBool ("IsMoving", true);
        }
        else
        {
            _myconidAC.SetBool ("IsMoving", false);
        }

        if(_checkDistanceToDeactivator)
        {
            _distanceToLastDeactivator = Vector3.Distance (transform.position, _lastDeactivator.ClosestPoint(transform.position));
        }        

        if(_lastDeactivator != null)
        {
            if(_distanceToPlayer <= _distanceToLastDeactivator)
            {
                RaycastHit hit;

                if (Physics.Raycast (transform.position + transform.up, _directionToPlayer + Player.transform.up, out hit, RangedChaseRange))
                {
                    Debug.DrawRay (transform.position + transform.up, _directionToPlayer + Player.transform.up);

                    if (hit.collider.transform.root.gameObject.CompareTag ("Player"))
                    {
                        _watchOutForPlayer = false;
                        _playerDetected = true;
                    }
                }                 
            }
            else if(_distanceToLastDeactivator > RangedChaseRange && _distanceToLastDeactivator < _distanceToPlayer)
            {
                _playerDetected = false;
                _watchOutForPlayer = false;
                _checkDistanceToDeactivator = false;
                _lastDeactivator = null;
            }
        }
    }

    private void OnDrawGizmosSelected ( )
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere (transform.position, MeleeAttackRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere (transform.position, MeleeChaseRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere (transform.position, FleeRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere (transform.position, RangedAttackRange);

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere (transform.position, RangedChaseRange);
    }

    private void OnTriggerEnter ( Collider other )
    {
        if(other.gameObject.CompareTag("EnemyDeactivator"))
        {
            _watchOutForPlayer = true;
            _lastDeactivator = other;
            _checkDistanceToDeactivator = true;
        }
    }
}
