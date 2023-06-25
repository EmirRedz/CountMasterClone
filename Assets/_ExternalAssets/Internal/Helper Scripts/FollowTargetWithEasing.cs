using System.Collections;
using System.Collections.Generic;

using Lean.Pool;
using UnityEngine;
using UnityEngine.Events;
using LaynarGames.IdleArcadeTemplate;

namespace LaynarGames
{
    public class FollowTargetWithEasing : MonoBehaviour
    {
        [SerializeField] Transform target;
        [SerializeField] bool raiseEventOnArrive;
        [SerializeField] float distanceToArrive;
        [SerializeField] UnityEvent eventToRaiseOnArrive;
        [SerializeField] float smoothSpeed;
        bool eventAlreadyInvoked;
        [SerializeField] bool despawnOnArrive;
        [SerializeField] bool despawnParentInsteadOfThis;
        public bool isFollowing;
        [SerializeField] float delayToStartFollowing;
        [SerializeField] TrailRenderer trail;
        float maxSpeed;

        [Header("Particles")]
        [SerializeField] GameObject particlesOnDespawn;

        [SerializeField] Animator anim;
        public bool canPickUp;

        [SerializeField] Rigidbody rb;

        private void OnEnable()
        {
            maxSpeed = smoothSpeed;
            StartCoroutine(DelayToStartFollowing());
        }


        IEnumerator DelayToStartFollowing()
        {
            yield return new WaitForSeconds(delayToStartFollowing);
            canPickUp = true;
            rb.isKinematic = true;

        }

        public void SetTarget(Transform newTarget)
        {
           // anim.enabled = false;

            isFollowing = true;
            target = newTarget;

            trail.enabled = true;
        }

        bool beingPicked;

        float distCheckTimer;
        float distCheckTimestep = .03f;

        // Update is called once per frame
        void Update()
        {
            if (canPickUp == false)
            {
                return;}
            if (target == null) { return; }
            if (isFollowing == false) { return; }

            smoothSpeed += Time.deltaTime;

            transform.position = Vector3.Lerp(transform.position, target.transform.position, Time.deltaTime * smoothSpeed);

            if (!raiseEventOnArrive || eventAlreadyInvoked) { return; }

            if (Vector3.Distance(transform.position, target.position) < distanceToArrive)
            {
                eventAlreadyInvoked = true;

                if (eventToRaiseOnArrive != null)
                {
                    eventToRaiseOnArrive.Invoke();
                }

                if (despawnOnArrive)
                {
                    Despawn();
                }
            }
        }

        void Despawn()
        {
            beingPicked = false;

            smoothSpeed = maxSpeed;

            trail.enabled = false;
            isFollowing = false;
            eventAlreadyInvoked = false;
            canPickUp = false;

            target = null;

            GameObject particles = LeanPool.Spawn(particlesOnDespawn, transform.position, Quaternion.identity, null);
            LeanPool.Despawn(particles, 2);
            rb.isKinematic = false;

            if (despawnParentInsteadOfThis)
            {
                LeanPool.Despawn(transform.parent);
            } else
            {
                LeanPool.Despawn(gameObject);
            }
        }
    }
}

