using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public bool IsFlying = false;

    public float Speed = 10.0f;

    public int Damage = 5;

    private Transform _playerTransform;

    private PlayerStats _playerStats;
    private Animator _playerAC;   

    private bool _isPositioned = false;   

    private void Start ( )
    {
        _playerTransform = GameObject.Find("Player").transform;
        _playerStats = GameObject.Find ("Player").GetComponent<PlayerStats> ();
        _playerAC = _playerStats.gameObject.GetComponent<Animator> ();       
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

    private void OnCollisionEnter ( Collision collision )
    {
        if (collision.gameObject.CompareTag ("Player"))
        {                            
            _playerStats.TakeDamage (Damage);
            Destroy (gameObject);            
        }
        else if (collision.gameObject.CompareTag ("Shield"))
        {
            if (_playerAC.GetBool ("IsBlocking") && _playerAC.GetBool ("ShieldEquipped"))
            {
                if (_playerStats.CurrentStamina - Damage >= 0)
                {
                    _playerStats.TakeDamageWithStamina (Damage);
                    Destroy (gameObject);
                }
                else
                {
                    float damage = Damage - _playerStats.CurrentStamina;
                    _playerStats.TakeDamageWithStamina (Damage);
                    _playerStats.TakeDamage ((int)damage);
                    Destroy (gameObject);
                }
            }
        }        
    }
}
