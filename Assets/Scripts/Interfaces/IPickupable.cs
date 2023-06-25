using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPickupable
{
    public void PickUp(PlayerController controller);
    public enum PickUpType
    {
        SpeedBoost,
        SlowDownTime,
        Shield
    }
}
