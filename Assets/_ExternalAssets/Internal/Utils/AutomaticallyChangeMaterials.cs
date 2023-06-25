using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticallyChangeMaterials : MonoBehaviour
{
    [SerializeField] Material desiredMaterial;
    [ContextMenu("Change Material")]
    public void ChangeMaterial()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
        foreach(Renderer renderer in renderers)
        {
            Material[] materials = new Material[renderer.materials.Length];
            for(int i=0; i<materials.Length; i++)
            {
                materials[i] = desiredMaterial;
            }

            renderer.materials = materials;
        }
    }
}
