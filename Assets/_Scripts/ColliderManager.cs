using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderManager : MonoBehaviour
{
    private Collider [] _hitboxes;

    private void Start ( )
    {
        _hitboxes = GetComponentsInChildren<BoxCollider> ();       
    }

    public void ActivateColliders()
    {
        foreach (BoxCollider collider in _hitboxes)
        {
            if (collider.gameObject.CompareTag ("Player"))
            {
                collider.enabled = true;
            }
        }
    }

    public void DisableColliders()
    {
        foreach (BoxCollider collider in _hitboxes)
        {
            if (collider.gameObject.CompareTag ("Player"))
            {
                collider.enabled = false;
            }
        }
    }
}
