using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class WeaponSwitch : NetworkBehaviour
{
    public GameObject Slot1;
    public GameObject Slot2;
    public GameObject Slot3;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Equip1();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Equip2();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Equip3();
        }
    }

    void Equip1()
    {
        Slot1.SetActive(true);
        Slot2.SetActive(false);
        Slot3.SetActive(false);
    }

    void Equip2()
    {
        Slot1.SetActive(false);
        Slot2.SetActive(true);
        Slot3.SetActive(false);
    }

    void Equip3()
    {
        Slot1.SetActive(false);
        Slot2.SetActive(false);
        Slot3.SetActive(true);
    }
}
