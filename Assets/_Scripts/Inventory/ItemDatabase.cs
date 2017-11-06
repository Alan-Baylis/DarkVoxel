using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    [SerializeField] private List<Item> _itemDatabase = new List<Item>();   

    public Item FetchItemByName(string name)
    {
        for (int i = 0; i < _itemDatabase.Count; i++)
        {
            if (_itemDatabase [i].Name == name)
            {
                return _itemDatabase [i];
            }            
        }
        return null;
    }  
}