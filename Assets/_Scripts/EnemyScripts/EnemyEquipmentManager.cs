using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEquipmentManager : MonoBehaviour
{
    public List<Equipment> EnemyEquipment;

    public SkinnedMeshRenderer TargetMesh;

    private void Start ( )
    {
        for (int i = 0; i < EnemyEquipment.Count; i++)
        {
            Equipment equipment = EnemyEquipment [i];
            SkinnedMeshRenderer newMesh = Instantiate (equipment.Mesh);

            newMesh.transform.parent = TargetMesh.transform;
            newMesh.transform.position = Vector3.zero;

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
