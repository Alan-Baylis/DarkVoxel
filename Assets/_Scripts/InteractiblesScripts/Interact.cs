using System.Collections;
using UnityEngine;


public class Interact : MonoBehaviour
{
    public Transform Player;
    public static GameObject InteractionObject;    

    public void InteractWithObject ( )
    {
        switch (InteractionObject.GetComponent<ObjectSorter> ().TypeOfObject)
        {
            case ObjectSorter.ObjectType.Chest:
                if (!InteractionObject.GetComponent<Chest> ().ChestOpened)
                {
                    StartCoroutine (PositionPlayerInFrontOfInteractible (0.1f));
                    StartCoroutine (StopCoroutines ());
                }                
                InteractionObject.GetComponent<Chest> ().Interact ();
                break;

            case ObjectSorter.ObjectType.Checkpoint:

                break;
        }
    }

    public void PickUpItems ( )
    {
        switch (InteractionObject.GetComponent<ObjectSorter> ().TypeOfObject)
        {
            case ObjectSorter.ObjectType.Chest:
                InteractionObject.GetComponent<Chest> ().TakeItems ();
                break;
        }
    }

    private IEnumerator PositionPlayerInFrontOfInteractible ( float time )
    {
        var targetRotation = InteractionObject.transform.rotation.eulerAngles;
        targetRotation.x = 0.0f;
        targetRotation.y -= 180.0f;
        targetRotation.z = 0.0f;
        while (Player.position != InteractionObject.transform.GetChild(0).transform.position)
        {
            Player.transform.position = Vector3.Lerp (Player.transform.position, InteractionObject.transform.GetChild (0).transform.position, time);
            Player.transform.rotation = Quaternion.Lerp (Player.transform.rotation, Quaternion.Euler(targetRotation), time);
            yield return 0;
        }      
    } 
    
    private IEnumerator StopCoroutines ()
    {
        yield return new WaitForSeconds (1f);
        StopAllCoroutines ();
    }
}
