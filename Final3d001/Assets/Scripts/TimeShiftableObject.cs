using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeShiftableObject : MonoBehaviour
{
    [SerializeField] int timeShiftLevel = 0;
    [SerializeField] List<GameObject> upgradeables;
    
    void Start()
    {
        UpdateMesh();
    }

    public void TimeShift()
    {
        if (timeShiftLevel + 1 >= upgradeables.Count)
        {
            timeShiftLevel = 0;
        } else
        {
            timeShiftLevel++;
        }

        UpdateMesh();
    }

    void UpdateMesh()
    {
        GetComponent<MeshFilter>().mesh = upgradeables[timeShiftLevel].GetComponent<MeshFilter>().mesh;
    }
}