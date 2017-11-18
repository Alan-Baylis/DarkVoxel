using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ShellLordAI : MonoBehaviour
{
    private Animator _shellLordAC;
    private CharacterController _characterController;
    private NavMeshAgent _agent;
    private GameManager _gameManager;

    private void Start ( )
    {
        _shellLordAC = GetComponent<Animator> ();
        _agent = GetComponent<NavMeshAgent> ();
        _characterController = GetComponent<CharacterController> ();
        _gameManager = GameManager.instance;
    }

    private void Update ( )
    {
        if(!_characterController.isGrounded)
        {
            _characterController.Move (new Vector3 (0.0f, -_gameManager.Gravity * Time.deltaTime * 40, 0.0f));
        }

        if(_characterController.isGrounded && _shellLordAC.GetBool("IsFlying"))
        {
            _shellLordAC.SetTrigger ("Land");
            _shellLordAC.SetBool ("IsFlying", false);
        }
    }
}