using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat
{
    [SerializeField] private int _baseValue;

    private List<int> _modifiers = new List<int>();

    public int GetValue()
    {
        int finalValue = _baseValue;        
        return finalValue;
    }

    public void AddModifier(int modifier)
    {
        if(modifier != 0)
        {
            _modifiers.Add (modifier);
            _baseValue += modifier;
        }
    }

    public void RemoveModifier(int modifier)
    {
        if(modifier != 0)
        {
            _modifiers.Remove (modifier);
            _baseValue -= modifier;
        }
    }
}
