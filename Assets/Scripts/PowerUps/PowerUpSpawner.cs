using System;
using System.Collections;
using System.Collections.Generic;
using Lean.Pool;
using UnityEngine;
using Random = UnityEngine.Random;

public class PowerUpSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] powerUps;
    [SerializeField] private Transform[] spawnPoints;

    private void Start()
    {
        foreach (Transform spawnPoint in spawnPoints)
        {
            var randomChance = Random.Range(0, 100);
            Debug.Log(randomChance);
            if (randomChance <= GameManager.Instance.chanceToSpawnPowerUpPerTile)
            {
                var randomPowerUp = powerUps[Random.Range(0, powerUps.Length)];
                LeanPool.Spawn(randomPowerUp, spawnPoint.position, Quaternion.identity, spawnPoint);
                break;
            }
        }
    }
}
