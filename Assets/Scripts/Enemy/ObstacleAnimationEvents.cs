using System.Collections;
using System.Collections.Generic;
using Lean.Pool;
using UnityEngine;

public class ObstacleAnimationEvents : MonoBehaviour
{
    [SerializeField] private GameObject slamEffectPrefab;
    [SerializeField] private Transform slamEffectSpawnPoint;

    private void SpawnSlamEffect()
    {
        var slamEffect = LeanPool.Spawn(slamEffectPrefab, slamEffectSpawnPoint.position, Quaternion.Euler(270,0,0));
        LeanPool.Despawn(slamEffect, 3f);
    }
}
