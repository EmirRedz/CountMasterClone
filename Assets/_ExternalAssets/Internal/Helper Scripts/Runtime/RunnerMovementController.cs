using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;


public class RunnerMovementController : MonoBehaviour
{
    [Header("Spline Follower Set Up")]
    [SerializeField] private SplineFollower splineFollower;
    [SerializeField] private SplinePositioner playerSplinePositioner;

    [Header("Spline Follower Behaviors")]
    [SerializeField] private float splineFollowerOffsetLerpSpeed;
    [SerializeField] private float furthestLeftOffset;
    [SerializeField] private float furthestRightOffset;

    [Header("Input")]
    [SerializeField] private float horizontalInputSensitivity;

    [Header("Player Set Up")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform playerArtTransform;
    [SerializeField] private float playerTargetLerpSpeed;
    [SerializeField] private float playerTransformLerpSpeed;

    [Header("Player Rotation")]
    [SerializeField] private bool rotatePlayerTowardsDirection;
    [SerializeField] private float rotationDeltaXDeadZone;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float rotationSnapBackSpeed;
    [SerializeField] private float minRotationY;
    [SerializeField] private float maxRotationY;

    private Vector3 initialMousePos;
    private Vector3 lastMousePos;
    private bool mouseDown;


    public void SetPlayerToRun()
    {
        //Set player target position
        playerSplinePositioner.SetPercent(splineFollower.result.percent);
        playerSplinePositioner.transform.rotation = splineFollower.transform.rotation;

        StartCoroutine(ResetReady());
    }

    bool readyToFollow;

    IEnumerator ResetReady()
    {
        yield return new WaitForSeconds(.1f);
        readyToFollow = true;
    }

    private void Update()
    {
        Vector3 currentMousePos = Input.mousePosition;
        float deltaX = currentMousePos.x - lastMousePos.x;

        if (!mouseDown)
        {
            deltaX = 0;
        }

        if (readyToFollow)
        {
            SetPlayerTargetPosition();

            SetPlayerPosition();
        }

        ControlMovement(deltaX);


        if (rotatePlayerTowardsDirection)
        {
            ControlRotation(deltaX);
        }

        lastMousePos = currentMousePos;
    }

    void ControlMovement(float deltaX)
    {
        if (Input.GetMouseButtonDown(0))
        {
            mouseDown = true;
            initialMousePos = Input.mousePosition;
            lastMousePos = initialMousePos;
        }

        if (Input.GetMouseButtonUp(0))
        {
            mouseDown = false;
        }

        if (!mouseDown)
        {
            ResetSplineFollowerOffset();
            return;
        }

        SetSplineFollowerOffset(deltaX);
    }

    void SetSplineFollowerOffset(float deltaX)
    {
        Vector3 targetOffset = new Vector3(splineFollower.motion.offset.x + (deltaX * horizontalInputSensitivity), 0, 0);

        if (targetOffset.x > furthestRightOffset)
        {
            targetOffset.x = furthestRightOffset;
        }
        if (targetOffset.x < furthestLeftOffset)
        {
            targetOffset.x = furthestLeftOffset;
        }

        splineFollower.motion.offset =
           Vector3.MoveTowards(splineFollower.motion.offset, targetOffset, Time.deltaTime * splineFollowerOffsetLerpSpeed);
    }

    void ControlRotation(float deltaX)
    {
        Vector3 rot = playerArtTransform.localEulerAngles + new Vector3(0, deltaX * rotationSpeed, 0f); //use local if your char is not always oriented Vector3.up
        rot.y = ClampAngle(rot.y, minRotationY, maxRotationY);

        if (Mathf.Abs(deltaX) < rotationDeltaXDeadZone) //snap back to facing forwards
        {
            Quaternion targetRot = Quaternion.Euler(new Vector3(0, 0, 0));
            playerArtTransform.localRotation =
               Quaternion.RotateTowards(playerArtTransform.localRotation, targetRot, Time.deltaTime * rotationSnapBackSpeed);
            return;
        }

        playerArtTransform.localEulerAngles = rot;
    }

    float ClampAngle(float angle, float from, float to)
    {
        // accepts e.g. -80, 80
        if (angle < 0f) angle = 360 + angle;
        if (angle > 180f) return Mathf.Max(angle, 360 + from);
        return Mathf.Min(angle, to);
    }

    void SetPlayerTargetPosition()
    {
        //Set player target position
        playerSplinePositioner.SetPercent(Mathf.Lerp((float)playerSplinePositioner.result.percent, (float)splineFollower.result.percent, Time.deltaTime * 5));
        playerSplinePositioner.transform.rotation = splineFollower.transform.rotation;

        Vector3 motionOffset = playerSplinePositioner.motion.offset;
        motionOffset.x = Mathf.MoveTowards(playerSplinePositioner.motion.offset.x, splineFollower.motion.offset.x,
           Time.deltaTime * playerTargetLerpSpeed);

        playerSplinePositioner.motion.offset = motionOffset;
    }

    void SetPlayerPosition()
    {
        //Set player position
        playerTransform.transform.position = Vector3.Lerp(playerTransform.transform.position,
           playerSplinePositioner.transform.position, Time.deltaTime * playerTransformLerpSpeed);
        playerTransform.transform.rotation = Quaternion.Lerp(playerTransform.rotation,
           playerSplinePositioner.transform.rotation, Time.deltaTime * playerTransformLerpSpeed);
    }

    void ResetSplineFollowerOffset()
    {
        Vector3 newMotionOffset = playerSplinePositioner.motion.offset;
        newMotionOffset.x = Mathf.MoveTowards(splineFollower.motion.offset.x, playerSplinePositioner.motion.offset.x,
           Time.deltaTime * splineFollowerOffsetLerpSpeed);
        splineFollower.motion.offset = newMotionOffset;
    }
}
