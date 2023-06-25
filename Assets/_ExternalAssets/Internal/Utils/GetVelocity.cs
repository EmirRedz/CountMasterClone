using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetVelocity : MonoBehaviour
{
    Vector3 previous;
    [SerializeField] Animator anim;
    [SerializeField]  float smoothSpeed = 1;
    private void Start()
    {
        previous = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 velocity = (transform.position - previous) / Time.deltaTime;
        previous = transform.position;

        float xVel = transform.InverseTransformDirection(velocity).x;

        float smoothVel = Mathf.Lerp(anim.GetFloat("Xvel"), xVel, Time.deltaTime * smoothSpeed);
        anim.SetFloat("Xvel", smoothVel);
    }
}
