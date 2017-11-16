using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static bool LockedOn = false;

    private PlayerManager _playerManager;
    private GameManager _gameManager;

    private Animator _playerAC;
    private CharacterController _characterController;
    private LockOnCamera _lockOnCamera;

    private Vector3 _moveDirection;
    private Vector3 _targetDirection;

    [SerializeField] private float _turnSmoothing = 7.0f;

    private void Awake ( )
    {
        _playerAC = GetComponent<Animator> ();
        _characterController = GetComponent<CharacterController> ();
    }

    private void Start ( )
    {
        _playerManager = PlayerManager.instance;
        _gameManager = GameManager.instance;
        _lockOnCamera = LockOnCamera.instance;
    }

    private void Update ( )
    {
        ApplyGravity ();
        Move ();        
    }

    private void Move ( )
    {
        if (!_playerAC.GetBool("LockedOn"))
        {
            FreeMovement ();
            OrientatePlayerInFreeMovement ();
        }
        else
        {        
            LockedOnMovement ();            
        }
    }

    //Movement while not locked on
    private void FreeMovement( )
    {
        PlayerInputManager.MoveDirection = Camera.main.transform.TransformDirection (PlayerInputManager.MoveDirection);
        PlayerInputManager.MoveDirection.y = 0.0f;

        _playerAC.SetFloat ("Vertical", PlayerInputManager.MoveMagnitude);            
    }

    //Movement while locked on
    private void LockedOnMovement( )
    {
        if(_playerAC.GetBool("LockedOn") && !_playerAC.GetBool("IsRunning") && _playerAC.GetBool("RollFinished"))
        {
            _targetDirection = _lockOnCamera.LockOnTarget.position - transform.position;
            _targetDirection.y = 0.0f;
            transform.rotation = Quaternion.RotateTowards (transform.rotation, Quaternion.LookRotation (_targetDirection), _playerManager.SetupFields.RotationSpeed * Time.deltaTime);

            if (_playerAC.GetBool ("InputEnabled"))
            {
                _playerAC.SetFloat ("Horizontal", PlayerInputManager.Horizontal);
                _playerAC.SetFloat ("Vertical", PlayerInputManager.Vertical);
            }
        }
        else if(_playerAC.GetBool("LockedOn") && _playerAC.GetBool("IsRunning"))
        {            
            PlayerInputManager.MoveDirection.y = 0.0f;

            _playerAC.SetFloat ("Vertical", PlayerInputManager.MoveMagnitude);

            OrientatePlayerInLockedOnMovement ();
        }
    }

    private void OrientatePlayerInFreeMovement( )
    {
        Quaternion targetRotation = Quaternion.LookRotation (PlayerInputManager.MoveDirection, transform.up);
        Quaternion newRotation = Quaternion.Lerp (transform.rotation, targetRotation, _turnSmoothing * Time.deltaTime);

        if (PlayerInputManager.MoveDirection == Vector3.zero)
        {
            return;
        }
        else
        {
            transform.rotation = newRotation;
        }
    }

    private void OrientatePlayerInLockedOnMovement()
    {
        PlayerInputManager.MoveDirection = Camera.main.transform.TransformDirection (PlayerInputManager.MoveDirection);
        PlayerInputManager.MoveDirection.y = 0.0f;

        Quaternion targetRotation = Quaternion.LookRotation (PlayerInputManager.MoveDirection, transform.up);
        Quaternion newRotation = Quaternion.Lerp (transform.rotation, targetRotation, _turnSmoothing * Time.deltaTime);

        transform.rotation = newRotation;
    }

    private void ApplyGravity( )
    {
        if (!_characterController.isGrounded)
        {
            _characterController.Move (new Vector3 (0.0f, -_gameManager.Gravity * Time.deltaTime, 0.0f));
        }        
    }
}
