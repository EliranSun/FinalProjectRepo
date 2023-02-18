using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: GameManager should inherit from NetworkBehaviour and transfer data to all objects
public class TimeShiftableObject : MonoBehaviour
{
    [SerializeField]
    int timeShiftLevel = 0;

    [SerializeField]
    List<GameObject> upgradeables;

    void Start()
    {
        for (int i = 0; i < upgradeables.Count; i++)
        {
            GameObject upgradeableObject = Instantiate(upgradeables[i], transform.position, Quaternion.identity);
            upgradeableObject.SetActive(i == timeShiftLevel);
            upgradeableObject.transform.parent = transform;
        }
    }

    public void TimeShift()
    {
        if (timeShiftLevel + 1 >= upgradeables.Count)
        {
            timeShiftLevel = 0;
        }
        else
        {
            timeShiftLevel++;
        }

        for (int i = 0; i < upgradeables.Count; i++)
        {
            transform.GetChild(i).gameObject.SetActive(i == timeShiftLevel);
        }
    }
}
