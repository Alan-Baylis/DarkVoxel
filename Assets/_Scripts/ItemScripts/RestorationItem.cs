using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New Restoration Item", menuName = "Inventory/Stackable Items/ Restoration Items")]
public class RestorationItem : Item
{
    public enum RestorationType { Health }
    public RestorationType TypeOfRestoration;
        
    public int RestorationAmount = 50;

    public override void Use ( )
    {
        base.Use ();

        if (TypeOfRestoration == RestorationType.Health)
        {
            PlayerStats.instance.Heal (RestorationAmount);
        }
    }
}
