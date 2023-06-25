using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class PlayerTopDownController : MonoBehaviour
{
    public static PlayerTopDownController instance;
    
    [Header("Movement")]
    public Joystick joystick;
    public float speed;
    private static readonly int Velocity = Animator.StringToHash("Velocity");
    public float smoothRotationSpeed = 0.1f;
    public Rigidbody rb;

    [Header("Animations")]
    public Animator animator;

    public bool canWalk = true;
    
    [Header("Ground Check")]
    private bool grounded;
    [SerializeField] private float distToGround;
    [SerializeField] private float gravity;
    [SerializeField] private LayerMask ground;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void FixedUpdate()
    {
        if (!canWalk)
        {
            return;
        }

       
        rb.velocity = Vector3.zero;
        if(animator != null){ SetFloatAnim(Velocity, 0);}

        transform.forward = Vector3.Lerp(transform.forward,
              new Vector3(joystick.Horizontal, 0, joystick.Vertical), smoothRotationSpeed);

        float magnitude = new Vector2(joystick.Horizontal, joystick.Vertical).magnitude;

        Vector3 newVelocity = transform.forward * (speed * magnitude);

        if (grounded)
        {
            newVelocity.y = rb.velocity.y;
        }
        else
        {
            newVelocity.y = gravity;
        }
        rb.velocity = newVelocity;

        Vector2 xZVelocity = new Vector2(newVelocity.x, newVelocity.z);
        if (animator != null) { SetFloatAnim(Velocity, xZVelocity.magnitude);}

        GroundCheck();
        UpdateState();
    }

    public bool GroundCheck()
    {
        grounded = Physics.Raycast(transform.position, -Vector3.up, distToGround , ground);
        return grounded;
    }
    void SetFloatAnim(int id, float value)
    {
        animator.SetFloat(id, value);
    }

    public void SetCanWalk()
    {
        canWalk = true;
    }

    public void SetCantWalk()
    {
        canWalk = false;

        if (animator != null) { SetFloatAnim(Velocity, 0); }

        Vector3 newVelocity = Vector3.zero;
        newVelocity.y = rb.velocity.y;
        rb.velocity = newVelocity;

        animator.SetFloat(Velocity, 0);
    }
    
    int stepsSinceLastGrounded;
    [SerializeField, Range(0f, 100f)]
    float maxSnapSpeed = 100f;
    private int groundContactCount;
    private Vector3 contactNormal;
    private float minGroundDotProduct;
    
    void UpdateState () {
        stepsSinceLastGrounded += 1;

        if (grounded || SnapToGround()) {
            stepsSinceLastGrounded = 0;

            if (groundContactCount > 1) {
                contactNormal.Normalize();
            }
        }
        else {
            contactNormal = Vector3.up;
        }
    }
    
    bool SnapToGround () {
        if (stepsSinceLastGrounded > 1) {
            return false;
        }
        float speed = rb.velocity.magnitude;
        if (speed > maxSnapSpeed) {
            return false;
        }
        if (!Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit)) {
            return false;
        }
        if (hit.normal.y < minGroundDotProduct) {
            return false;
        }

        groundContactCount = 1;
        contactNormal = hit.normal;
        //float speed = velocity.magnitude;
        float dot = Vector3.Dot(rb.velocity, hit.normal);
        if (dot > 0f) {
            rb.velocity = (rb.velocity - hit.normal * dot).normalized * speed;
        }
        return true;
    }
}
