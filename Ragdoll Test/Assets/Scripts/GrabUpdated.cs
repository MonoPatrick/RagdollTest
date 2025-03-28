using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabUpdated : MonoBehaviour
{
    //public Animator animator;
    GameObject grabbedObj;
    public Rigidbody rb;
    public int isLeftorRight;

    public bool alreadyGrabbing = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(isLeftorRight))
        {
            if (grabbedObj != null)
            {
                FixedJoint fj = grabbedObj.GetComponent<FixedJoint>(); // Check if it already exists
                if (fj == null) // Prevent adding multiple FixedJoints
                {
                    fj = grabbedObj.AddComponent<FixedJoint>();
                    fj.connectedBody = rb;
                    fj.breakForce = 9001;
                }
            }
        }
        else if (Input.GetMouseButtonUp(isLeftorRight))
        {
            Debug.Log("Button Released");

            if (grabbedObj != null)
            {
                Debug.Log("Dropping object: " + grabbedObj.name);
                FixedJoint fj = grabbedObj.GetComponent<FixedJoint>();
                if (fj != null)
                {
                    Debug.Log("Destroying FixedJoint");
                    Destroy(fj);
                }

                grabbedObj = null;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Object"))
        {
            grabbedObj = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
    
            grabbedObj = null;
        
    }
}
