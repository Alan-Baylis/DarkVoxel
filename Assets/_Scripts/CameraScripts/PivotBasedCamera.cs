using System;
using UnityEngine;


public abstract class PivotBasedCamera : CameraTargetFollower
{
    // This script is designed to be placed on the root object of a camera rig,
    // comprising 3 gameobjects, each parented to the next:
    // 	Camera Rig
    // 		Pivot
    // 			Camera

    protected Transform _cameraTransform;
    protected Transform _pivotTransform;

    protected Vector3 _lastTargetPosition;

    protected virtual void Awake()
    {
        // find the camera in the object hierarchy
        _cameraTransform = GetComponentInChildren<Camera> ().transform;
        _pivotTransform = _cameraTransform.parent;
    }
}

