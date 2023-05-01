using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossbowControl : MonoBehaviour
{
    [SerializeField]
    GameObject arrowPrefab;
    [SerializeField]
    float arrowSpeed = 10.0f;
    [SerializeField]
    float fireRate = 0.8f;
    [SerializeField]
    float arrowOffset = 1.0f; 

    private Animator myAnim;
    private bool canShoot = true;
    private float nextFireTime = 0.0f;

    void Start()
    {
        myAnim = gameObject.GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && canShoot && Time.time >= nextFireTime)
        {
            Vector3 arrowSpawnPos = transform.position + transform.forward * arrowOffset; 
            GameObject newArrow = Instantiate(arrowPrefab, arrowSpawnPos, Quaternion.LookRotation(transform.forward) * Quaternion.Euler(0, 180, 0));
            StartCoroutine(StartFireAnim());

            Rigidbody arrowRigidbody = newArrow.GetComponent<Rigidbody>();

            if (arrowRigidbody)
            {
                arrowRigidbody.AddForce(transform.forward * arrowSpeed * Time.deltaTime);
            }
        }

        if (Time.time >= nextFireTime)
        {
            canShoot = true;
        }
    }

    IEnumerator StartFireAnim()
    {
        canShoot = false;
        nextFireTime = Time.time + fireRate;

        myAnim.Play("CrossbowFire");
        yield return new WaitForSeconds(0.8f);
        myAnim.Play("New State");
    }
}