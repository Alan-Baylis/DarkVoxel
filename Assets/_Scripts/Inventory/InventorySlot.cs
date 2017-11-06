using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{    
    public Text _numberOfItems;

    public Image Icon;    

    private Item _item;

    private void Start ( )
    {
        _numberOfItems = GetComponentInChildren<Text> ();
    }

    public void AddItem(Item newItem)
    {
        _item = newItem;

        Icon.sprite = _item.Icon;

        if (!newItem.IsDefaultItem)
        {
            Icon.enabled = true;
        }
        else
        {
            Icon.enabled = false;
        }
    }

    public void ClearSlot()
    {
        _item = null;

        Icon.sprite = null;
        Icon.enabled = false;
    }

    public void OnRemoveButton()
    {
        Inventory.instance.Remove (_item);
    }

    public void UseItem()
    {
        if(_item != null)
        {            
            _item.Use ();            
        }
    }    
}
