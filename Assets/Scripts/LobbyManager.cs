using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class LobbyManager : NetworkBehaviour

{
    public Button startButton;
    public TMPro.TMP_Text statusLabel;

    // Start is called before the first frame update
    void Start()
    {
        startButton.onClick.AddListener(OnStartButtonClicked);
        NetworkManager.OnClientStarted += OnClientStarted;
        NetworkManager.OnServerStarted += OnServerStarted;

        startButton.gameObject.SetActive(false);
        statusLabel.text = "Start Client/Server/Host";
    }

        private void OnClientStarted()
    {
        if (!IsHost)
        {
            statusLabel.text = "Waiting for game to start";
        }
    }

    private void OnServerStarted()
    {
        GoToLobby();
    }
    private void OnStartButtonClicked()
    {
        StartGame();
    }

    public void GoToLobby() {
        NetworkManager.SceneManager.LoadScene(
            "Lobby",
            UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
    public void StartGame()
    {
        NetworkManager.SceneManager.LoadScene(
            "Arena1Game",
            UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}