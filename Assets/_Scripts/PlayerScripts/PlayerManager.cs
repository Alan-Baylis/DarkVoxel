using UnityEngine;

public class PlayerManager: MonoBehaviour
{
    public static PlayerManager instance;

    public Interactable Focus;

    private GameManager _gameManager;

    public float InteractionAngle = 90.0f;

    private float _angle;    

    [System.Serializable]
    public class PlayerSetupFields
    {
        public float LockOnRadius;

        [Tooltip("Determains angle in front of player in which he can see enemies")]
        public float FieldOfViewAngle = 110f;

        [Tooltip("Determains rotation speed while locked on")]
        public float RotationSpeed = 1000;
    }

    public PlayerSetupFields SetupFields = new PlayerSetupFields ();  

    private void Awake ( )
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    private void Start ( )
    {
        _gameManager = GameManager.instance;
    }

    private void SetFocus(Interactable newFocus)
    {
        if(newFocus != Focus)
        {
            if (Focus != null)
            {
                Focus.OnDefocused ();
            }
            Focus = newFocus;
            Interact.InteractionObject = newFocus.gameObject;
        }
        
        newFocus.OnFocused (transform);
    }

    private void RemoveFocus()
    {
        if (Focus != null)
        {
            Focus.OnDefocused ();
        }

        Focus = null;
    }

    private void OnTriggerEnter ( Collider other )
    {
        if (other.gameObject.CompareTag ("SurfaceChanger"))
        {
            _gameManager.TypeOfSurface = other.gameObject.GetComponent<SurfaceChanger> ().TypeOfSurface;
        }
    }

    private void OnTriggerStay ( Collider other )
    {
        if(other.gameObject.CompareTag("Interactable"))
        {
            Vector3 direction = other.transform.parent.position - transform.position;
            _angle = Vector3.Angle (direction, transform.forward);

            if (_angle <= InteractionAngle)
            {
                SetFocus (other.GetComponentInParent<Interactable> ());
            }
            else
            {
                RemoveFocus ();
            }
        }       
    }   

    private void OnTriggerExit ( Collider other )
    {
        if(other.gameObject.CompareTag("Interactable"))
        {
            RemoveFocus ();
        }
    }

    private void OnDrawGizmos ( )
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere (transform.position, SetupFields.LockOnRadius);
    }
}
