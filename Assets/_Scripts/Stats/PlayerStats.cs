using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : CharacterStats
{    
    public static PlayerStats instance;

    [Tooltip ("Amount of stamina per second used while running")]
    public float RunStaminaUsage;
    [Tooltip ("Amount of stamina used for a roll")]
    public float RollStaminaUsage;
    [Tooltip ("Amount of stamina used for an attack")]
    public float AttackStaminaUsage;
    [Tooltip ("Amount of stamina used for heavy attack")]
    public float HeavyAttackStaminaUsage;
    [Tooltip ("Amount of stamina regained every second")]
    public float StaminaGain;

    public Image HealthBar;
    public Image HealthBarOverTime;
    public Image StaminaBar;
    public Image StaminaBarOverTime;

    public float DecrementOverTime;                                                 //Used to control speed of health over time depletion

    private EquipmentManager _equipmentManager;

    private void Awake ( )
    {
        if(instance == null)
        {
            instance = this;
        }
    }      

    private void Start ( )
    {
        _equipmentManager = EquipmentManager.instance;
        _equipmentManager.OnEquipmentChangedCallback += OnEquipmentChanged;

        HealthBar.fillAmount = CurrentHealth / MaxHealth;
        HealthBarOverTime.fillAmount = CurrentHealth / MaxHealth;
        StaminaBar.fillAmount = CurrentStamina / MaxStamina;
        StaminaBarOverTime.fillAmount = CurrentStamina / MaxStamina;
    }

    private void Update ( )
    {
        if(HealthBar.fillAmount != CurrentHealth / MaxHealth || StaminaBar.fillAmount != CurrentStamina / MaxStamina)
        {
            UpdateUI ();
        }
    }

    private void OnEquipmentChanged ( Equipment newItem, Equipment oldItem )
    {
        if (newItem != null)
        {            
            if(newItem.ScalingStat.Count != 0)
            {
                for (int i = 0; i < newItem.ScalingStat.Count; i++)
                {
                    switch(newItem.ScalingStat[i])
                    {
                        case ScalingStatType.Dexterity:
                            Damage.AddModifier (newItem.BaseDamage + (newItem.ScalePercent[i] * Dexterity.GetValue() / 100));
                            HeavyDamage.AddModifier (Mathf.RoundToInt ((newItem.BaseDamage + (newItem.ScalePercent [i] * Dexterity.GetValue () / 100)) * newItem.HeavyAttackDamageModifier));
                            break;

                        case ScalingStatType.Inteligence:
                            Damage.AddModifier (newItem.BaseDamage + (newItem.ScalePercent [i] * Inteligence.GetValue () / 100 ));
                            HeavyDamage.AddModifier (Mathf.RoundToInt ((newItem.BaseDamage + (newItem.ScalePercent [i] * Inteligence.GetValue () / 100)) * newItem.HeavyAttackDamageModifier));
                            break;

                        case ScalingStatType.Strength:
                            Damage.AddModifier (newItem.BaseDamage + (newItem.ScalePercent [i] * Strength.GetValue () / 100));
                            HeavyDamage.AddModifier (Mathf.RoundToInt ((newItem.BaseDamage + (newItem.ScalePercent [i] * Strength.GetValue () / 100)) * newItem.HeavyAttackDamageModifier));
                            break;

                        case ScalingStatType.Wisdom:
                            Damage.AddModifier (newItem.BaseDamage + (newItem.ScalePercent [i] * Wisdom.GetValue () / 100));
                            HeavyDamage.AddModifier (Mathf.RoundToInt ((newItem.BaseDamage + (newItem.ScalePercent [i] * Wisdom.GetValue () / 100)) * newItem.HeavyAttackDamageModifier));
                            break;
                    }
                }
            }
            
        }

        if (oldItem != null)
        {
            if (oldItem.ScalingStat.Count != 0)
            {
                for (int i = 0; i < oldItem.ScalingStat.Count; i++)
                {
                    switch (oldItem.ScalingStat [i])
                    {
                        case ScalingStatType.Dexterity:
                            Damage.RemoveModifier (oldItem.BaseDamage + (oldItem.ScalePercent [i] * Dexterity.GetValue () / 100));
                            HeavyDamage.RemoveModifier (Mathf.RoundToInt ((oldItem.BaseDamage + (oldItem.ScalePercent [i] * Dexterity.GetValue () / 100)) * oldItem.HeavyAttackDamageModifier));
                            break;

                        case ScalingStatType.Inteligence:
                            Damage.RemoveModifier (oldItem.BaseDamage + (oldItem.ScalePercent [i] * Inteligence.GetValue () / 100));
                            HeavyDamage.RemoveModifier (Mathf.RoundToInt ((oldItem.BaseDamage + (oldItem.ScalePercent [i] * Inteligence.GetValue () / 100)) * oldItem.HeavyAttackDamageModifier));
                            break;

                        case ScalingStatType.Strength:
                            Damage.RemoveModifier (oldItem.BaseDamage + (oldItem.ScalePercent [i] * Strength.GetValue () / 100));
                            HeavyDamage.RemoveModifier (Mathf.RoundToInt ((oldItem.BaseDamage + (oldItem.ScalePercent [i] * Strength.GetValue () / 100)) * oldItem.HeavyAttackDamageModifier));
                            break;

                        case ScalingStatType.Wisdom:
                            Damage.RemoveModifier (oldItem.BaseDamage + (oldItem.ScalePercent [i] * Wisdom.GetValue () / 100));
                            HeavyDamage.RemoveModifier (Mathf.RoundToInt((oldItem.BaseDamage + (oldItem.ScalePercent [i] * Wisdom.GetValue () / 100)) * oldItem.HeavyAttackDamageModifier));
                            break;
                    }
                }
            }
        }
    }

    public void UpdateUI ( )
    {
        HealthBar.fillAmount = CurrentHealth / MaxHealth;        
        StaminaBar.fillAmount = CurrentStamina / MaxStamina;

        if(HealthBar.fillAmount > HealthBarOverTime.fillAmount)
        {
            HealthBarOverTime.fillAmount = HealthBar.fillAmount;
        }

        if(StaminaBar.fillAmount > StaminaBarOverTime.fillAmount)
        {
            StaminaBarOverTime.fillAmount = StaminaBar.fillAmount;
        }

        StartCoroutine (UpdateUIOverTime ());
    }

    private IEnumerator UpdateUIOverTime()
    {
        while(HealthBarOverTime.fillAmount != CurrentHealth / MaxHealth)
        {
            if(HealthBar.fillAmount < HealthBarOverTime.fillAmount)
            {
                HealthBarOverTime.fillAmount -= DecrementOverTime * Time.deltaTime;
            }
            break;
        }

        while (StaminaBarOverTime.fillAmount != CurrentStamina / MaxStamina)
        {
            if(StaminaBar.fillAmount < StaminaBarOverTime.fillAmount)
            {
                StaminaBarOverTime.fillAmount -= DecrementOverTime * Time.deltaTime;
            }
            break;
        }

        yield return 0;
    }
}
