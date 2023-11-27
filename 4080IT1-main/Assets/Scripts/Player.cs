using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

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

        ApplyPlayerColor();
        PlayerColor.OnValueChanged += OnPlayerColorChanged;

        if (IsClient) {
            ScoreNetVar.OnValueChanged += ClientOnScoreValueChanged;
        }
    }

        private void ApplyPlayerColor()
        {
            if (playerBody != null)
            {
                playerBody.GetComponent<Renderer>().material.color = PlayerColor.Value;
            }
        }
    private void Awake()
    {
        NetworkHelper.Log(this, "Awake");
    }

    void Start() {
        NetworkHelper.Log(this, "Start");
    }

    void Update() 
    {
        if (IsOwner) 
        {
            OwnerHandleInput();
            if (Input.GetButtonDown("Fire1")) 
            {
                NetworkHelper.Log("Requesting Fire");
                bulletSpawner.FireServerRpc();
            }
        }
    }

    public override void OnNetworkSpawn()
    {
        NetworkHelper.Log(this, "OnNetworkSpawn");
        NetworkInit();
        base.OnNetworkSpawn(); 
    }


    private void OnCollisionEnter(Collision collision) {
        if(IsServer) {
            ServerHandleCollision(collision);
        }
    }

    private void OnTriggerEnter (Collider other) {
        if (IsServer) {
            if (other.CompareTag("power_up"))
            {
                other.GetComponent<BasePowerUp>().ServerPickUp(this);
            }
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


    private void ClientOnScoreValueChanged(int old, int current) {
        if (IsOwner){
        NetworkHelper.Log(this, $"My score is {ScoreNetVar.Value}");
        }
    }

    private void OwnerHandleInput()
    {
        Vector3 movement = CalcMovement();
        Vector3 rotation = CalcRotation();

        if (IsHost)
        {
            MoveServerRpc(movement, rotation);
        }
        else
        {
            Vector3 newPosition = transform.position + movement;
            newPosition.x = Mathf.Clamp(newPosition.x, -8f, 8f);
            newPosition.z = Mathf.Clamp(newPosition.z, -8f, 8f);
            
            MoveServerRpc(movement, rotation);
        }
    }

    public void OnPlayerColorChanged(Color previous, Color current) {
        ApplyPlayerColor();
    }

       [ServerRpc(RequireOwnership = true)]
    private void MoveServerRpc(Vector3 posChange, Vector3 rotChange) {
        transform.Translate(posChange);
        transform.Rotate(rotChange);
    }

    [ServerRpc(RequireOwnership = true)]
    private void FireServerRpc() {
        NetworkHelper.Log("Fire");
        bulletSpawner.FireServerRpc();
    }

    private Vector3 CalcRotation() {
        bool isShiftKeyDown = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        Vector3 rotVect = Vector3.zero;
        if (!isShiftKeyDown) {
            rotVect = new Vector3(0, Input.GetAxis("Horizontal"), 0);
            rotVect *= rotationSpeed * Time.deltaTime;
        }
        return rotVect;
    }

        private Vector3 CalcMovement()
    {
        bool isShiftKeyDown = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        float x_move = 0.0f;
        float z_move = Input.GetAxis("Vertical");

        if (isShiftKeyDown)
        {
            x_move = Input.GetAxis("Horizontal");
        }

        Vector3 moveVect = new Vector3(x_move, 0, z_move);
        moveVect *= movementSpeed * Time.deltaTime;

        return moveVect;
    }
}