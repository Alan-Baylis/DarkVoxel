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
    
    private CharacterControllerCamera _characterControllerCamera;    
    
    private void Awake ( )
    {
        if(instance == null)
        {
            instance = this;
        }       
    }

    private void Start ( )
    {
        _characterControllerCamera = CharacterControllerCamera.instance;        
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
            Quaternion newRotation = Quaternion.LookRotation (direction); 
            Mathf.Clamp (newRotation.x, _minimumCameraTilt, _maximumCameraTilt);
            MainCameraPivot.transform.rotation = Quaternion.Lerp (MainCameraPivot.transform.rotation, newRotation, 10.0f * Time.deltaTime);
            MainCameraPivot.transform.localEulerAngles = new Vector3 (Mathf.Clamp (MainCameraPivot.transform.localEulerAngles.x, _minimumCameraTilt, _maximumCameraTilt), MainCameraPivot.transform.localEulerAngles.y, 0.0f);
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
