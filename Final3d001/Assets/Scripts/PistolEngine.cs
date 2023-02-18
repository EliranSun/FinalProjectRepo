using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolEngine : MonoBehaviour
{
    public float damage = 10f;
    public float range = 100f;
    public float fireRate = 0.1f;
    public int magazineSize = 12;
    public float bulletSpeed = 500f;

    public Camera fpsCamera;
    public GameObject gunBarrel;
    public GameObject bulletPrefab;
    public GameObject bulletHolePrefab;

    private int roundsRemaining;
    private int roundsInInventory;
    private float nextFireTime = 0f;

    void Start()
    {
        roundsRemaining = magazineSize;
        roundsInInventory = magazineSize * 4;
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && roundsRemaining > 0 && Time.time >= nextFireTime)
        {
            Shoot();
            roundsRemaining--;
            nextFireTime = Time.time + fireRate;
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
        Debug.Log(roundsInInventory);

        GameObject bullet = Instantiate(bulletPrefab, gunBarrel.transform.position, gunBarrel.transform.rotation);                
       

        RaycastHit hit;
        if (Physics.Raycast(fpsCamera.transform.position, fpsCamera.transform.forward, out hit, range))
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
}
