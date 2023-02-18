using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowEngine : MonoBehaviour
{

    float penetrationDepth = 0.5f;




    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
		


    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Hit!");

        if (collision.gameObject.CompareTag("Arrow"))
        {
            GameObject arrow = collision.gameObject;

            arrow.transform.parent = collision.transform;
            arrow.transform.position = collision.contacts[0].point - arrow.transform.forward * penetrationDepth;

            Rigidbody arrowRigidbody = arrow.GetComponent<Rigidbody>();
            if (arrowRigidbody)
            {
                arrowRigidbody.isKinematic = true;
            }
        }
    }


}
