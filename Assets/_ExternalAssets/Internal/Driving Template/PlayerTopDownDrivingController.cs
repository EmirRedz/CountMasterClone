using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace LaynarGames
{
    public class PlayerTopDownDrivingController : MonoBehaviour
    {
        [Header("Movement")]
        public Joystick joystick;
        public float targetSpeed;
        float currentSpeed;
        public float accelerationSpeed;

        float currentSmoothRotationSpeed;
        public float smoothRotationSpeed = 0.1f;
        public Rigidbody rb;

        [Header("Animations")]
        public Animator playerAnimator;
        public Animator vehicleAnimator;

        public bool canDrive = true;

        [HideInInspector]
        public float steeringAngle;
        [HideInInspector]
        public float forwardVelocity;

        [SerializeField] float rotationAccelerationSpeed;

        enum TurningDirection { Clockwise,CounterClockwise,Straight};
        TurningDirection lastTurningDirection = TurningDirection.Straight;
        
        [Header("Ground Check")]
        private bool grounded;
        [SerializeField] private float distToGround;
        [SerializeField] private float gravity;
        [SerializeField] private LayerMask ground;
        
        private void FixedUpdate()
        {
            if (!canDrive) { return; }

            if (vehicleAnimator != null) { SetFloatAnim(0);}
            
            float magnitude = new Vector2(joystick.Horizontal, joystick.Vertical).magnitude;

            if(magnitude != 0)
            {
                currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * accelerationSpeed);

                Vector3 newVelocity = rb.transform.forward * (currentSpeed * magnitude);

                if (grounded)
                {
                    newVelocity.y = rb.velocity.y;
                }
                else
                {
                    newVelocity.y = gravity;
                }
                rb.velocity = newVelocity;

                //Get the difference between the two directions, the current facing direction of the car and the joystick direction

                Vector3 newDirection = new Vector3(joystick.Horizontal, 0, joystick.Vertical);
                Vector3 currentDirection = rb.transform.forward;
                float angle = Vector3.SignedAngle(newDirection, currentDirection, Vector3.up);
                float lastAngle = steeringAngle;

                steeringAngle = angle;

               
                RotateInDirection();
            }

            forwardVelocity = rb.velocity.magnitude;

            SetFloatAnim(rb.velocity.magnitude);

            GroundCheck();
            UpdateState();

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
        
        public bool GroundCheck()
        {
            grounded = Physics.Raycast(transform.position, -Vector3.up, distToGround , ground);
            return grounded;
        }

        void RotateInDirection()
        {
            currentSmoothRotationSpeed = Mathf.Lerp(currentSmoothRotationSpeed, smoothRotationSpeed, Time.deltaTime * rotationAccelerationSpeed);

            Vector3 targetPos = rb.transform.position + (new Vector3(joystick.Horizontal, 0, joystick.Vertical));
            Vector3 targetDirection = targetPos - rb.transform.position;

            //Get proper turningDirection
            Vector3 newRotDirection = Vector3.Lerp(rb.transform.forward, Vector3.RotateTowards(rb.transform.forward, targetDirection, 180, 0), Time.deltaTime * currentSmoothRotationSpeed);

            TurningDirection newTurningDirection = lastTurningDirection;

            newTurningDirection = GetRotateDirection(rb.transform.rotation, Quaternion.LookRotation(newRotDirection));

            //Set rotation
            rb.transform.rotation = Quaternion.LookRotation(newRotDirection);

            //Now accelerate rotation

            switch (newTurningDirection)
            {
                case TurningDirection.Clockwise:
                    if (lastTurningDirection == TurningDirection.CounterClockwise)
                    {
                        currentSmoothRotationSpeed = 0;
                    }
                    break;
                case TurningDirection.CounterClockwise:
                    if (lastTurningDirection == TurningDirection.Clockwise)
                    {
                        currentSmoothRotationSpeed = 0;
                    }
                    break;
                default:
                    break;
            }

            lastTurningDirection = newTurningDirection;

        }

        TurningDirection GetRotateDirection(Quaternion from, Quaternion to)
        {
            float fromY = from.eulerAngles.y;
            float toY = to.eulerAngles.y;
            float clockWise = 0f;
            float counterClockWise = 0f;

            if (fromY <= toY)
            {
                clockWise = toY - fromY;
                counterClockWise = fromY + (360 - toY);
            }
            else
            {
                clockWise = (360 - fromY) + toY;
                counterClockWise = fromY - toY;
            }

            if(clockWise <= counterClockWise)
            {
                return TurningDirection.Clockwise;
            }
            else
            {
                return TurningDirection.CounterClockwise;
            }
        }

        void SetFloatAnim( float value)
        {
            if(playerAnimator != null)
            {
                playerAnimator.SetFloat("Velocity", Mathf.Round(value * 10) / 10);
            }

            if (vehicleAnimator != null)
            {
                vehicleAnimator.SetFloat("Velocity", Mathf.Round(value * 10) / 10);
            }
        }

        public void SetCanWalk()
        {
            canDrive = true;
        }

        public void SetCantWalk()
        {
            canDrive = false;

            if (playerAnimator != null) { SetFloatAnim(0);}

            Vector3 newVelocity = Vector3.zero;
            newVelocity.y = rb.velocity.y;
            rb.velocity = newVelocity;

            forwardVelocity = rb.velocity.magnitude;

            steeringAngle = 0;

        }

    }

}
