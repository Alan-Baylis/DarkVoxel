using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInputManager : MonoBehaviour
{
	public static float Horizontal;
    public static float Vertical;
    public static float MoveMagnitude;    

    public static Vector3 MoveDirection;

    public GameObject Menu;
    public GameObject EquipmentHUD;
    public GameObject MainCameraRig;
    public GameObject MainCameraRigPivot;

    private PlayerManager _playerManager;
    private LockOnCamera _lockOnCamera;
    private CharacterControllerCamera _characterControllerCamera;
    private Animator _playerAC;
    private GameManager _gameManager;
    private PlayerMovement _playerMovement;
    private PlayerStats _playerStats;

    private Collider [] _colliders;
    private List<GameObject> _visibleEnemies = new List<GameObject>();

    [SerializeField] private float _runTimer = 0.75f;                       //time in sec that dictates how long the player needs to hold run button before running, or how fast must he release the button fo rolling
    private float _currentTimer;

    private GameObject _lockedOnEnemy;    

    private Transform _inventoryCameraPosition;

    private bool _targetChanged = false;
    private bool _isResting = false;                                        //Disables running emidiately after stamina is regained if the run button is stil pressed down                                                 
    
    private int _numberOfPreviousColliders;    

    private Vector3 _directionToEnemy;

    private float lowestAngle = -1;
    private float angle = 0;
    private float distanceToEnemy;

    private int _layerMask = 1 << 9;

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
        _inventoryCameraPosition = transform.Find ("InventoryCameraPosition");
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
            HeavyAttack ();
            Block ();
        }       

        AccesMenu ();
    }

    private void MoveInput( )
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

    private void LockOn ( )
    {
        lowestAngle = -1;        
        angle = 0;

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

        foreach (Collider collider in _colliders)
        {
            if (collider.gameObject.CompareTag ("LockOnCollider"))
            {
                _directionToEnemy = (collider.transform.position - (transform.position + transform.up));
                angle = Vector3.Angle (_directionToEnemy, Camera.main.transform.forward);

                EnemyController enemyController = collider.GetComponentInParent<EnemyController> ();
                //check if enemy is in players FOV
                if (angle < Camera.main.fieldOfView)
                {
                    if (!enemyController.OnList)
                    {
                        _visibleEnemies.Add (collider.transform.root.gameObject);
                        enemyController.OnList = true;
                    }
                }
                else
                {
                    if(enemyController.OnList)
                    {
                        _visibleEnemies.Remove (collider.transform.root.gameObject);
                        enemyController.OnList = false;
                    }
                }
            }
        }

        #region LockOn
        if (Input.GetButtonDown("LockOn") && _lockedOnEnemy == null)
        {
            if(_visibleEnemies.Count != 0)
            {
                foreach (GameObject enemy in _visibleEnemies)
                {
                    _directionToEnemy = (enemy.transform.position - transform.position);
                    angle = Vector3.Angle (_directionToEnemy, Camera.main.transform.forward);

                    if (lowestAngle == -1 || lowestAngle > angle)
                    {
                        RaycastHit hit;

                        Debug.DrawRay (transform.position + transform.up, _directionToEnemy);

                        if (Physics.Raycast (transform.position + transform.up, _directionToEnemy, out hit, _playerManager.SetupFields.LockOnRadius, _layerMask))
                        {
                            if (hit.collider.transform.root.gameObject == enemy)
                            {
                                _lockOnCamera.LockOnTarget = enemy.transform;
                                _lockedOnEnemy = enemy;

                                _lockedOnEnemy.GetComponent<EnemyController> ().LockedOn = true;

                                _playerAC.SetBool ("LockedOn", true);
                                PlayerMovement.LockedOn = true;
                                _characterControllerCamera.Enabled = false;
                                _characterControllerCamera.enabled = false;
                                _lockOnCamera.enabled = true;

                                lowestAngle = angle;
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region ChangeLockOnTarget
        else if(Input.GetAxis("Mouse X") >= 0.9f && !_targetChanged && _lockedOnEnemy != null)
        {
            float smallestDirectionAngle = -1;

            foreach (GameObject enemy in _visibleEnemies)
            {
                EnemyController enemyController = enemy.GetComponent<EnemyController> ();

                if(!enemyController.LockedOn)
                {
                    _directionToEnemy = (enemy.transform.position - (transform.position + transform.up));

                    Vector3 sideDirection = Vector3.Cross (transform.forward, _directionToEnemy);
                    float directionAngle = Vector3.Dot (sideDirection, transform.up);

                    if (directionAngle > 0.0f)
                    {
                        if (smallestDirectionAngle == -1 || smallestDirectionAngle > directionAngle)
                        {
                            RaycastHit hit;

                            if (Physics.Raycast (transform.position + transform.up, _directionToEnemy, out hit, _playerManager.SetupFields.LockOnRadius, _layerMask))
                            {
                                if (hit.collider.transform.root.gameObject == enemy)
                                {
                                    _lockOnCamera.LockOnTarget = enemy.transform;
                                    _lockedOnEnemy = enemy;

                                    _lockedOnEnemy.GetComponent<EnemyController> ().LockedOn = true;

                                    _playerAC.SetBool ("LockedOn", true);
                                    PlayerMovement.LockedOn = true;
                                    _characterControllerCamera.Enabled = false;
                                    _characterControllerCamera.enabled = false;
                                    _lockOnCamera.enabled = true;

                                    _targetChanged = true;

                                    lowestAngle = angle;
                                }
                            }
                        }
                    }
                }
            }
        }
        else if(Input.GetAxis("Mouse X") <= -0.9f && !_targetChanged && _lockedOnEnemy != null)
        {
            float smallestDirectionAngle = -1;

            foreach (GameObject enemy in _visibleEnemies)
            {
                EnemyController enemyController = enemy.GetComponent<EnemyController> ();

                if (!enemyController.LockedOn)
                {
                    _directionToEnemy = (enemy.transform.position - (transform.position + transform.up));

                    Vector3 sideDirection = Vector3.Cross (transform.forward, _directionToEnemy);
                    float directionAngle = Vector3.Dot (sideDirection, transform.up);

                    if (directionAngle < 0.0f)
                    {
                        if (smallestDirectionAngle == -1 || smallestDirectionAngle > directionAngle)
                        {
                            RaycastHit hit;

                            if (Physics.Raycast (transform.position + transform.up, _directionToEnemy, out hit, _playerManager.SetupFields.LockOnRadius, _layerMask))
                            {
                                if (hit.collider.transform.root.gameObject == enemy)
                                {
                                    _lockOnCamera.LockOnTarget = enemy.transform;
                                    _lockedOnEnemy = enemy;

                                    _lockedOnEnemy.GetComponent<EnemyController> ().LockedOn = true;

                                    _playerAC.SetBool ("LockedOn", true);
                                    PlayerMovement.LockedOn = true;
                                    _characterControllerCamera.Enabled = false;
                                    _characterControllerCamera.enabled = false;
                                    _lockOnCamera.enabled = true;

                                    _targetChanged = true;

                                    lowestAngle = angle;
                                }
                            }
                        }
                    }
                }
            }
        }
        else if(Input.GetAxis("Mouse X") > -0.9f && Input.GetAxis("Mouse X") < 0.9f)
        {
            _targetChanged = false;
        }
        #endregion

        #region CancleLockOn
        if(Input.GetButtonDown("LockOn") && _lockedOnEnemy != null)
        {
            if (lowestAngle == -1)
            {
                PlayerMovement.LockedOn = false;
                _playerAC.SetBool ("LockedOn", false);
                lowestAngle = -1f;
                _characterControllerCamera.enabled = true;
                EnemyController enemyController = _lockedOnEnemy.GetComponent<EnemyController> ();
                enemyController.LockedOn = false;

                foreach (GameObject enemy in _visibleEnemies)
                {
                    enemy.GetComponent<EnemyController> ().OnList = false;
                }

                _visibleEnemies.Clear ();
                _lockOnCamera.enabled = false;
                _lockOnCamera.LockOnTarget = null;
                _lockedOnEnemy = null;
            }
        }        

        if (_lockedOnEnemy != null)
        {
            distanceToEnemy = Vector3.Distance (transform.position, _lockedOnEnemy.transform.position);

            if(distanceToEnemy > _playerManager.SetupFields.LockOnRadius)
            {
                PlayerMovement.LockedOn = false;
                _playerAC.SetBool ("LockedOn", false);
                lowestAngle = -1f;
                distanceToEnemy = 0;
                _lockOnCamera.enabled = false;
                _characterControllerCamera.enabled = true;
                _lockOnCamera.LockOnTarget = null;
                _lockedOnEnemy.GetComponent<EnemyController> ().LockedOn = false;

                foreach (GameObject enemy in _visibleEnemies)
                {
                    enemy.GetComponent<EnemyController> ().OnList = false;
                }

                _visibleEnemies.Clear ();
                _lockedOnEnemy = null;
            }
        }
        #endregion
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
            if (_currentTimer > 0.0f && _playerAC.GetBool ("RollEnabled") && _playerAC.GetBool("IsMoving") && _playerStats.CurrentStamina > 0.0f)
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
            if (_currentTimer <= 0.0f && _playerAC.GetBool("IsMoving") && !_playerAC.GetBool("IsBlocking"))
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

        if(_playerAC.GetBool("IsRunning"))
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
    private void Attack( )
    {
        if(Input.GetButtonDown("Attack") && _gameManager.StateOfGame != GameManager.GameState.InMenu && !_playerAC.GetBool("AttackQeued"))
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

                        if(_playerAC.GetBool("Attacking") || _playerAC.GetBool("HeavyAttacking"))
                        {
                            _playerAC.SetBool ("AttackQeued", true);
                        }
                    }
                }                
            }
        }
    }

    private void HeavyAttack ( )
    {
        if (Input.GetButtonDown ("HeavyAttack") && _gameManager.StateOfGame != GameManager.GameState.InMenu && !_playerAC.GetBool ("AttackQeued"))
        {
            if (_playerStats.CurrentStamina > 0)
            {
                _playerAC.SetTrigger ("HeavyAttack");  
                
                if(_playerAC.GetBool("Attacking") || _playerAC.GetBool ("HeavyAttacking"))
                {
                    _playerAC.SetBool ("AttackQeued", true);
                }
            }
        }
    }

    private void Block ( )
    {
        if(Input.GetButton("Block") && _playerAC.GetBool("ShieldEquipped"))
        {
            _playerAC.SetBool ("IsBlocking", true);
        }
        else
        {
            _playerAC.SetBool("IsBlocking", false);
        }
    }

    private void AccesMenu()
    {
        if(Input.GetButtonDown("Menu"))
        {
            if (Menu.activeSelf)
            {
                Menu.SetActive (false);
                EquipmentHUD.SetActive (true);
                _characterControllerCamera.enabled = true;
                _gameManager.StateOfGame = GameManager.GameState.Playing;
            }
            else
            {
                Menu.SetActive (true);
                EquipmentHUD.SetActive (false);
                _characterControllerCamera.enabled = false;
                StartCoroutine (SetInventoryCameraPosition ());
                Menu.GetComponentInChildren<Button> ().Select ();
                _gameManager.StateOfGame = GameManager.GameState.InMenu;
            }
        }
    }

    IEnumerator SetInventoryCameraPosition()
    {
        while (Vector3.Distance (MainCameraRig.transform.position, _inventoryCameraPosition.position) > 0.05f)
        {
            MainCameraRig.transform.position = Vector3.Lerp (MainCameraRig.transform.position, _inventoryCameraPosition.position, 0.1f);
            MainCameraRigPivot.transform.rotation = Quaternion.Lerp (MainCameraRigPivot.transform.rotation, _inventoryCameraPosition.rotation, 0.1f);

            yield return null;
        }


        yield return 0;
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
