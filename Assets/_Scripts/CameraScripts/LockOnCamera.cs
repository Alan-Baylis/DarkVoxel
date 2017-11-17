using UnityEngine;
using System.Collections;

public class LockOnCamera : MonoBehaviour
{
    public static LockOnCamera instance;

    public Transform LockOnTarget;
   
    public Transform PlayerTransform;
    public Transform MainCameraPivot;
    
    [SerializeField] private float _yOffset;
    [SerializeField] private float _cameraSmoothTime = 1f;   
    
    [Space]
    [SerializeField] private float _minimumCameraTilt;
    [SerializeField] private float _maximumCameraTilt;
    
    [SerializeField] private CharacterControllerCamera _characterControllerCamera;   

    private Vector3 _lastForward;
    private Vector3 _currentForward;

    private float _cameraLookAngle;

    public float angle;
    
    private void Awake ( )
    {
        if(instance == null)
        {
            instance = this;
        }       
    }

    private void OnDisable ( )
    {
        _characterControllerCamera._cameraLookAngle = _cameraLookAngle;
    }  

    private void Start ( )
    {
        _characterControllerCamera = CharacterControllerCamera.instance;

        _cameraLookAngle = _characterControllerCamera._cameraLookAngle;
        _lastForward = MainCameraPivot.transform.forward;
    }

    private void OnEnable ( )
    {
        _cameraLookAngle = _characterControllerCamera._cameraLookAngle;
        _lastForward = MainCameraPivot.transform.forward;
    }

    private void LateUpdate ( )
    {
        PositionCamera ();
        FocusOnTarget ();        
    }

    private void FocusOnTarget()
    {
        if (LockOnTarget != null)
        {            
            Vector3 direction = LockOnTarget.transform.position - MainCameraPivot.transform.position;

            _currentForward = MainCameraPivot.transform.forward;

            angle = Vector3.Angle (_currentForward, _lastForward);
           
            if(Vector3.Cross(_currentForward, _lastForward).y > 0)
            {
                angle = -angle;
            }
            
            _cameraLookAngle += angle;             

            Quaternion newRotation = Quaternion.LookRotation (direction);            
            MainCameraPivot.transform.rotation = Quaternion.Lerp (MainCameraPivot.transform.rotation, newRotation, 10.0f * Time.deltaTime);
            MainCameraPivot.transform.localEulerAngles = new Vector3 (Mathf.Clamp (MainCameraPivot.transform.localEulerAngles.x, _minimumCameraTilt, _maximumCameraTilt), MainCameraPivot.transform.localEulerAngles.y, 0.0f);

            _lastForward = _currentForward;
        }                
    }

    private void PositionCamera()
    {
        float xPosition = Mathf.Lerp (transform.position.x, PlayerTransform.position.x, _cameraSmoothTime * Time.deltaTime);
        float yPosition = Mathf.Lerp (transform.position.y, PlayerTransform.position.y + transform.up.y * _yOffset, _cameraSmoothTime * Time.deltaTime);
        float zPosition = Mathf.Lerp (transform.position.z, PlayerTransform.position.z, _cameraSmoothTime * Time.deltaTime);

        Vector3 newPosition = new Vector3 (xPosition, yPosition, zPosition);
        
        transform.position = newPosition;
    }  

    private void OnDrawGizmos ( )
    {
        Gizmos.color = Color.red;

        if(LockOnTarget != null)
        Gizmos.DrawLine (transform.position, LockOnTarget.position);        
    }    
}
