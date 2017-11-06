using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CopyScript : MonoBehaviour
{
    public static float Horizontal;
    public static float Vertical;
    public static float MoveMagnitude;

    public static Vector3 MoveDirection;

    public GameObject Menu;

    private PlayerManager _playerManager;
    private LockOnCamera _lockOnCamera;
    private CharacterControllerCamera _characterControllerCamera;
    private Animator _playerAC;
    private GameManager _gameManager;
    private PlayerMovement _playerMovement;
    private PlayerStats _playerStats;

    private Collider [] _colliders;
    private List<GameObject> _visibleEnemies = new List<GameObject> ();

    [SerializeField] private float _runTimer = 0.75f;                       //time in sec that dictates how long the player needs to hold run button before running, or how fast must he release the button fo rolling
    private float _currentTimer;

    private GameObject _lockedOnEnemy;

    private bool _targetChanged = false;
    private bool _isResting = false;                                        //Disables running emidiately after stamina is regained if the run button is stil pressed down

    private int _numberOfPreviousColliders;

    private float distanceToEnemy;
    private float shortestDistance;

    private void Start ( )
    {
        _playerMovement = GetComponent<PlayerMovement> ();
        _playerManager = PlayerManager.instance;
        _lockOnCamera = LockOnCamera.instance;
        _characterControllerCamera = CharacterControllerCamera.instance;
        _playerAC = GetComponent<Animator> ();
        _gameManager = GameManager.instance;
        _playerStats = PlayerStats.instance;
        _currentTimer = _runTimer;
    }

    private void Update ( )
    {
        MoveInput ();
        LockOn ();
        Roll ();
        Run ();

        if (_gameManager.StateOfGame == GameManager.GameState.Playing)
        {
            Attack ();
            Block ();
        }

        AccesMenu ();
    }

    private void MoveInput ( )
    {
        if (_playerAC.GetBool ("InputEnabled"))
        {
            Horizontal = Input.GetAxis ("Horizontal");
            Vertical = Input.GetAxis ("Vertical");
        }

        MoveDirection = new Vector3 (Horizontal, 0.0f, Vertical);

        MoveMagnitude = new Vector2 (Horizontal, Vertical).magnitude;
        if (MoveMagnitude > 1.0f)
        {
            MoveMagnitude = 1.0f;
        }
    }

    //This code needs to be restructured for better performance (if possible). This is for testing purposes. Also, system needs to be enhanced (only locks on onto first enemy on list which is not necesarely the one
    //the player is looking at

    private void LockOn ( )
    {
        shortestDistance = -1;

        if (_colliders != null)
        {
            _numberOfPreviousColliders = _colliders.Length;
        }

        _colliders = Physics.OverlapSphere (transform.position, _playerManager.SetupFields.LockOnRadius);

        int numberOfCurrentColliders = _colliders.Length;

        if (numberOfCurrentColliders != _numberOfPreviousColliders)
        {
            foreach (GameObject enemy in _visibleEnemies)
            {
                enemy.GetComponent<EnemyController> ().OnList = false;
            }
            _visibleEnemies.Clear ();
        }

        for (int i = 0; i < _colliders.Length; i++)
        {
            if (_colliders [i].gameObject.CompareTag ("LockOnCollider"))
            {
                if (_colliders [i].GetComponentInParent<EnemyController> ().LockedOn)
                {
                    distanceToEnemy = Vector3.Distance (transform.position, _colliders [i].gameObject.transform.position);
                }

                Vector3 direction = (_colliders [i].transform.position - (transform.position + transform.up)).normalized;
                float angle = Vector3.Angle (direction, transform.forward);

                //check if enemy is in players FOV
                if (angle < _playerManager.SetupFields.FieldOfViewAngle * 0.5f)
                {
                    if (!_colliders [i].GetComponentInParent<EnemyController> ().OnList)
                    {
                        _visibleEnemies.Add (_colliders [i].transform.root.gameObject);
                        _colliders [i].GetComponentInParent<EnemyController> ().OnList = true;
                    }

                    for (int n = 0; n < _visibleEnemies.Count; n++)
                    {
                        if (Input.GetButtonDown ("LockOn") && !_visibleEnemies [n].GetComponent<EnemyController> ().LockedOn)
                        {
                            if ((shortestDistance == -1 || shortestDistance > distanceToEnemy))
                            {
                                shortestDistance = distanceToEnemy;

                                RaycastHit hit;

                                if (Physics.Raycast (transform.position + transform.up, direction, out hit, _playerManager.SetupFields.LockOnRadius))
                                {
                                    if (hit.collider.transform.root.gameObject == _visibleEnemies [n])
                                    {
                                        _lockOnCamera.LockOnTarget = _visibleEnemies [n].transform;
                                        _lockedOnEnemy = _visibleEnemies [n];

                                        EnemyController enemyController = _lockedOnEnemy.GetComponent<EnemyController> ();
                                        enemyController.LockedOn = true;

                                        //Add visual que that enemy is locked on 

                                        Debug.Log ("Hit " + hit.collider.gameObject.name);
                                    }
                                }
                            }
                        }
                        else if (Input.GetAxis ("Mouse X") >= 0.9f && _visibleEnemies [n].GetComponent<EnemyController> ().LockedOn && !_targetChanged)
                        {
                            if (n < _visibleEnemies.Count - 1)
                            {
                                n++;

                                RaycastHit hit;

                                if (Physics.Raycast (transform.position + transform.up, direction, out hit, _playerManager.SetupFields.LockOnRadius))
                                {
                                    if (hit.collider.transform.root.gameObject == _visibleEnemies [n])
                                    {
                                        _lockOnCamera.LockOnTarget = _visibleEnemies [n].transform;
                                        _lockedOnEnemy = _visibleEnemies [n];

                                        _lockedOnEnemy.GetComponent<EnemyController> ().LockedOn = true;
                                        _targetChanged = true;
                                    }
                                }
                            }
                            else
                            {
                                n = 0;

                                RaycastHit hit;

                                if (Physics.Raycast (transform.position + transform.up, direction, out hit, _playerManager.SetupFields.LockOnRadius))
                                {
                                    if (hit.collider.transform.root.gameObject == _visibleEnemies [n])
                                    {
                                        _lockOnCamera.LockOnTarget = _visibleEnemies [n].transform;
                                        _lockedOnEnemy = _visibleEnemies [n];

                                        _lockedOnEnemy.GetComponent<EnemyController> ().LockedOn = true;
                                        _targetChanged = true;
                                    }
                                }
                            }
                        }
                        else if (Input.GetAxis ("Mouse X") <= -0.9f && _visibleEnemies [n].GetComponent<EnemyController> ().LockedOn && !_targetChanged)
                        {
                            if (n > 0)
                            {
                                n--;

                                RaycastHit hit;

                                if (Physics.Raycast (transform.position + transform.up, direction, out hit, _playerManager.SetupFields.LockOnRadius))
                                {
                                    if (hit.collider.transform.root.gameObject == _visibleEnemies [n])
                                    {
                                        _lockOnCamera.LockOnTarget = _visibleEnemies [n].transform;
                                        _lockedOnEnemy = _visibleEnemies [n];

                                        _lockedOnEnemy.GetComponent<EnemyController> ().LockedOn = true;
                                        _targetChanged = true;
                                    }
                                }
                            }
                            else
                            {
                                n = _visibleEnemies.Count - 1;

                                RaycastHit hit;

                                if (Physics.Raycast (transform.position + transform.up, direction, out hit, _playerManager.SetupFields.LockOnRadius))
                                {
                                    if (hit.collider.transform.root.gameObject == _visibleEnemies [n])
                                    {
                                        _lockOnCamera.LockOnTarget = _visibleEnemies [n].transform;
                                        _lockedOnEnemy = _visibleEnemies [n];

                                        _lockedOnEnemy.GetComponent<EnemyController> ().LockedOn = true;
                                        _targetChanged = true;
                                    }
                                }
                            }
                        }
                        else if (Input.GetAxis ("Mouse X") < 0.9f && Input.GetAxis ("Mouse X") > -0.9f)
                        {
                            _targetChanged = false;
                        }
                    }
                }
            }
        }

        if (Input.GetButtonDown ("LockOn") && _lockOnCamera.LockOnTarget != null)
        {
            if (shortestDistance != -1f)
            {
                if (!PlayerMovement.LockedOn)
                {
                    _playerAC.SetBool ("LockedOn", true);
                    PlayerMovement.LockedOn = true;
                    _characterControllerCamera.Enabled = false;
                    _characterControllerCamera.enabled = false;
                    _lockOnCamera.enabled = true;
                }
                else
                {
                    PlayerMovement.LockedOn = false;
                    _playerAC.SetBool ("LockedOn", false);
                    shortestDistance = -1f;
                    _characterControllerCamera.enabled = true;
                    EnemyController enemyController = _lockedOnEnemy.GetComponent<EnemyController> ();
                    enemyController.LockedOn = false;

                    for (int i = 0; i < _visibleEnemies.Count; i++)
                    {
                        _visibleEnemies [i].GetComponent<EnemyController> ().OnList = false;
                    }
                    _visibleEnemies.Clear ();
                    _lockOnCamera.enabled = false;
                    _lockOnCamera.LockOnTarget = null;

                    if (_lockedOnEnemy != null)
                    {
                        enemyController = _lockedOnEnemy.GetComponent<EnemyController> ();
                        enemyController.LockedOn = false;

                        for (int i = 0; i < _visibleEnemies.Count; i++)
                        {
                            _visibleEnemies [i].GetComponent<EnemyController> ().OnList = false;
                        }
                        _visibleEnemies.Clear ();
                        _lockedOnEnemy = null;
                    }
                    //Disable lockon que
                }
            }
            else
            {
                PlayerMovement.LockedOn = false;
                _playerAC.SetBool ("LockedOn", false);
                shortestDistance = -1f;
                _characterControllerCamera.enabled = true;
                EnemyController enemyController = _lockedOnEnemy.GetComponent<EnemyController> ();
                enemyController.LockedOn = false;

                for (int i = 0; i < _visibleEnemies.Count; i++)
                {
                    _visibleEnemies [i].GetComponent<EnemyController> ().OnList = false;
                }
                _visibleEnemies.Clear ();
                _lockOnCamera.LockOnTarget = null;

                if (_lockedOnEnemy != null)
                {
                    enemyController = _lockedOnEnemy.GetComponent<EnemyController> ();
                    enemyController.LockedOn = false;

                    for (int i = 0; i < _visibleEnemies.Count; i++)
                    {
                        _visibleEnemies [i].GetComponent<EnemyController> ().OnList = false;
                    }
                    _visibleEnemies.Clear ();
                    _lockedOnEnemy = null;
                }
                //Disable lockon que
            }
        }

        if (distanceToEnemy > _playerManager.SetupFields.LockOnRadius)
        {
            PlayerMovement.LockedOn = false;
            _playerAC.SetBool ("LockedOn", false);
            shortestDistance = -1f;
            distanceToEnemy = 0;
            _lockOnCamera.enabled = false;
            _characterControllerCamera.enabled = true;
            _lockOnCamera.LockOnTarget = null;

            if (_lockedOnEnemy != null)
            {
                EnemyController enemyController = _lockedOnEnemy.GetComponent<EnemyController> ();
                enemyController.LockedOn = false;

                for (int i = 0; i < _visibleEnemies.Count; i++)
                {
                    _visibleEnemies [i].GetComponent<EnemyController> ().OnList = false;
                }
                _visibleEnemies.Clear ();
                _lockedOnEnemy = null;
            }
        }
    }

    private void Roll ( )
    {
        if (Input.GetButton ("Roll"))
        {
            StartCoroutine (Countdown ());
        }

        if (Input.GetButtonUp ("Roll"))
        {
            StopCoroutine (Countdown ());
            if (_currentTimer > 0.0f && _playerAC.GetBool ("RollEnabled") && _playerAC.GetBool ("IsMoving") && _playerStats.CurrentStamina > 0.0f)
            {
                _playerAC.SetTrigger ("Rolling");

                _playerStats.CanRegainStamina = false;

                _currentTimer = _runTimer;
            }
            else
            {
                _currentTimer = _runTimer;
            }
        }
    }

    private void Run ( )
    {
        if (Input.GetButton ("Run"))
        {
            StartCoroutine (Countdown ());
            if (_currentTimer <= 0.0f && _playerAC.GetBool ("IsMoving"))
            {
                if (_playerStats.CurrentStamina > 0.0f && !_isResting)
                {
                    _playerAC.SetBool ("IsRunning", true);
                }
                else
                {
                    _playerAC.SetBool ("IsRunning", false);
                    _isResting = true;
                }
            }
            else
            {
                _playerAC.SetBool ("IsRunning", false);
            }
        }

        if (Input.GetButtonUp ("Run"))
        {
            _isResting = false;
            StopCoroutine (Countdown ());
            _playerAC.SetBool ("IsRunning", false);
            _currentTimer = _runTimer;
        }

        if (_playerAC.GetBool ("IsRunning"))
        {
            //Use stamina
            _playerStats.UseStamina (_playerStats.RunStaminaUsage);
        }
        else
        {
            //Recover stamina
            _playerStats.RegainStamina (_playerStats.StaminaGain);
        }
    }

    //System needs to be redone for better performance!!
    //This is just for testing
    private void Attack ( )
    {
        if (Input.GetButtonDown ("Attack") && _gameManager.StateOfGame != GameManager.GameState.InMenu)
        {
            if (_playerStats.CurrentStamina > 0)
            {
                if (_playerAC.GetBool ("BowEquipped"))
                {
                    if (!_playerAC.GetBool ("RollFinished"))
                    {
                        _playerAC.SetBool ("RollAttack", true);
                        _playerAC.SetBool ("RollAttack", true);
                    }
                    else
                    {
                        _playerAC.SetTrigger ("Attack");
                        _playerAC.SetBool ("Attacking", true);
                    }
                }
                else
                {
                    if (!_playerAC.GetBool ("RollFinished"))
                    {
                        _playerAC.SetBool ("RollAttack", true);
                    }
                    else
                    {
                        _playerAC.SetTrigger ("Attack");
                    }
                }
            }
        }
    }

    private void Block ( )
    {
        if (Input.GetButton ("Block") && _playerAC.GetBool ("ShieldEquipped"))
        {
            _playerAC.SetBool ("IsBlocking", true);
        }
        else
        {
            _playerAC.SetBool ("IsBlocking", false);
        }
    }

    private void AccesMenu ( )
    {
        if (Input.GetButtonDown ("Menu"))
        {
            if (Menu.activeSelf)
            {
                Menu.SetActive (false);
                _gameManager.StateOfGame = GameManager.GameState.Playing;
            }
            else
            {
                Menu.SetActive (true);
                Menu.GetComponentInChildren<Button> ().Select ();
                _gameManager.StateOfGame = GameManager.GameState.InMenu;
            }
        }
    }

    IEnumerator Countdown ( )
    {
        if (_currentTimer > 0.0f)
        {
            _currentTimer -= Time.deltaTime;
        }

        yield return 0;
    }
}
