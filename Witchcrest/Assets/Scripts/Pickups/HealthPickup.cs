using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : Pickup
{
    [SerializeField] private int healing;

    public override bool UseItem(PlayerController player)
    {
        return(player.Heal(healing));
    }
}
