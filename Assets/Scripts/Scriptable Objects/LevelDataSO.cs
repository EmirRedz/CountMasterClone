using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level Date", menuName = "Scriptable Object/Level Data")]
public class LevelDataSO : ScriptableObject
{
    [Header("Tiles")]
    public List<GameObject> tilesToSpawn;
    [Header("Gates")]
    public GateDataContainer[] GateDatasContainers;
    
    [Header("Enemies")]
    public int[] enemyAmounts;
    
    [Header("Finish")]
    public FinishManager.FinishType finishType;
    
    public int rewardAmount;
    public int amountOfStairsToSpawn;
    
    [Space]
    public BossController bossToSpawn;
    public int bossHealth;
}

[System.Serializable]
public class GateData
{
    public Gate.GateType gateType;
    public int value;
}

[System.Serializable]
public class GateDataContainer
{
    public List<GateData> gateDatas;
}
