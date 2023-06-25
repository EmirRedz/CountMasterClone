using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetUpPowerUps : MonoBehaviour
{
    [SerializeField]private Vector2Int xRange;
    [SerializeField]private Vector2Int zRange;
    

    [ContextMenu("Set Up Power Points")]
    private void SetUp()
    {
        for (int i = xRange.y; i < xRange.x; i++)
        {
            for (int j = zRange.x; j < zRange.y; j++)
            {
                var position = new Vector3(i, 0, j);
                GameObject spawnPointGO = new GameObject();
                var spawnPoint = Instantiate(spawnPointGO);
                spawnPoint.transform.SetParent(transform);
                spawnPoint.transform.localPosition = position;
                spawnPoint.name = "Power UP Spawn Point";
            }
        }
    }
    
    
}
