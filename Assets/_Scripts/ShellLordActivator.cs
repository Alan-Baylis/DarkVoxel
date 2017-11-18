using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellLordActivator : MonoBehaviour
{
    public GameObject ShellLord;

    public Animator ShellLordAC;

    private void OnTriggerEnter ( Collider other )
    {
        if(other.transform.root.CompareTag("Player"))
        {
            ShellLord.SetActive (true);
            ShellLordAC.SetTrigger ("Fly");
            ShellLordAC.SetBool ("IsFlying", true);
        }
    }
}
