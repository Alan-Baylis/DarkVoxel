using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    #region Singleton
    public static Inventory instance;

    private void Awake ( )
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    #endregion    
    public int StackAmount = 1;

    public List<Item> Items = new List<Item> ();
    public List<GameObject> Slots = new List<GameObject> ();

    public delegate void OnItemChanged ( );
    public OnItemChanged OnItemChangedCallback;

    public GameObject InventoryContent;

    private GameObject _slotPanel;
    [SerializeField] private GameObject _inventorySlot;  

    private ItemDatabase _database;

    private int _slotAmount;

    private void Start ( )
    {
        _database = GetComponent<ItemDatabase> ();

        _slotAmount = 20;        
        _slotPanel = InventoryContent.transform.Find("SlotPanel").gameObject;

        for (int i = 0; i < _slotAmount; i++)
        {
            
            Slots.Add (Instantiate(_inventorySlot));
            Slots [i].transform.SetParent (_slotPanel.transform);
            Slots [i].transform.localPosition = Vector2.zero;
        }      
    }

    public bool AddItem(Item item)
    {
       if(!item.IsDefaultItem)
        {
            Items.Add (item);

            if (OnItemChangedCallback != null)
            {
                OnItemChangedCallback.Invoke ();
            }
        }
              
        return true;               
    }

    private bool CheckIfItemIsInInventory(Item item)
    {
        for (int i = 0; i < Items.Count; i++)
        {
            if(Items[i].Name == item.Name)
            {
                return true;
            }
        }
        return false;
    }

    public void Remove ( Item item )
    {
        Items.Remove (item);

        if (OnItemChangedCallback != null)
        {
            OnItemChangedCallback.Invoke ();           
        }
    }
}
