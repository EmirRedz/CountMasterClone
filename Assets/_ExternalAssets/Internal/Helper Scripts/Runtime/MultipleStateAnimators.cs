using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleStateAnimators : MonoBehaviour
{
    Animator[] animators;
    [SerializeField] private bool waitToGetAnimators;

    private void Start()
    {
        if (waitToGetAnimators)
        {
            StartCoroutine(GetAnimatorsAfterTime());
            return;
        } 
        animators = GetComponentsInChildren<Animator>();
    }

    IEnumerator GetAnimatorsAfterTime()
    {
        yield return new WaitForSeconds(.2f);
        animators = GetComponentsInChildren<Animator>();
    }

    public void GetAnimators()
    {
        animators = new Animator[0];
        StartCoroutine(GetAnimatorsAfterTime());
    }

    public void SetBoolTrue(string name)
    {
        if(animators == null) { return; };

        foreach (Animator anim in animators)
        {
            anim.SetBool(name, true);
        }
    } 
 
    public void SetBoolFalse(string name)
    {
        if (animators == null) { return; };

        foreach (Animator anim in animators)
        {
            anim.SetBool(name, false);
        }
    }
    
    public void SetTrigger(string name)
    {
        if (animators == null) { return; };

        foreach (Animator anim in animators)
        {
            anim.SetTrigger(name);
        }
    }

    public void ResetTrigger(string name)
    {
        if (animators == null) { return; };

        foreach (Animator anim in animators)
        {
            anim.ResetTrigger(name);
        }
    }

    public void SetInteger(string name, int value)
    {
        if (animators == null) { return; };

        foreach (Animator anim in animators)
        {
            anim.SetInteger(name, value);
        }

    }

    public void SetIsRootEnabled(bool var)
    {
        if (animators == null) { return; };

        foreach (Animator anim in animators)
        {
            anim.applyRootMotion = var;
        }
    }

    public void SetFloat(int id, float value)
    {
        foreach (Animator anim in animators)
        {
            anim.SetFloat(id, value);
        }
    }

    public void SetAnimatorGameObjectLocalPosAndRotToZero()
    {
        foreach (Animator anim in animators)
        {
            anim.transform.localPosition = Vector3.zero;
            anim.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
