using UnityEngine;
using UnityEngine.UI;

public class Interactable : MonoBehaviour
{
    public Transform InteractionTransform;

    public GameObject InteractPrompt;
    public GameObject OpenChestPrompt;              //Button for opening the chest

    public bool HasInteracted = false;

    private bool _isFocused = false;

    private Transform _player;

    public virtual void Start ( )
    {

    }

    public virtual void Interact()
    {
        Debug.Log ("Interacting with " + transform.name);        
    }    

    public void OnFocused(Transform playerTransform)
    {
        _isFocused = true;
        _player = playerTransform;
        
        switch (InteractionTransform.GetComponentInParent<ObjectSorter>().TypeOfObject)
        {
            case ObjectSorter.ObjectType.Chest:
                if(!InteractPrompt.activeSelf && !InteractionTransform.GetComponentInParent<Chest>().ChestOpened)
                {
                    OpenChestPrompt.SetActive (true);
                    OpenChestPrompt.GetComponent<Button> ().Select ();
                }
                else if(!InteractPrompt.activeSelf && !InteractionTransform.GetComponentInParent<Chest>().HasInteracted)
                {
                    InteractPrompt.SetActive (true);
                    InteractPrompt.GetComponent<Button> ().Select ();
                }
                break;
        }             
    }

    public void OnDefocused()
    {
        switch (InteractionTransform.GetComponentInParent<ObjectSorter> ().TypeOfObject)
        {
            case ObjectSorter.ObjectType.Chest:
                if (!InteractPrompt.activeSelf && !InteractionTransform.GetComponentInParent<Chest> ().ChestOpened)
                {
                    OpenChestPrompt.SetActive (false);                    
                }
                else if (InteractPrompt.activeSelf && !InteractionTransform.GetComponentInParent<Chest> ().HasInteracted)
                {
                    InteractPrompt.SetActive (false);                    
                }
                break;
        }

        _isFocused = false;
        _player = null;
    }

    private void OnDrawGizmosSelected ( )
    {
        if(InteractionTransform == null)
        {
            InteractionTransform = transform;
        }

        Gizmos.color = Color.yellow;        
    }
}
