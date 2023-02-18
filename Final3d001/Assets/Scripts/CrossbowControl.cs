using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossbowControl : MonoBehaviour
{
    /*[SerializeField]
    Transform arrowSpawnPointPos;*/
    [SerializeField]
    GameObject arrowPrefab;
    [SerializeField]
    float arrowSpeed = 10.0f;
    [SerializeField]
    

    private Animator myAnim;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Collision detected");
        myAnim = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            GameObject newArrow = Instantiate(arrowPrefab, transform.position, Quaternion.Euler(0, transform.rotation.y + 180, 0));
            StartCoroutine(StartFireAnim());

            Rigidbody arrowRigidbody = newArrow.GetComponent<Rigidbody>();
            if (arrowRigidbody)
            {
                arrowRigidbody.AddForce(-transform.forward * arrowSpeed * Time.deltaTime);
            }
        }
    }
    

    IEnumerator StartFireAnim()
    {
        myAnim.Play("CrossbowFire");
        yield return new WaitForSeconds(0.5f);
        myAnim.Play("New State");
    }
}