using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Lean.Pool;
using UnityEngine;

public class ProtectionShield : MonoBehaviour, IPickupable
{
    [SerializeField] private float shieldDuration;
    private bool isPickedUp;
    public void PickUp(PlayerController controller)
    {
        if (isPickedUp)
        {
            return;
        }
        StartCoroutine(ActivateShield(controller));
        transform.DOScale(Vector3.zero, 0.15f);
        isPickedUp = true;
    }

    IEnumerator ActivateShield(PlayerController controller)
    {
        controller.SetProtection(true);
        yield return new WaitForSeconds(shieldDuration);
        controller.SetProtection(false);
        LeanPool.Despawn(gameObject);

    }
}
