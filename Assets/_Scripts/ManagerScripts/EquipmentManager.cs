using UnityEngine;
using UnityEngine.UI;

//Keep track of equipment.Has functions for adding and removing items.

public class EquipmentManager : MonoBehaviour
{
    #region Singleton
    public static EquipmentManager instance;

    private void Awake ( )
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    #endregion

    //Callback for when an item is equipped/unequipped
    public delegate void OnEquipmentChanged ( Equipment newEquipment, Equipment oldEquipment );
    public OnEquipmentChanged OnEquipmentChangedCallback;

    public SkinnedMeshRenderer TargetMesh;

    public Transform RightHandTransform;
    public Transform LeftHandTransform;

    [Header("Inventory equipment slots")]
    public Image HeadSlotImage;
    public Image TorsoSlotImage;
    public Image LegsSlotImage;
    public Image FeetSlotImage;
    public Image LeftArmSlotImage;
    public Image RightArmSlotImage;

    [Header ("HUD equipment slots")]
    public Image RightHandEquipment;
    public Image LeftHandEquipment;
    public Image HUDItem;
    public Image HUDUsableItem;

    [Space(20.0f)]
    public Equipment [] DefaultEquipment;

    public Animator PlayerAC;                   //Used for setting correct animations with correct equipment

    private Equipment [] _currentEquipment;     //Curently equipped items
    private SkinnedMeshRenderer [] _currentMeshes;

    private Inventory _inventory;               //Reference to inventory

    private void Start ( )
    {
        _inventory = Inventory.instance;

        //Initialize currentEquipment based on number of equipment slots
        int numberOfSlots = System.Enum.GetNames (typeof (EquipmentSlot)).Length;
        _currentEquipment = new Equipment [numberOfSlots];
        _currentMeshes = new SkinnedMeshRenderer [numberOfSlots];

        EquipDefaultItems ();
    }

    //Equip a new item
    public void Equip ( Equipment newEquipment )
    {
        //Find outwhat slot the item fits in        
        int slotIndex = (int) newEquipment.EquipSlot;          
        Equipment oldEquipment = Unequip (slotIndex);
        #region WeaponType
        switch (newEquipment.TypeOfWeapon)
        {
            case WeaponType.ArmingSword:
                PlayerAC.SetBool ("ArmingSwordEquipped", true);
                break;

            case WeaponType.Axe:
                PlayerAC.SetBool ("AxeEquipped", true);
                break;

            case WeaponType.Bow:
                PlayerAC.SetBool ("BowEquipped", true);
                break;

            case WeaponType.Crossbow:
                PlayerAC.SetBool ("CrossbowEquipped", true);
                break;

            case WeaponType.Knives:
                PlayerAC.SetBool ("KnivesEquipped", true);
                break;

            case WeaponType.Polearm:
                PlayerAC.SetBool ("PolearmEquipped", true);
                break;

            case WeaponType.Shield:
                PlayerAC.SetBool ("ShieldEquipped", true);
                break;

            case WeaponType.StraightSword:
                PlayerAC.SetBool ("StraightSwordEquipped", true);
                break;
        }
        #endregion        

        if(_currentEquipment[slotIndex] != null)
        {
            oldEquipment = _currentEquipment [slotIndex];
            _inventory.AddItem (oldEquipment);
        }

        //An item has been equipped so we trigger the callback
        if (OnEquipmentChangedCallback != null)
        {
            OnEquipmentChangedCallback.Invoke (newEquipment, oldEquipment);
        }

        SetEquipmentBlendShapes (newEquipment, 100);

        //Insert the item into the slot   
        _currentEquipment [slotIndex] = newEquipment;
        SkinnedMeshRenderer newMesh = Instantiate (newEquipment.Mesh);      

        //Switch statement za različite tipove equipmenta
        switch(newEquipment.EquipSlot)
        {
            case EquipmentSlot.RightHand:
                newMesh.transform.parent = RightHandTransform;
                newMesh.transform.localPosition = Vector3.zero;
                newMesh.transform.localRotation = Quaternion.identity;
                RightArmSlotImage.sprite = newEquipment.Icon;
                RightArmSlotImage.enabled = true;
                RightHandEquipment.sprite = newEquipment.Icon;
                RightHandEquipment.enabled = true;
            break;

            case EquipmentSlot.LeftHand:
                newMesh.transform.parent = LeftHandTransform;
                newMesh.transform.position = Vector3.zero;
                LeftArmSlotImage.sprite = newEquipment.Icon;
                LeftArmSlotImage.enabled = true;
                LeftHandEquipment.sprite = newEquipment.Icon;
                LeftHandEquipment.enabled = true;
                break;

            case EquipmentSlot.Chest:
                newMesh.transform.parent = TargetMesh.transform;
                newMesh.transform.position = Vector3.zero;
                TorsoSlotImage.sprite = newEquipment.Icon;

                if (!newEquipment.IsDefaultItem)
                {
                    TorsoSlotImage.enabled = true;
                }
                else
                {
                    TorsoSlotImage.enabled = false;
                }
                break;

            case EquipmentSlot.Feet:
                newMesh.transform.parent = TargetMesh.transform;
                newMesh.transform.position = Vector3.zero;
                FeetSlotImage.sprite = newEquipment.Icon;

                if (!newEquipment.IsDefaultItem)
                {
                    FeetSlotImage.enabled = true;
                }
                else
                {
                    FeetSlotImage.enabled = false;
                }
                break;

            case EquipmentSlot.Head:
                newMesh.transform.parent = TargetMesh.transform;
                newMesh.transform.position = Vector3.zero;
                HeadSlotImage.sprite = newEquipment.Icon;

                if (!newEquipment.IsDefaultItem)
                {
                    HeadSlotImage.enabled = true;
                }
                else
                {
                    HeadSlotImage.enabled = false;
                }
                break;

            case EquipmentSlot.Legs:
                newMesh.transform.parent = TargetMesh.transform;
                newMesh.transform.position = Vector3.zero;
                LegsSlotImage.sprite = newEquipment.Icon;

                if (!newEquipment.IsDefaultItem)
                {
                    LegsSlotImage.enabled = true;
                }
                else
                {
                    LegsSlotImage.enabled = false;
                }
                break;

            case EquipmentSlot.DualWield:
                SkinnedMeshRenderer newMeshTwo = Instantiate (newEquipment.MeshNumberTwo);
                newMesh.transform.parent = LeftHandTransform.transform;
                newMesh.transform.position = Vector3.zero;
                newMeshTwo.transform.parent = RightHandTransform.transform;
                newMeshTwo.transform.position = Vector3.zero;
                newMeshTwo.bones = TargetMesh.bones;
                newMeshTwo.rootBone = TargetMesh.rootBone;
                break;
        }        

        newMesh.bones = TargetMesh.bones;
        newMesh.rootBone = TargetMesh.rootBone;
        _currentMeshes [slotIndex] = newMesh;

        //Remove item from inventory and place it into equipment slot
    }

    //Unequip an item with particular index
    public Equipment Unequip ( int slotIndex )
    {
        if (_currentEquipment [slotIndex] != null)
        {
            if (_currentMeshes [slotIndex] != null)
            {
                Destroy (_currentMeshes [slotIndex].gameObject);
            }

            //Add the item to inventory
            Equipment oldEquipment = _currentEquipment [slotIndex];
            SetEquipmentBlendShapes (oldEquipment, 0);
            _inventory.AddItem (oldEquipment);

            //Remove the item from the equipment array
            _currentEquipment [slotIndex] = null;

            #region WeaponType
            switch (oldEquipment.TypeOfWeapon)
            {
                case WeaponType.ArmingSword:
                    PlayerAC.SetBool ("ArmingSwordEquipped", false);
                    break;

                case WeaponType.Axe:
                    PlayerAC.SetBool ("AxeEquipped", false);
                    break;

                case WeaponType.Bow:
                    PlayerAC.SetBool ("BowEquipped", false);
                    break;

                case WeaponType.Crossbow:
                    PlayerAC.SetBool ("CrossbowEquipped", false);
                    break;

                case WeaponType.Knives:
                    PlayerAC.SetBool ("KnivesEquipped", false);
                    break;

                case WeaponType.Polearm:
                    PlayerAC.SetBool ("PolearmEquipped", false);
                    break;

                case WeaponType.Shield:
                    PlayerAC.SetBool ("ShieldEquipped", false);
                    break;

                case WeaponType.StraightSword:
                    PlayerAC.SetBool ("StraightSwordEquipped", false);
                    break;
            }
            #endregion

            //Equipment has been removed so we trigger the callback
            if (OnEquipmentChangedCallback != null)
            {
                OnEquipmentChangedCallback.Invoke (null, oldEquipment);
            }
            return oldEquipment;
        }
        return null;
    }

    private void SetEquipmentBlendShapes ( Equipment equipment, int weight )
    {
        foreach (EquipmentMeshRegion blendShape in equipment.CoveredMeshRegions)
        {
            TargetMesh.SetBlendShapeWeight ((int) blendShape, weight);
        }
    }

    private void EquipDefaultItems ( )
    {
        foreach (Equipment equipment in DefaultEquipment)
        {
            Equip (equipment);
        }
    }
}
