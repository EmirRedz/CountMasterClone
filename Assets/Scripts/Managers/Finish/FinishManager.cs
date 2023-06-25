using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Lean.Pool;
using UnityEngine;

public class FinishManager : MonoBehaviour
{
    [SerializeField] private FinishType finishType;
    [SerializeField] private Transform[] confettiSpawnPoints;
    
    [Header("Stairs")] 
    [SerializeField] private int amountOfStairsToSpawn;
    [SerializeField] private StairController stairPrefab;
    [SerializeField] private StairController finalStair;
    [SerializeField] private Transform stairSpawnPoint;
    [SerializeField] private float zFactor;
    [SerializeField] private float yFactor;
    [SerializeField] private CinemachineVirtualCamera stairsCamera;
    [SerializeField] private Material[] stairMaterials;

    [Header("Boss")]
    [SerializeField] private GameObject bossArena;
    [SerializeField] private Transform bossSpawnPoint;
    [SerializeField] private CinemachineVirtualCamera bossCamera;
    private BossController boss;
    private void Start()
    {
        Init();
    }

    private void Init()
    {
        switch (finishType)
        {
            case FinishType.Stairs:
                SpawnStairs();
                break;
            case FinishType.Boss:
                SetUpBossFight();
                break;
        }
       
    }

    private void SetUpBossFight()
    {
        bossArena.SetActive(true);
        boss = LeanPool.Spawn(GameManager.Instance.GetCurrentLevelData().bossToSpawn, bossSpawnPoint.position,
            bossSpawnPoint.rotation, bossSpawnPoint);
        boss.InitBoss(GameManager.Instance.GetCurrentLevelData().bossHealth);
    }
    private void SpawnStairs()
    {
        int currentMaterialIndex = 0;
        float currentPoint = GameManager.Instance.startingPointValue;
        for (int i = 0; i < amountOfStairsToSpawn; i++)
        {
            var prefab = i == amountOfStairsToSpawn - 1 ? finalStair : stairPrefab;
            var stair = LeanPool.Spawn(prefab, stairSpawnPoint);
            var stairTargetPosition = new Vector3(0, yFactor * i, zFactor * i);
            stair.transform.localPosition = stairTargetPosition;

            stair.InitStairs(stairMaterials[currentMaterialIndex], currentPoint);
            
            currentMaterialIndex++;
            if (currentMaterialIndex > stairMaterials.Length - 1)
            {
                currentMaterialIndex = 0;
            }

            currentPoint += GameManager.Instance.pointValueToIncrement;
        }
    }

    public void TriggerFinish(PlayerController controller)
    {
        switch (finishType)
        {
            case FinishType.Stairs:
                controller.StackMinions(stairsCamera.gameObject);
                stairsCamera.m_Follow = controller.MyMinions()[controller.MyMinions().Count - 1].transform;
                break;
            case FinishType.Boss:
                controller.FightBoss(boss);
                boss.StartFighting();
                bossCamera.gameObject.SetActive(true);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        foreach (Transform confettiSpawnPoint in confettiSpawnPoints)
        {
            GameManager.Instance.PlayConfetti(confettiSpawnPoint.position, confettiSpawnPoint.eulerAngles);
        }
    }

    public void SetUp(FinishType _type, int stairsAmount)
    {
        finishType = _type;
        amountOfStairsToSpawn = stairsAmount;
    }
    
    public enum FinishType
    {
        Stairs,
        Boss
    }
}
