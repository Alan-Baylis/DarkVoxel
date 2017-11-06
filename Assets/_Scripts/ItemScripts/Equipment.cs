using System.Collections.Generic;
using UnityEngine;

//An item that can be equipped
[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Equipment")]
public class Equipment : Item
{
    public EquipmentSlot EquipSlot;     //Slot to store item in
    public WeaponType TypeOfWeapon;     //Weapon type used. Needed to set right animations
    public SkinnedMeshRenderer Mesh;
    public SkinnedMeshRenderer MeshNumberTwo;
    public EquipmentMeshRegion [] CoveredMeshRegions;
       
    public List<ScalingStatType> ScalingStat;
    public List<int> ScalePercent;

    public int BaseDamage;
    public int BaseArmour;

    public float HeavyAttackDamageModifier = 2.0f;

    //When item is used in the inventory
    public override void Use ( )
    {
        base.Use ();

        EquipmentManager.instance.Equip (this);

        RemoveFromInventory ();                                         //Unequip item        
    }
}

public enum EquipmentSlot { Head, Chest, Legs, Feet, LeftHand, RightHand, DualWield }
public enum EquipmentMeshRegion { Hip, Thighs, Calvs, Chest, ForearmLeft, ForearmRight, ArmLeft, ArmRight, Head, FootLeft, FootRight }
public enum WeaponType { ArmingSword, StraightSword, Knives, Axe, Bow, Crossbow, Polearm, Shield, None }
public enum ScalingStatType { Strength, Dexterity, Inteligence, Wisdom }
