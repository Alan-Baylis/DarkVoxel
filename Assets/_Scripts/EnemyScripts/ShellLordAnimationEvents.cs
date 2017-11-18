using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellLordAnimationEvents : MonoBehaviour
{
    public GameObject FallParticles;

    public Transform FallParticlesSpawnPoint;

    public void EmitFallParticles()
    {
        Instantiate (FallParticles, FallParticlesSpawnPoint);
    }
}
