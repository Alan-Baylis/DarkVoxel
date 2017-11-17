using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitcher : MonoBehaviour
{
    public GameObject LightToSwitch;

    private void OnTriggerEnter ( Collider other )
    {
        if(other.transform.root.CompareTag("Player"))
        {
            LightToSwitch.SetActive (!LightToSwitch.activeSelf);
        }
    }
}
