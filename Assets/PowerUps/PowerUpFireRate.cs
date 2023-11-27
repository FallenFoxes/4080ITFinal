using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PowerUpFireRate : BasePowerUp
{
    public float timeBetweenBullets = .2f;
    protected override bool ApplyToPlayer(Player thePickerUpper) {
        if(thePickerUpper.bulletSpawner.timeBetweenBullets <= timeBetweenBullets)
        {
            return false;
        } else {
           thePickerUpper.bulletSpawner.timeBetweenBullets = timeBetweenBullets;
        return true;
        }
        
    }
}
