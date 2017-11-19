using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public enum SurfaceType { Grass, Stone }
    public SurfaceType TypeOfSurface;

    public bool LockedOn = false;
    public bool OnList = false;

    private LockOnCamera _lockOnCamera;

    private void Start ( )
    {
        _lockOnCamera = LockOnCamera.instance;

        InvokeRepeating ("CheckIfLockedOn", 0.0f, 0.5f);
    }

    private void CheckIfLockedOn ( )
    {
        if (_lockOnCamera.LockOnTarget != transform)
        {
            LockedOn = false;
        }
    }
}
