using UnityEngine;

public class Item : ScriptableObject
{
    public string Name = "New Item";

    public Sprite Icon = null;

    public enum ItemType { Item, Equipment }
    public ItemType TypeOfItem;

    public bool IsDefaultItem = false;
    public bool Stackable = false;       

    public virtual void Use()
    {
        //Use the item           

        Debug.Log ("Using " + name);
    }

    public void RemoveFromInventory()
    {
        Inventory.instance.Remove (this);
    }
}
