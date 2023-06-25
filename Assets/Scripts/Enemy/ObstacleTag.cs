using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleTag : MonoBehaviour
{
    public static Action<Animator> RegisterObstacleAnimator;

    private void Start()
    {
        RegisterObstacleAnimator?.Invoke(GetComponent<Animator>());
    }
}
