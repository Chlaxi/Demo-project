using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : Pickup
{
    [SerializeField] private int gems = 10;
    public override bool UseItem(PlayerController player)
    {
        GameManager.instance.AddGems(gems);
        return true;
    }

}
