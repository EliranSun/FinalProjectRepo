using System;
using Unity.Netcode;
using UnityEngine;
using TMPro;

public class PlayerKillController : NetworkBehaviour
{
    bool oldIsBeingHit = false;
    private NetworkVariable<PlayerWeaponData> weaponState = new NetworkVariable<PlayerWeaponData>();
    private NetworkVariable<Vector3> playerPosition = new NetworkVariable<Vector3>();
    NetworkVariable<int> killCount = new NetworkVariable<int>(0);
    public NetworkVariable<int> health = new NetworkVariable<int>(100);

    int damage = 10;
    int range = 100;

    private void Awake()
    {
        gameObject.name = $"Player-{GetComponent<NetworkObject>().OwnerClientId}";

        playerPosition.OnValueChanged += OnPlayerPositionChanged;
        weaponState.OnValueChanged += OnWeaponStateChanged;
        health.OnValueChanged += OnHealthChanged;
        killCount.OnValueChanged += OnKillCountChanged;
    }

    private void Start()
    {
        if (IsOwner)
        {
            GameObject head = GameObject.Find("Head (1)");

            float red = Mathf.Clamp(UnityEngine.Random.value, 0f, 1f);
            float green = Mathf.Clamp(UnityEngine.Random.value, 0f, 1f);
            float blue = Mathf.Clamp(UnityEngine.Random.value, 0f, 1f);

            head.GetComponent<Renderer>().material.color = new Color(red, green, blue);
        }
    }

    void Update()
    {
        switch (weaponState.Value.weaponIndex % 3)
        {
            case 0: // knife
                damage = 51;
                range = 2;
                break;
            case 1: // crossbow
                damage = 42;
                range = 80;
                break;
            case 2: // pistol
                damage = 34;
                range = 100;
                break;
        }

        if (Input.GetButtonDown("Fire1"))
        {
            if (!Physics.Raycast(transform.position, transform.forward, out var hit, range))
            {
                return;
            }
            
            if (!IsOwner)
            {
                // TODO: Should move outside this scope
                return;
            }

            if (hit.collider.gameObject.CompareTag("Player"))
            {
                PlayerKillController hitPlayerController = hit.collider.gameObject.GetComponent<PlayerKillController>();
                hitPlayerController.TakeAHitServerRpc(damage);

                if (hitPlayerController.health.Value - damage <= 0)
                {
                    transmitWeaponIndexServerRpc();
                    TransmitPlayerKillCountServerRpc();
                }
            }
        }
    }

    [ServerRpc]
    void TransmitPlayerKillCountServerRpc()
    {
        killCount.Value++;
    }

    void OnKillCountChanged(int oldKillCount, int newKillCount)
    {
        print($"KILL COUNT: {newKillCount}");
    }

    private void OnHealthChanged(int oldHealth, int newHealth)
    {
        if (IsOwner)
        {
            GameObject.Find("PlayerMessages").GetComponent<TextMeshProUGUI>().text = $"Hit! {newHealth}/100 \r\n";
            Invoke(nameof(ClearPlayerMessages), 4);

            if (newHealth == 0)
            {
                health.Value = 100;
                Respawn();
            }
        }
    }

    private void OnPlayerPositionChanged(Vector3 oldPosition, Vector3 newPosition)
    {
        transform.position = newPosition;
    }

    private void OnWeaponStateChanged(
        PlayerWeaponData oldWeaponData,
        PlayerWeaponData newWeaponData
    )
    {
        try
        {
            GetComponentInChildren<TimeShiftableObject>().TimeShift();
        }
        catch (Exception e)
        {
            print($"error {e}");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void TakeAHitServerRpc(int damage = 10)
    {
        health.Value -= damage;
        GameObject.Find("PlayerMessages").GetComponent<TextMeshProUGUI>().text = $"Hit! {health.Value}/100 \r\n";
        Invoke(nameof(ClearPlayerMessages), 4);

        if (health.Value <= 0)
        {
            health.Value = 100;
            Respawn();
        }
    }

    void ClearPlayerMessages()
    {
        GameObject.Find("PlayerMessages").GetComponent<TextMeshProUGUI>().text = "";
    }

    void Respawn()
    {
        transmitPlayerPositionServerRpc(PlayerMovement.spawnPosition);
        transform.position = PlayerMovement.spawnPosition;
    }

    [ServerRpc(RequireOwnership = false)]
    void transmitPlayerPositionServerRpc(Vector3 position)
    {
        playerPosition.Value = position;
    }

    [ServerRpc(RequireOwnership = false)]
    void transmitWeaponIndexServerRpc(ServerRpcParams serverRpcParams = default)
    {
        ulong senderClientId = serverRpcParams.Receive.SenderClientId;
        if (!NetworkManager.ConnectedClients.ContainsKey(senderClientId))
        {
            return;
        }

        weaponState.Value = new PlayerWeaponData()
        {
            weaponIndex = weaponState.Value.weaponIndex + 1,
            playerId = senderClientId
        };
    }

    struct PlayerWeaponData : INetworkSerializable
    {
        public int weaponIndex;
        public ulong playerId;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer)
            where T : IReaderWriter
        {
            serializer.SerializeValue(ref weaponIndex);
            serializer.SerializeValue(ref playerId);
        }
    }

    // [ServerRpc]
    // void OnHitServerRpc(ulong clientId, ServerRpcParams serverRpcParams = default)
    // {
    //     if (NetworkManager.ConnectedClients.ContainsKey(clientId))
    //     {
    //         ulong senderClientId = serverRpcParams.Receive.SenderClientId;
    //         NetworkClient targetClient = NetworkManager.ConnectedClients[clientId];
    //
    //         print($"Hitting player {targetClient.PlayerObject.name} {targetClient.PlayerObject}");
    //         PlayerKillController playerTarget = targetClient.PlayerObject.GetComponent<PlayerKillController>();
    //         playerTarget.OnHitPlayerClientRpc(senderClientId);
    //         // announce all players
    //         TestClientRpc(senderClientId);
    //
    //         // if (playerTarget.health - 10 == 0)
    //         // {
    //         //     OnKillSuccess(senderClientId);
    //         // }
    //     }
    // }
    //
    // [ClientRpc]
    // void TestClientRpc(ulong killerId)
    // {
    //     GameObject killerPlayer = GameObject.Find($"Player-{killerId}");
    //     print($"Searching player {killerId} -- found? {killerPlayer}");
    //     killerPlayer.GetComponent<PlayerKillController>().shiftableWeapon.TimeShift();
    // }
    //
    // [ClientRpc]
    // void OnHitPlayerClientRpc(ulong killerId)
    // {
    //     health -= 10;
    //     print($"I was hit! Health: {health}, id: {NetworkObject.OwnerClientId}");
    //
    //     if (health <= 0)
    //     {
    //         health = 100;
    //         print($"{NetworkObject.OwnerClientId} was killed by {killerId}");
    //         // GetComponent<PlayerMovement>().Respawn();
    //     }
    // }
    //
    // void OnKillSuccess()
    // {
    //     killCount++;
    //     GameManager.GlobalKillCount++;
    //
    //     if (killCount % 1 == 0)
    //     {
    //         shiftableWeapon.TimeShift();
    //         // ShiftWeaponServerRpc(killerId);
    //     }
    // }
    //
    // [ServerRpc]
    // void ShiftWeaponServerRpc(ulong killerId, ServerRpcParams serverRpcParams = default)
    // {
    //     // ulong clientId = serverRpcParams.Receive.SenderClientId;
    //     if (NetworkManager.ConnectedClients.ContainsKey(killerId))
    //     {
    //         var client = NetworkManager.ConnectedClients[killerId];
    //         print($"server time shift for client id {killerId}");
    //         client.PlayerObject.GetComponent<PlayerKillController>().ShiftWeaponClientRpc();
    //     }
    // }

    // [ClientRpc]
    // void ShiftWeaponClientRpc()
    // {
    //     shiftableWeapon.TimeShift();
    // }
}
