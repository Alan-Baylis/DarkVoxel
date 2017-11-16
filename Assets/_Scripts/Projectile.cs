using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Transform EnemyTransform;

    public bool IsFlying = false;

    public float Speed = 10.0f;

    public int Damage = 5;

    private Transform _playerTransform;   

    private PlayerStats _playerStats;
    private Animator _playerAC;
    private PlayerAnimationEvents _playerAnimationEvents;

    private bool _isPositioned = false;
    private bool _damagedPlayer = false;

    private void Start ( )
    {
        _playerTransform = GameObject.Find("Player").transform;
        _playerStats = GameObject.Find ("Player").GetComponent<PlayerStats> ();
        _playerAC = _playerStats.gameObject.GetComponent<Animator> ();
        _playerAnimationEvents = _playerStats.gameObject.GetComponent<PlayerAnimationEvents> ();
    }

    private void Update ( )
    {
        if(IsFlying)
        {
            if (!_isPositioned)
            {
                transform.LookAt (_playerTransform.position + _playerTransform.up);                
                _isPositioned = true;
            }
            
            transform.Translate (Vector3.forward * Speed * Time.deltaTime);
        }
    }

    private void Destroy ( )
    {
        Destroy (gameObject);
    }   

    private void OnTriggerEnter ( Collider other )
    {
        if (other.gameObject.CompareTag ("Player"))
        {
            if (_playerAnimationEvents.CanGetDamaged && !_damagedPlayer)
            {
                _damagedPlayer = true;
                _playerStats.TakeDamage (Damage);
            }
            Destroy (gameObject);            
        }
        else if (other.gameObject.CompareTag ("Shield"))
        {
            if (_playerAC.GetBool ("IsBlocking") && _playerAC.GetBool ("ShieldEquipped") && !_damagedPlayer)
            {
                _damagedPlayer = true;
                if (_playerStats.CurrentStamina - Damage >= 0)
                {
                    if (_playerAnimationEvents.CanGetDamaged)
                    {
                        _playerStats.TakeDamageWithStamina (Damage);
                    }
                    Destroy (gameObject);
                }
                else
                {
                    if (_playerAnimationEvents.CanGetDamaged && !_damagedPlayer)
                    {
                        _damagedPlayer = true;
                        float damage = Damage - _playerStats.CurrentStamina;
                        _playerStats.TakeDamageWithStamina (Damage);
                        _playerStats.TakeDamage ((int) damage);
                    }
                    Destroy (gameObject);
                }
            }
        }
        else if(other.transform.root.transform != EnemyTransform)
        {
            Destroy ();
        }
    }
}
