using UnityEngine;
using System.Collections;
using System;

[RequireComponent (typeof(Rigidbody))]
public class PhysicsFollower : MonoBehaviour 
{
	public Transform target;

	[NonSerialized] public Rigidbody rb;

	[Range(0f,1f)] public float positionStrength = 1f;
	[Range(0f,1f)] public float rotationStrength = 1f;

	void Awake () 
	{
		rb = GetComponent<Rigidbody>();
		rb.useGravity = false;
		rb.maxAngularVelocity = 30f; //set it to something pretty high so it can actually follow properly!
	}
	
	void FixedUpdate () 
	{
		if(target == null) return;

		Vector3 deltaPos = target.position - transform.position;
		rb.velocity = 1f/Time.fixedDeltaTime * deltaPos * Mathf.Pow(positionStrength, 90f*Time.fixedDeltaTime);

		Quaternion deltaRot = target.rotation * Quaternion.Inverse(transform.rotation);

		float angle;
		Vector3 axis;

		deltaRot.ToAngleAxis(out angle, out axis);

		if (angle > 180.0f) angle -= 360.0f;

		if (angle != 0) rb.angularVelocity = (1f/Time.fixedDeltaTime * angle * axis * 0.01745329251994f * Mathf.Pow(rotationStrength, 90f*Time.fixedDeltaTime));
	}
}