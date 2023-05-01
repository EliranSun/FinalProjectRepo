using Unity.Netcode;
using UnityEngine;

// TODO: GameManager should inherit from NetworkBehaviour and transfer data to all objects
public class TimeShiftableObject : NetworkBehaviour
{
    [SerializeField] int timeShiftLevel = 0;
    public NetworkVariable<int> timeShiftLevelNetworkVariable = new NetworkVariable<int>(0);

    void Awake()
    {
        timeShiftLevelNetworkVariable.OnValueChanged += OnTimeShiftLevelChanged;
    }

    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(i == timeShiftLevel);
        }
    }

    void OnTimeShiftLevelChanged(int oldTimeShiftLevel, int newTimeShiftLevel)
    {
        print($"time shift level changed {oldTimeShiftLevel} -> {newTimeShiftLevel}");
        TimeShift();
    }

    public void TimeShift()
    {
        print($"time shift {timeShiftLevel} for {NetworkManager.LocalClientId}");
        if (timeShiftLevel + 1 >= transform.childCount)
        {
            timeShiftLevel = 0;
        }
        else
        {
            timeShiftLevel++;
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            bool isActive = i == timeShiftLevel;
            transform.GetChild(i).gameObject.SetActive(isActive);
        }

    }

    // [ServerRpc (RequireOwnership = false)]
    // void OnTimeShiftServerRpc(int serverTimeShiftLevel, ServerRpcParams serverRpcParams = default)
    // {
    //     var clientId = serverRpcParams.Receive.SenderClientId;
    //     if (!NetworkManager.ConnectedClients.ContainsKey(clientId))
    //     {
    //         return;
    //     }
    //     
    //     
    //     var client = NetworkManager.ConnectedClients[clientId];
    //     TimeShiftableObject shiftableWeapon = client.PlayerObject.GetComponentInChildren<TimeShiftableObject>();
    //     print($"server time shift {shiftableWeapon} for client id {clientId}");
    //     shiftableWeapon.TimeShiftClientRpc(serverTimeShiftLevel);
    // }
    //
    // [ClientRpc]
    // void TimeShiftClientRpc(int serverTimeShiftLevel)
    // {
    //     print($"client time shift {serverTimeShiftLevel} for client id {NetworkManager.LocalClientId}");
    //     for (int i = 0; i < transform.childCount; i++)
    //     {
    //         transform.GetChild(i).gameObject.SetActive(i == serverTimeShiftLevel);
    //     }
    // }
}
