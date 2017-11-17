using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEquipmentManager : MonoBehaviour
{
    public List<Equipment> EnemyEquipment;

    public SkinnedMeshRenderer TargetMesh;

    public Transform LeftHandEquipment;
    public Transform RightHandEquipment;

    private void Start ( )
    {
        for (int i = 0; i < EnemyEquipment.Count; i++)
        {
            Equipment equipment = EnemyEquipment [i];
            SkinnedMeshRenderer newMesh = Instantiate (equipment.Mesh);

            switch(equipment.EquipSlot)
            {
                case EquipmentSlot.LeftHand:
                    newMesh.transform.parent = LeftHandEquipment;
                    newMesh.transform.localPosition = Vector3.zero;
                    newMesh.transform.localRotation = Quaternion.identity;
                    break;

                case EquipmentSlot.RightHand:
                    newMesh.transform.parent = RightHandEquipment;
                    newMesh.transform.localPosition = Vector3.zero;
                    newMesh.transform.localRotation = Quaternion.identity;
                    break;

                case EquipmentSlot.Chest:
                    newMesh.transform.parent = TargetMesh.transform;
                    newMesh.transform.localPosition = Vector3.zero;
                    newMesh.transform.localRotation = Quaternion.identity;
                    break;

                case EquipmentSlot.Feet:
                    newMesh.transform.parent = TargetMesh.transform;
                    newMesh.transform.localPosition = Vector3.zero;
                    newMesh.transform.localRotation = Quaternion.identity;
                    break;

                case EquipmentSlot.Head:
                    newMesh.transform.parent = TargetMesh.transform;
                    newMesh.transform.localPosition = Vector3.zero;
                    newMesh.transform.localRotation = Quaternion.identity;
                    break;

                case EquipmentSlot.Legs:
                    newMesh.transform.parent = TargetMesh.transform;
                    newMesh.transform.localPosition = Vector3.zero;
                    newMesh.transform.localRotation = Quaternion.identity;
                    break;
            }           

            newMesh.bones = TargetMesh.bones;
            newMesh.rootBone = TargetMesh.rootBone;

            SetEquipmentBlendShapes (equipment, 100);
        }
    }

    private void SetEquipmentBlendShapes ( Equipment equipment, int weight )
    {
        foreach (EquipmentMeshRegion blendShape in equipment.CoveredMeshRegions)
        {
            TargetMesh.SetBlendShapeWeight ((int) blendShape, weight);
        }
    }
}
