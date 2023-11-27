using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Arena1Game : NetworkBehaviour
{
    public Player clientPrefab;
    public Player playerHost;
    public Camera arenaCamera;

    private int positionIndex = 0;
    private Vector3[] startPositions = new Vector3[]
    {
        new Vector3(2, 2, 0),
        new Vector3(-2, 2, 0),
        new Vector3(0, 2, 4),
        new Vector3(0, 2, -4)
    };

    void Start()
    {
        arenaCamera.enabled = !IsClient;
        arenaCamera.GetComponent<AudioListener>().enabled = !IsClient;
        if (IsServer)
        {
            SpawnPlayers();
        }
    }
        
    private Vector3 NextPosition()
    {
        Vector3 pos = startPositions[positionIndex];
        positionIndex += 1;
        if (positionIndex > startPositions.Length - 1)
        {
            positionIndex = 0;
        }
        return pos;
    }

    private void SpawnPlayers()
    {
        foreach (ulong clientId in NetworkManager.ConnectedClientsIds)
        {
            Player playerPrefabToSpawn = clientPrefab;
            if (NetworkManager.LocalClientId == clientId)
            {
                playerPrefabToSpawn = playerHost;
            }

                Player playerSpawn = Instantiate(
                    playerPrefabToSpawn, 
                    NextPosition(), 
                    Quaternion.identity);
            playerSpawn.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
            //playerSpawn.PlayerColor.Value = NextColor();
        }
    }
}