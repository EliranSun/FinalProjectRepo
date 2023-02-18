using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class UpgradeableObject
//{
//    int level = 0;
//    public string name = "";
//    List<GameObject> upgradeables;

//    public UpgradeableObject(string name, List<GameObject> upgradeables)
//    {
//        this.name = name;
//        this.upgradeables = upgradeables;
//    }

//    public void TimeShift()
//    {
//        if (level + 1 >= upgradeables.Count)
//        {
//            level = 0;
//            return;
//        }

//        level++;
//    }

//    public GameObject GetTimeShiftedObject()
//    {
//        return upgradeables[level];
//    }
//}

public class GameManager : MonoBehaviour
{
    static int UI_LAYER = 5;
    public static int globalKillCount = 0;
    List<string> uniqueKilledPlayerIds = new List<string>();
    List<string> playerIds = new List<string>();

    // List of spwan points in map. TODO: Some data structure that combines
    // Vector3 and boolean
    public static int[][] spawnPoints = new int[10][]
    {
        new int[4] { 7, 0, 8, 0 },
        new int[4] { 20, 4, 26, 0 },
        new int[4] { 30, 0, 20, 0 },
        new int[4] { 40, 0, 20, 0 },
        new int[4] { 50, 0, 20, 0 },
        new int[4] { 60, 0, 20, 0 },
        new int[4] { 70, 0, 20, 0 },
        new int[4] { 80, 0, 20, 0 },
        new int[4] { 90, 0, 20, 0 },
        new int[4] { 100, 0, 20, 0 },
    };

    void Start() { }

    void Update()
    {
        if (uniqueKilledPlayerIds.Count >= playerIds.Count / 2)
        {
            ShiftTimeForAllUpgradeableUIObjects();
        }
    }

    public static void ShiftTimeForAllUpgradeableUIObjects()
    {
        GameObject[] timeShiftablesObjects = GameObject.FindGameObjectsWithTag("TimeShiftable");

        foreach (GameObject timeshiftableObject in timeShiftablesObjects)
        {
            if (timeshiftableObject.layer == UI_LAYER)
            {
                timeshiftableObject.GetComponent<TimeShiftableObject>().TimeShift();
            }
        }
    }

    public static void ShiftTimeForAllUpgradeableObjects()
    {
        GameObject[] timeShiftablesObjects = GameObject.FindGameObjectsWithTag("TimeShiftable");

        foreach (GameObject timeshiftableObject in timeShiftablesObjects)
        {
            timeshiftableObject.GetComponent<TimeShiftableObject>().TimeShift();
        }
    }

    public static Vector3 GetAvailableSpawnPoint()
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (spawnPoints[i][3] == 0) // 0 = not taken
            {
                Vector3 availablePosition = new Vector3(spawnPoints[i][0], spawnPoints[i][1], spawnPoints[i][2]);
                spawnPoints[i][3] = 1; // 1 = taken
                return availablePosition;
            }
        }

        return new Vector3(0, 0, 0);
    }
}
