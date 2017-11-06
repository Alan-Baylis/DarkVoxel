using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public Transform ItemsParent;

    private Inventory _inventory;

    private Item _item;

    private InventorySlot [] _slots;

    private void Start ( )
    {
        _inventory = Inventory.instance;
        _inventory.OnItemChangedCallback += UpdateUI;       
    } 

    private void UpdateUI()
    {
        _slots = ItemsParent.GetComponentsInChildren<InventorySlot> ();

        for (int i = 0; i < _slots.Length; i++)
        {
            if(i < _inventory.Items.Count)
            {
                _slots [i].AddItem (_inventory.Items [i]);
            }
            else
            {
                _slots[i].ClearSlot();
            }
        }
    }
}
