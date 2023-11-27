using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : NetworkBehaviour
{
    public NetworkVariable<Color> PlayerColor = new NetworkVariable<Color>(Color.red);
    public NetworkVariable<int> ScoreNetVar = new NetworkVariable<int>(0);
    public BulletSpawner bulletSpawner;

    public float movementSpeed = 50f;
    private float rotationSpeed = 130f;
    private Camera playerCamera;
    private  GameObject playerBody;

    private void NetworkInit()
    {
        playerBody = transform.Find("PlayerBody").gameObject;
        playerCamera = transform.Find("Camera").GetComponent<Camera>();

        playerCamera.enabled = IsOwner;
        playerCamera.GetComponent<AudioListener>().enabled = IsOwner;

        PlayerColor.OnValueChanged += OnPlayerColorChanged;
        ApplyPlayerColor();

        if (IsClient) {
            ScoreNetVar.OnValueChanged += ClientOnScoreValueChanged;
        }
    }

    void Start() {
        NetworkHelper.Log(this, "Start");
    }

    void Update() {
        if (IsOwner) {
            OwnerHandleMovementInput();
            if (Input.GetButtonDown("Fire1")) {
                NetworkHelper.Log("Requesting Fire");
                bulletSpawner.FireServerRpc();
            }
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if(IsServer) {
            ServerHandleCollision(collision);
        }
    }

    private void ServerHandleCollision(Collision collision) {
        if (collision.gameObject.CompareTag("bullet")) {
            ulong ownerId = collision.gameObject.GetComponent<NetworkObject>().OwnerClientId;
        NetworkHelper.Log(this,
        $"Hit by {collision.gameObject.name} " +
        $"owned by {ownerId}");
        Player other = NetworkManager.Singleton.ConnectedClients[ownerId].PlayerObject.GetComponent<Player>();
        other.ScoreNetVar.Value += 1;
        Destroy(collision.gameObject);
        }
    }

    public override void OnNetworkSpawn()
    {
        NetworkHelper.Log(this, "OnNetworkSpawn");
        NetworkInit();
        base.OnNetworkSpawn();
    }

    private void ClientOnScoreValueChanged(int old, int current) {
        if (IsOwner){
        NetworkHelper.Log(this, $"My score is {ScoreNetVar.Value}");
        }
    }

    public void OnPlayerColorChanged(Color previous, Color current) {
        ApplyPlayerColor();
    }

    [ServerRpc]
    private void MoveServerRpc(Vector3 posChange, Vector3 rotChange) {
        transform.Translate(posChange);
        transform.Rotate(rotChange);
    }
    private Vector3 CalcRotation() {
        bool isShiftKeyDown = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        Vector3 rotVect = Vector3.zero;
        if (!isShiftKeyDown) {
            rotVect = new Vector3(0, Input.GetAxis("Horizontal"), 0);
            rotVect *= rotationSpeed * Time.deltaTime;
        }
    }
}