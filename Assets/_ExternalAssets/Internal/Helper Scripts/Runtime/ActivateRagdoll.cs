using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateRagdoll : MonoBehaviour
{
    public Rigidbody[] rigidbodies;
    public Collider[] colls;
    public Animator anim;

    [Header("Force To Give Ragdoll")] 
    public bool activated;
    public Vector3 forceToGive;
    public Rigidbody rbToAddForce;
    public float maxVelocity;
    public float rbMass;

    Transform makeForceRelativeToThisTransform;
    Transform originalParent;

    [SerializeField] PhysicMaterial physicsMat;
    [SerializeField] float throwPowerMultiplier;

    private void Awake()
    {
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        colls = GetComponentsInChildren<Collider>();
        anim = GetComponent<Animator>();

        originalParent = transform.parent;

        makeForceRelativeToThisTransform = GameObject.FindGameObjectWithTag("Ending").transform;
    }

    [ContextMenu("Activate")]
    public void Activate()
    {
        transform.parent = null;    
        anim.enabled = false;
        Debug.Log("Activate Ragdoll");

        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = false;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }

        foreach (Collider collider in colls)
        {
            collider.isTrigger = false;
        }

        activated = true;
    }

    [ContextMenu("DeActivate")]
    public void Deactivate()
    {
        transform.parent = originalParent;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);

        Debug.Log("Deactivate");
        anim.enabled = true;
        Debug.Log("Deactivate Ragdoll");

        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = true;
        }

        foreach (Collider collider in colls)
        {
            collider.isTrigger = true;
        }

        activated = false;
    }
    

    [ContextMenu("Set Ragdoll")]
    public void SetRagdollEditor()
    {
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        colls = GetComponentsInChildren<Collider>();
        
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.mass = rbMass;
        }
    }

    [ContextMenu("Set Physics Material")]
    public void SetPhysicsMaterialEditor()
    {
        foreach(Collider coll in colls)
        {
            coll.material = physicsMat;
        }
    }

    public void ApplyForceToRagdoll()
    {
        float throwPower = throwPowerMultiplier;

        rbToAddForce.AddForce(forceToGive.z * transform.forward * throwPower, ForceMode.Impulse);
        rbToAddForce.AddForce(forceToGive.y * transform.up * throwPower, ForceMode.Impulse);
    }

    public void WaitAndApplyForce()
    {

        StartCoroutine(WaitToAddForce());
    }

    IEnumerator WaitToAddForce()
    {
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.velocity = Vector3.zero;
        }

        yield return new WaitForSeconds(.05f);


        if (makeForceRelativeToThisTransform == null)
        {
            ApplyForceToRagdoll();
        } else
        {
            transform.rotation = makeForceRelativeToThisTransform.rotation;
            yield return new WaitForSeconds(.05f);

            rbToAddForce.AddForce(forceToGive.x * makeForceRelativeToThisTransform.right, ForceMode.Impulse);
            rbToAddForce.AddForce(forceToGive.z * makeForceRelativeToThisTransform.forward, ForceMode.Impulse);
            rbToAddForce.AddForce(forceToGive.y * makeForceRelativeToThisTransform.up, ForceMode.Impulse);
        }
    }

    private void FixedUpdate()
    {
        if (activated)
        {
            rbToAddForce.velocity = Vector3.ClampMagnitude(rbToAddForce.velocity, maxVelocity);
        }
    }
}
