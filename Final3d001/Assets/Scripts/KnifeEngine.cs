using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class KnifeEngine : NetworkBehaviour
{

    public float damage = 34;
    public float range = 2f;
    private Camera cam;
    public GameObject Weapon;

    public float stabCooldown = 0.5f;
    private bool canStab = true;
    private float currentStabCooldown = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.Find("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner)
        {
            return;
        }
        if (Input.GetButtonDown("Fire1") && canStab)
        {
            StartCoroutine(StartStab());
            StartCoroutine(StartShot());
            canStab = false;
            currentStabCooldown = stabCooldown;
        }

        if (!canStab)
        {
            currentStabCooldown -= Time.deltaTime;
            if (currentStabCooldown <= 0.0f)
            {
                canStab = true;
            }
        }
    }


    IEnumerator StartStab()
    {
        Weapon.GetComponent<Animator>().Play("KnifeStab");
        yield return new WaitForSeconds(0.7f);
        Weapon.GetComponent<Animator>().Play("New State");
    }

    IEnumerator StartShot()
    {
        yield return new WaitForSeconds(0.23f); //Anim hit delay
        ShootRaycast();
    }

    void ShootRaycast()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range))
        {
            print(hit.transform.name);

            Target target = hit.transform.GetComponent<Target>();
            if (target != null)
            {
                print("Stabbed");
                target.TakeDamage(damage);
            }
        }
    }


}
