using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PistolEngine : NetworkBehaviour
{
    public float damage = 51f;
    public float range = 100f;
    public float fireRate = 0.3f;
    public int magazineSize = 12;
    public float bulletSpeed = 500f;

    public Camera fpsCamera;
    public GameObject gunBarrel;
    public GameObject bulletPrefab;
    public GameObject bulletHolePrefab;

    private int roundsRemaining;
    private int roundsInInventory;
    private float nextFireTime = 0f;

    public GameObject weapon;

    void Start()
    {
        roundsRemaining = magazineSize;
        roundsInInventory = magazineSize * 1000;
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && roundsRemaining > 0 && Time.time >= nextFireTime)
        {
            Shoot();
            StartCoroutine(StartShootAnim());
            roundsRemaining--;
            nextFireTime = Time.time + fireRate;
            StartShootAnim();
        }

        if (Input.GetKeyDown(KeyCode.R) && roundsRemaining < magazineSize)
        {
            int roundsToAdd = Mathf.Min(magazineSize - roundsRemaining, roundsInInventory);
            roundsRemaining += roundsToAdd;
            roundsInInventory -= roundsToAdd;
        }
    }

    void Shoot()
    {
        // GetComponent<AudioSource>().Play();

        Debug.Log(roundsRemaining);



        GameObject bullet = Instantiate(bulletPrefab, transform);

        if (Physics.Raycast(fpsCamera.transform.position, fpsCamera.transform.forward, out RaycastHit hit, range))
        {
            Quaternion rotation = Quaternion.LookRotation(hit.normal, Vector3.up);
            GameObject bulletHole = Instantiate(bulletHolePrefab, hit.point, rotation);
            bulletHole.transform.parent = hit.transform;
            bulletHole.transform.position += hit.normal * 0.001f; // Offset-z



            Target targetHealth = hit.collider.GetComponent<Target>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(damage);
            }
        }

        Destroy(bullet, 2f);
    }

    IEnumerator StartShootAnim()
    {
        weapon.GetComponent<Animator>().Play("PistolFire");
        yield return new WaitForSeconds(0.3f);
        weapon.GetComponent<Animator>().Play("New State");
    }
}