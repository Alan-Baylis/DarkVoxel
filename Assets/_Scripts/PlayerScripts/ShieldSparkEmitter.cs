using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldSparkEmitter : MonoBehaviour
{
    public GameObject ShieldSparks;

    public Transform SparkImpact;

    public void EmitShieldSparks()
    {
        GameObject sparks = Instantiate (ShieldSparks, SparkImpact);
        Destroy (sparks, 5.0f);
    }
}
