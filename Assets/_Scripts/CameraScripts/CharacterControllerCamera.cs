using System;
using UnityEngine;


public class CharacterControllerCamera : PivotBasedCamera
{
    public static CharacterControllerCamera instance;
    // This script is designed to be placed on the root object of a camera rig,
    // comprising 3 gameobjects, each parented to the next:
    // 	Camera Rig
    // 		Pivot
    // 			Camera      
    public bool UseJoystick = false;

    public Transform Pivot;

    public Animator PlayerAC;

    private float camX;
    private float camY;

    [SerializeField] private float _cameraMoveSpeed = 1f;                       // How fast the rig will move to keep up with the target's position.
    [SerializeField] private float _cameraTurnSmoothing = 0.0f;                 // How much smoothing to apply to the turn input, to reduce mouse-turn jerkiness
    [SerializeField] private float _cameraTiltMax = 75f;                        // The maximum value of the x axis rotation of the pivot.
    [SerializeField] private float _cameraTiltMin = 45f;                        // The minimum value of the x axis rotation of the pivot.

    [SerializeField] private bool _lockCursor = false;                          // Whether the cursor should be hidden and locked.
    [SerializeField] private bool _verticalAutoReturn = false;                  // set wether or not the vertical axis should auto return

    [Range (0f, 10f)] [SerializeField] private float _cameraTurnSpeed = 1.5f;   // How fast the rig will rotate from user input.

    public float _cameraLookAngle;                                             // The rig's y axis rotation.
    private float _cameraTiltAngle;                                             // The pivot's x axis rotation.

    private const float _lookDistance = 100f;                                  // How far in front of the pivot the character's look target is.

    private Vector3 _pivotEulers;

    private Quaternion _pivotTargetRotation;
    public Quaternion TransformTargetRotation;

    public bool Enabled = true;

    protected override void Awake()
    {
        base.Awake ();

        if(instance == null)
        {
            instance = this;
        }

        // Lock or unlock the cursor
        Cursor.lockState = _lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !_lockCursor;
        _pivotEulers = _pivotTransform.transform.rotation.eulerAngles;

        _pivotTargetRotation = _pivotTransform.transform.localRotation;
        TransformTargetRotation = transform.localRotation;
    }

    protected void Update ( )
    {
        HandleRotationMovement ();
        if(_lockCursor && Input.GetMouseButtonUp(0))
        {
            Cursor.lockState = _lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !_lockCursor;
        }
    }   

    private void OnDisable ( )
    {       
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    protected override void FollowTarget ( float deltaTime )
    {
        if(_target == null)
        {
            return;
        }
        //Move the rig towards the target position
        transform.position = Vector3.Lerp (transform.position, _target.position, deltaTime * _cameraMoveSpeed);
    }

    private void HandleRotationMovement()
    {
        if(Time.timeScale < float.Epsilon)
        {
            return;
        }

        // Read the user input
        var moveX = Input.GetAxis ("Horizontal");

        if(UseJoystick)
        {
            camX = Input.GetAxis ("HorizontalCameraMove");
            camY = Input.GetAxis ("VerticalCameraMove");
        }
        else
        {
            camX = Input.GetAxis ("Mouse X");
            camY = Input.GetAxis ("Mouse Y");
        }


        // Adjust the look angle by an amount proportional to the turn speed and horizontal input
        if (camX == 0)
        {
            if (PlayerAC.GetBool ("IsRunning"))
            {
                _cameraLookAngle += moveX * (_cameraTurnSpeed / 1.5f);
            }
            else
            {
                _cameraLookAngle += moveX * (_cameraTurnSpeed / 4);
            }
        }
        else 
        {
            _cameraLookAngle += camX * _cameraTurnSpeed;
        }
       


        // Rotate the rig around Y axis only
        TransformTargetRotation = Quaternion.Euler (0.0f, _cameraLookAngle, 0.0f);

        if(_verticalAutoReturn)
        {
            _cameraTiltAngle = camY > 0 ? Mathf.Lerp (0, -_cameraTiltMin, camY) : Mathf.Lerp (0, _cameraTiltMax, -camY);
        }
        else
        {
            _cameraTiltAngle -= camY * _cameraTurnSpeed;
            _cameraTiltAngle = Mathf.Clamp (_cameraTiltAngle, -_cameraTiltMin, _cameraTiltMax);
        }

        _pivotTargetRotation = Quaternion.Euler (_cameraTiltAngle, _pivotEulers.y, _pivotEulers.z);

        if(_cameraTurnSmoothing > 0)
        {
            _pivotTransform.localRotation = Quaternion.Slerp (_pivotTransform.localRotation, _pivotTargetRotation, _cameraTurnSmoothing * Time.deltaTime);
            transform.localRotation = Quaternion.Slerp (transform.localRotation, TransformTargetRotation, _cameraTurnSmoothing * Time.deltaTime);
        }
        else
        {
            _pivotTransform.localRotation = _pivotTargetRotation;
            transform.localRotation = TransformTargetRotation;
        }
    }
}

