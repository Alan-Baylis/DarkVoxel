using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public bool IsFlying = false;

    public float Speed = 10.0f;

    private void Update ( )
    {
        if(IsFlying)
        {
            transform.Translate (transform.forward * Speed * Time.deltaTime);
        }
    }

    private void Destroy ( )
    {
        Destroy (gameObject);
    }
}
