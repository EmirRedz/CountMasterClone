using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorStateExtension : MonoBehaviour
{
    [SerializeField] Animator animator_;
    public void SetBoolTrue(string name) => animator_.SetBool(name, true);
 
    public void SetBoolFalse(string name) => animator_.SetBool(name, false);

    public void SetFloatToTen(string name)
    {
        animator_.SetFloat(name, 10);
    }
}
