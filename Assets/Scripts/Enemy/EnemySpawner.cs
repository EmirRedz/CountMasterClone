using System;
using System.Collections;
using System.Collections.Generic;
using Lean.Pool;
using TMPro;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    public List<MinionController> playerMinionsInArea;
    private List<EnemyController> myEnemies = new List<EnemyController>();
    [SerializeField] private GameObject myTile;
    [SerializeField] private EnemyController enemyPrefab;
    [SerializeField] [Range(0,1)] private float angle, radius;
    [SerializeField] private TMP_Text amountText;
    
    public void Init(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            SpawnEnemy();
        }
    }
    public void SpawnEnemy()
    {
        var minion = LeanPool.Spawn(enemyPrefab, transform.position, Quaternion.identity, transform);
        minion.Init(this);

        AddEnemyToList(minion);
        minion.transform.position = GetEnemySpawnPosition(myEnemies.IndexOf(minion));
        minion.transform.localEulerAngles = Vector3.zero;
    }
    
    private Vector3 GetEnemySpawnPosition(int index)
    {
        float goldenAngle = 137.5f * angle;  

        float x = radius * Mathf.Sqrt(index + 1) * Mathf.Cos(Mathf.Deg2Rad * goldenAngle * (index + 1));
        float z = radius * Mathf.Sqrt(index + 1) * Mathf.Sin(Mathf.Deg2Rad * goldenAngle * (index + 1));

        Vector3 localMinionPosition = new Vector3(x, 0, z);
        Vector3 minionTargetPosition = transform.TransformPoint(localMinionPosition);

        return minionTargetPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Minion"))
        {
            AddMinions(other.GetComponent<MinionController>());
        }
    }

    private void AddMinions(MinionController minionToAdd)
    {
        if (playerMinionsInArea.Contains(minionToAdd))
        {
            return;
        }

        playerMinionsInArea.Add(minionToAdd);
    }
    
    public void RemoveMinion(MinionController minionToRemove)
    {
        if (!playerMinionsInArea.Contains(minionToRemove))
        {
            return;
        }

        playerMinionsInArea.Remove(minionToRemove);
    }
    
    private void AddEnemyToList(EnemyController  enemyToAdd)
    {
        if (myEnemies.Contains(enemyToAdd))
        {
            return;
        }
        myEnemies.Add(enemyToAdd);
        amountText.SetText(myEnemies.Count.ToString());
    }
    
    public void RemoveEnemyToList(EnemyController  minionToAdd)
    {
        if (!myEnemies.Contains(minionToAdd))
        {
            return;
        }

        myEnemies.Remove(minionToAdd);

        if (myEnemies.Count <=0)
        {
            myTile.tag = "Untagged";
            GameManager.Instance.playerController.SetPlayerSpeed(false);
            amountText.gameObject.SetActive(false);
        }
        
        amountText.SetText(myEnemies.Count.ToString());

    }
}
