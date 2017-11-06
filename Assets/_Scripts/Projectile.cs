using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public bool IsFlying = false;

    public float Speed = 10.0f;    

    private Transform _playerTransform;

    private bool _isPositioned = false;

    private void Start ( )
    {
        _playerTransform = GameObject.Find("Player").transform;
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
}
