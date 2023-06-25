using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LaynarGames.Flex.Gameplay
{
    public class RetargetRig : MonoBehaviour
    {
        [SerializeField] SkinnedMeshRenderer targetSkinnedMesh;
        [SerializeField] SkinnedMeshRenderer thisSkinnedMesh;

        [ContextMenu("Retarget Skinned Mesh")]
        public void RetargetSkinnedMesh()
        {
            Debug.Log("Retargeted");
            thisSkinnedMesh.bones = targetSkinnedMesh.bones;
        }
    }
}

