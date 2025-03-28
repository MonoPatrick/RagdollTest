using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandGrabHandler : MonoBehaviour
{
   //SerializeField] Animator animator;

    FixedJoint fixedJoint;
    Rigidbody rigidbody3D;

    void Awake()
    {
        rigidbody3D = GetComponent<Rigidbody>(); // Fixed typo
    }

    bool TryCarryObject(Collision collision)
    {
        // Ensure rigidbody3D exists
      

        if (fixedJoint != null)
            return false; // Prevent multiple joints

        if (collision.transform.root == rigidbody3D.transform) 
            return false;
        //gets the rigid body object
        if (!collision.collider.TryGetComponent(out Rigidbody otherObjectRigidbody))
            return false; // Make sure object has a Rigidbody
        Debug.Log("HI");

        //Adds the object as a fixed joint
        fixedJoint = transform.gameObject.AddComponent<FixedJoint>();

        //connects the join to the rigid body
        fixedJoint.connectedBody = otherObjectRigidbody; // Corrected property name
        Debug.Log(otherObjectRigidbody.name);

        fixedJoint.autoConfigureConnectedAnchor = false;

        
        fixedJoint.connectedAnchor = collision.transform.InverseTransformPoint(collision.GetContact(0).point);

        return true;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Object"))
        {
            Debug.Log("");
            TryCarryObject(collision);
        }
    }

}
