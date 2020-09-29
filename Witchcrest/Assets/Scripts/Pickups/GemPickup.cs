using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemPickup : Pickup
{
    public override bool UseItem(PlayerController player)
    {
        GameManager.instance.AddGems(1);
        return true;
    }
}
