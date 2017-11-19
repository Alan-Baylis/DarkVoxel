using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitcher : MonoBehaviour
{
    public List<GameObject> LightsToSwitch;

    private void OnTriggerEnter ( Collider other )
    {
        if(other.transform.root.CompareTag("Player"))
        {
            for (int i = 0; i < LightsToSwitch.Count; i++)
            {
                LightsToSwitch[i].SetActive (!LightsToSwitch[i].activeSelf);
            }           
        }
    }
}
