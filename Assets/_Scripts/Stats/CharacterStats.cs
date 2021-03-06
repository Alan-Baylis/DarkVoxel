﻿using UnityEngine;
using UnityEngine.AI;

public class CharacterStats : MonoBehaviour
{
    public int MaxHealth = 100;
    public int MaxStamina = 100;

    [Space(10)]
    public float CurrentHealth;
    public float CurrentStamina;   

    public Stat Damage;
    public Stat HeavyDamage;
    public Stat Armour;

    public Stat Strength;
    public Stat Dexterity;
    public Stat Constitution;
    public Stat Tenacity;
    public Stat Inteligence;
    public Stat Wisdom;
    public Stat Charisma;

    public bool CanRegainStamina = true;

    public Animator CharacterAC;

    public NavMeshAgent Agent;
    public Rigidbody RB;

    private void Start ( )
    {
        CurrentHealth = MaxHealth;
        CurrentStamina = MaxStamina;        
    }    

    public void TakeDamage(int amount)
    {
        //take armour into acount
        amount -= Armour.GetValue();
        amount = Mathf.Clamp (amount, 0, int.MaxValue);

        CurrentHealth -= amount;        
        Debug.Log (transform.name + " takes " + amount + " damage.");

        if(CurrentHealth <= 0)
        {
            Die ();
        }
    }

    public void Heal(int amount)
    {
        CurrentHealth += amount;
        CurrentHealth = Mathf.Clamp (amount, 0, MaxHealth);
    }

    public void TakeDamageWithStamina(float amount)
    {
        CurrentStamina -= amount;
    }

    public void UseStamina(float amount)
    {
        CurrentStamina -= amount * Time.deltaTime;        
    }

    public void RegainStamina(float amount)
    {
        if (CurrentStamina < MaxStamina)
        {
            if (CanRegainStamina)
            {
                CurrentStamina += amount * Time.deltaTime;
            }
        }
        else
        {
            CurrentStamina = MaxStamina;
        }
    }

    private void Die()
    {
        if (!CharacterAC.GetBool ("Dead"))
        {
            CharacterAC.SetTrigger ("Die");
            CharacterAC.SetBool ("Dead", true);
        }

        Collider [] colliders = GetComponentsInChildren<Collider> ();
        MonoBehaviour [] scripts = GetComponents<MonoBehaviour> ();

        foreach (Collider collider in colliders)
        {
            //collider.enabled = false;
        }

        if(Agent != null)
        {
            Agent.enabled = false;
        }

        foreach (MonoBehaviour script in scripts)
        {
            script.enabled = false;
        } 
        
        if(RB != null)
        {
            //RB.useGravity = false;
            //RB.isKinematic = true;
        }
    }     
}
