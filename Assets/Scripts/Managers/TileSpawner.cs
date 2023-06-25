using System;
using System.Collections;
using System.Collections.Generic;
using GameAnalyticsSDK;
using Lean.Pool;
using UnityEngine;

public class TileSpawner : MonoBehaviour
{
    private float zPosition;
    [SerializeField] private float tileLength;

    private void Awake()
    {
        int currentLevelIndex = 0;
        int currentGateDataIndex = 0;
        int currentEnemyGateIndex = 0;
        for (int i = 0; i < GameManager.Instance.levels[0].tilesToSpawn.Count; i++)
        {
            var tile = SpawnTile(GameManager.Instance.levels[0].tilesToSpawn[i]);

            if (tile.CompareTag("GateTile"))
            {
                tile.GetComponentInChildren<GateHolder>().InitGates(GameManager.Instance.levels[currentLevelIndex].GateDatasContainers[currentGateDataIndex].gateDatas);
                currentGateDataIndex++;
            }

            if (tile.CompareTag("EnemyTile"))
            {
                tile.GetComponentInChildren<EnemySpawner>().Init(GameManager.Instance.levels[currentLevelIndex].enemyAmounts[currentEnemyGateIndex]);
                currentEnemyGateIndex++;
            }

            if (tile.CompareTag("FinishTile"))
            {
                var finishManager = tile.GetComponentInChildren<FinishManager>();
                GameManager.Instance.SetFinishLine(finishManager);
                finishManager.SetUp(GameManager.Instance.levels[currentLevelIndex].finishType, 
                    GameManager.Instance.levels[currentLevelIndex].amountOfStairsToSpawn);
            }
        }
        
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, GameAnalyticsDataManager.LEVEL_PROGRESS);
    }

    private GameObject SpawnTile(GameObject tileToSpawn)
    {
        var newTile = LeanPool.Spawn(tileToSpawn, transform.forward * zPosition, Quaternion.identity);
        zPosition += tileLength;

        newTile.transform.SetParent(transform);

        return newTile;
    }
}
