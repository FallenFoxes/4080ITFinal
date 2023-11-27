using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Runtime.CompilerServices;

public class Lobby : NetworkBehaviour
{
    public NetworkedPlayers networkedPlayers;
    public LobbyUi lobbyUi;
    void Start()
    {
        if (IsServer) {
            ServerPopulateCards();
            networkedPlayers.allNetPlayers.OnListChanged += ServerNetPlayersChanged;
            lobbyUi.ShowStart(true);
            lobbyUi.OnStartClicked += ServerStartClicked;
        } else {
            ClientPopulateCards();
            networkedPlayers.allNetPlayers.OnListChanged += ClientNetPlayerChanged;
            lobbyUi.ShowStart(false);
            lobbyUi.OnReadyToggled += ClientOnReadyToggled;
            NetworkManager.OnClientDisconnectCallback += ClientOnClientDisconnected;
    }
            lobbyUi.OnChangeNameClicked += OnChangeNameClicked;
}

    private void OnChangeNameClicked(string newValue)
    {
        UpdatePlayerNameServerRpc(newValue);
    }
    private void PopulateMyInfo()
    {
        NetworkPlayerInfo myInfo = networkedPlayers.GetMyPlayerInfo();
        if(myInfo.clientId != ulong.MaxValue) 
        {
            lobbyUi.SetPlayerName(myInfo.playerName.ToString());
        }
    }

    private void ServerPopulateCards()
    {
        lobbyUi.playerCards.Clear();
        foreach(NetworkPlayerInfo info in networkedPlayers.allNetPlayers)
        {
            PlayerCard pc = lobbyUi.playerCards.AddCard("Some Player");
            pc.clientId = info.clientId;
            pc.ready = info.ready;
            pc.color = info.color;
            pc.playerName = info.playerName.ToString();
            if(info.clientId == NetworkManager.LocalClientId)
            {
                pc.ShowKick(false);
            } else {
                pc.ShowKick(true);
            }
            pc.OnKickClicked += ServerOnKickClicked;
            pc.UpdateDisplay();
        }
    }

        private void ServerStartClicked() {
            NetworkManager.SceneManager.LoadScene(
            "Arena1Game",
            UnityEngine.SceneManagement.LoadSceneMode.Single);
        }

        private void ClientPopulateCards()
    {
        lobbyUi.playerCards.Clear();
        foreach(NetworkPlayerInfo info in networkedPlayers.allNetPlayers)
        {
            PlayerCard pc = lobbyUi.playerCards.AddCard("Some Player");
            pc.clientId = info.clientId;
            pc.ready = info.ready;
            pc.color = info.color;
            pc.playerName = info.playerName.ToString();
            pc.ShowKick(false);
            pc.UpdateDisplay();
        }
    }

    private void ClientOnReadyToggled(bool newValue)
    {
        UpdateReadyServerRpc(newValue);
    }

    private void ServerNetPlayersChanged(NetworkListEvent<NetworkPlayerInfo> changeEvent) {
    ServerPopulateCards();
    lobbyUi.EnableStart(networkedPlayers.AllPlayersReady());
    }

    private void ServerOnKickClicked(ulong clientId)
    {
        NetworkManager.DisconnectClient(clientId);
    }
      private void ClientNetPlayerChanged(NetworkListEvent<NetworkPlayerInfo> changeEvent) {
        ClientPopulateCards();
        PopulateMyInfo();

      }

      private void ClientOnClientDisconnected(ulong clientId) {
        lobbyUi.gameObject.SetActive(false);
      }

    [ServerRpc(RequireOwnership = false)]
    private void UpdateReadyServerRpc(bool newValue, ServerRpcParams rpcParams = default)
    {
        networkedPlayers.UpdateReady(rpcParams.Receive.SenderClientId, newValue);
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdatePlayerNameServerRpc(string newValue, ServerRpcParams rpcParams = default) {
        networkedPlayers.UpdatePlayerName(rpcParams.Receive.SenderClientId, newValue);
    }
}


