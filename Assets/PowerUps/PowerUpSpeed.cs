using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PowerUpSpeed : BasePowerUp
{
    public float speedBoostAmount = 75f;
    protected override bool ApplyToPlayer(Player thePickerUpper)
    {
        if (thePickerUpper.movementSpeed <= speedBoostAmount) {
            return false;
        } else {
            thePickerUpper.movementSpeed = speedBoostAmount;
            return true;
        }
    }

}
