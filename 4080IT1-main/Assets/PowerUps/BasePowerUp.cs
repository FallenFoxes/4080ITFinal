using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BasePowerUp : NetworkBehaviour
{
    public void ServerPickUp(Player thePickerUpper) {
        if(IsServer)
        {
            GetComponent<NetworkObject>().Despawn();
        }
    }
}
 