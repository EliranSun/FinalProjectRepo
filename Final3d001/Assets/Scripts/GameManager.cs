using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Player
{
    private ulong id;
    private Vector3 respawnPoint;
    private GameObject shiftableWeapon;

    public Player(ulong id, Vector3 respawnPoint, GameObject shiftableWeapon)
    {
        this.id = id;
        this.respawnPoint = respawnPoint;
        this.shiftableWeapon = shiftableWeapon;
    }
}

public class GameManager : MonoBehaviour
{
    static int UI_LAYER = 5;
    public static int GlobalKillCount = 0;
    private NetworkVariable<int> GlobalKillCountNetwork = new NetworkVariable<int>();
    [SerializeField] private GameObject shiftableWeapon;
    [SerializeField] private bool inspectMode = false;

    static HashSet<Player> playerIds = new HashSet<Player>();

    // List of spwan points in map. TODO: Some data structure that combines
    // Vector3 and boolean
    public static int[][] SpawnPoints = new int[31][]
    {
        new int[4] { 7, 0, 8, 0 },
        new int[4] { 20, 4, 25, 0 },
        new int[4] { 12, 0, 12, 0 },
        new int[4] { 25, 0, -1, 0 },
        new int[4] { 25, 5, 6, 0 },
        new int[4] { 52, 0, 10, 0 },
        new int[4] { 40, 0, 45, 0 },
        new int[4] { 40, 0, 73, 0 },
        new int[4] { 17, 0, 42, 0 },
        new int[4] { 10, 10, 45, 0 },
        new int[4] { 20, 0, 72, 0 },
        new int[4] { 7, 0, 8, 0 },
        new int[4] { 20, 4, 25, 0 },
        new int[4] { 6, 0, 60, 0 },
        new int[4] { -20, 0, 68, 0 },
        new int[4] { -5, 8, 21, 0 },
        new int[4] { -13, 0, 13, 0 },
        new int[4] { -25, 5, 42, 0 },
        new int[4] { -38, 0, 46, 0 },
        new int[4] { -36, 0, 74, 0 },
        new int[4] { -33, 0, 64, 0 },
        new int[4] { 53, 0, 14, 0 },
        new int[4] { 57, 4, -4, 0 },
        new int[4] { 32, 4, 14, 0 },
        new int[4] { 17, 5, 10, 0 },
        new int[4] { 31, 0, 39, 0 },
        new int[4] { 46, 0, 39, 0 },
        new int[4] { 47, 0, 10, 0 },
        new int[4] { 47, 6, 10, 0 },
        new int[4] { 44, 6, -20, 0 },
        new int[4] { 39, 0, 39, 0 },
    };

    void Start()
    {
        if (!inspectMode)
        {
            return;
        }

        // Instantiate mark for each spawn point
        foreach (int[] spawnPoint in SpawnPoints)
        {
            Vector3 position = new Vector3(spawnPoint[0], spawnPoint[1], spawnPoint[2]);
            GameObject mark = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            mark.transform.position = position;
            mark.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            mark.GetComponent<Renderer>().material.color = Color.red;
        }
    }

    void Update()
    {
        if (GlobalKillCountNetwork.Value != GlobalKillCount)
        {
            GlobalKillCount = GlobalKillCountNetwork.Value;
        }

        if (GlobalKillCount >= 1)
        {
            ShiftTimeForAllUpgradeableUIObjects();
            SendUpdateToAllClientsServerRpc(GlobalKillCount);
            GlobalKillCount = 0;
        }
    }

    [ServerRpc]
    public void UpdatePlayerIdsServerRpc(ulong playerId)
    {
        // TODO: on client disconnect, remove player from playerIds
        playerIds.Add(new Player(playerId, GetAvailableSpawnPoint(), shiftableWeapon));
        // print($"server RPC player ids {playerIds}");
        UpdatePlayerIdsClientRpc(playerId);
    }

    [ClientRpc]
    public void UpdatePlayerIdsClientRpc(ulong playerId)
    {
        playerIds.Add(new Player(playerId, GetAvailableSpawnPoint(), shiftableWeapon));
        // print($"client RPC player ids {playerIds}");
    }

    [ServerRpc]
    public void SendUpdateToAllClientsServerRpc(int killCount)
    {
        GlobalKillCountNetwork.Value = killCount;
    }

    public static void ShiftTimeForAllUpgradeableUIObjects()
    {
        GameObject[] timeShiftObjects = GameObject.FindGameObjectsWithTag("TimeShiftable");

        foreach (GameObject timeShiftObject in timeShiftObjects)
        {
            if (timeShiftObject.layer == UI_LAYER)
            {
                timeShiftObject.GetComponent<TimeShiftableObject>().TimeShift();
            }
        }
    }

    public static void ShiftTimeForAllUpgradeableObjects()
    {
        GameObject[] timeShiftObjects = GameObject.FindGameObjectsWithTag("TimeShiftable");

        foreach (GameObject timeShiftObject in timeShiftObjects)
        {
            timeShiftObject.GetComponent<TimeShiftableObject>().TimeShift();
        }
    }

    public static Vector3 GetAvailableSpawnPoint()
    {
        // TODO: Hold index of last spawn point used instead of looping through all
        foreach (int[] spawnPoint in SpawnPoints)
        {
            if (spawnPoint[3] != 0)
            {
                // != 0 aka =1, which means the spawn point is taken
                continue;
            }

            Vector3 availablePosition = new Vector3(spawnPoint[0], spawnPoint[1], spawnPoint[2]);
            spawnPoint[3] = 1; // 1 = taken
            print($"SpawnPoint: {availablePosition}");
            return availablePosition;
        }

        // For testing
        // foreach (int[] spawnPoint in SpawnPoints)
        // {
        //     spawnPoint[3] = 0;
        // }

        return new Vector3(7, 10, 8);
    }
}