using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Lean.Pool;
using UnityEngine;

public class SlowObstacles : MonoBehaviour, IPickupable
{
    
    [SerializeField] private float slowDuration;
    private bool isPickedUp;
    public void PickUp(PlayerController controller)
    {
        if (isPickedUp)
        {
            return;
        }
        StartCoroutine(SlowObstaclesCO(controller));
        transform.DOScale(Vector3.zero, 0.15f);
        isPickedUp = true;
    }

    IEnumerator SlowObstaclesCO(PlayerController controller)
    {
        controller.SetObstacleSpeed(0.3f);
        yield return new WaitForSeconds(slowDuration);     
        controller.SetObstacleSpeed(1f);
        LeanPool.Despawn(gameObject);

    }
       
}
