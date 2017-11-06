using UnityEngine;

public class ItemPickup : Interactable
{
    public Item Item;

    public override void Interact ( )
    {
        base.Interact ();

        PickUp ();
    }

    private void PickUp()
    {
        Debug.Log ("Picking up " + Item.Name);
        bool wasPickedUp = Inventory.instance.AddItem (Item);

        if (wasPickedUp)
        {
            Destroy (gameObject);
        }
    }
}
