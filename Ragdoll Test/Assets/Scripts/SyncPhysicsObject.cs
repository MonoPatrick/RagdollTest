using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncPhysicsObject : MonoBehaviour
{
    Rigidbody rb;
    ConfigurableJoint joint;

    [SerializeField]
    Rigidbody animatedRb;

    [SerializeField]
    bool syncAnimation = false;

    Quaternion startLocalRotation;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        joint = GetComponent<ConfigurableJoint>();

        //store starting rotation
        startLocalRotation = transform.localRotation;
    }

    public void UpdateJointFromAnimation()
    {
        if(!syncAnimation)
        {
            return;
        }

        ConfigurableJointExtensions.SetTargetRotationLocal(joint, animatedRb.transform.localRotation, startLocalRotation);
    }
}
