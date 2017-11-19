using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedObjectDestroyer : MonoBehaviour
{
    public float DestructionTimer = 5.0f;

    private void Start ( )
    {
        Destroy (gameObject, DestructionTimer);
    }
}
