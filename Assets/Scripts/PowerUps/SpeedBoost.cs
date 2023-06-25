using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Lean.Pool;
using UnityEngine;

public class SpeedBoost : MonoBehaviour, IPickupable
{
    [SerializeField] private float powerUpDuration;
    [SerializeField] private float speedFactor;
    private bool isPickedUp;
    public void PickUp(PlayerController controller)
    {
        if (isPickedUp)
        {
            return;
        }

        StartCoroutine(SetSpeed(controller));
        isPickedUp = true;
        transform.DOScale(Vector3.zero, 0.15f);
    }

    IEnumerator SetSpeed(PlayerController controller)
    {
        controller.SetPlayerSpeed(controller.NormalSpeed() * speedFactor);
        yield return new WaitForSeconds(powerUpDuration);
        controller.SetPlayerSpeed(controller.NormalSpeed());
        LeanPool.Despawn(gameObject);

    }
}
