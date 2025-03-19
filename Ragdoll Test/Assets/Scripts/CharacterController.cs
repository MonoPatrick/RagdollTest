using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CharacterController : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private ConfigurableJoint mainJoint;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform sphereCenterTransform; // The sphere center

    private Vector2 moveInputVector = Vector2.zero;
    private bool isJumping = false;

    private float maxSpeed = 5f;
    private float movementForce = 50f;
    private float rotationSpeed = 5f; // Smooth turning
    private float jumpForce = 10f;
    private float gravityForce = 15f;

    private bool isGrounded = false;
    private RaycastHit[] raycastHits = new RaycastHit[10];
    private SyncPhysicsObject[] syncPhysicsObjects;

    void Awake()
    {
        syncPhysicsObjects = GetComponentsInChildren<SyncPhysicsObject>();
    }

    void Update()
    {
        moveInputVector.x = Input.GetAxis("Horizontal"); // A/D or Left/Right
        moveInputVector.y = Input.GetAxis("Vertical");   // W/S or Up/Down

        if (Input.GetKey(KeyCode.Space))
        {
            isJumping = true;
        }
    }

    private void FixedUpdate()
    {
        isGrounded = false;

        // SphereCast to check if grounded
        int numberOfHits = Physics.SphereCastNonAlloc(rb.position, 0.2f, -transform.up, raycastHits, 0.5f);
        for (int i = 0; i < numberOfHits; i++)
        {
            if (raycastHits[i].transform.root == transform) continue;
            isGrounded = true;
            break;
        }

        // Get Camera Direction
        Transform cameraTransform = Camera.main.transform;
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        //Find Character's Upward Normal (relative to sphere)
        Vector3 sphereCenter = sphereCenterTransform.position;
        Vector3 characterToCenter = (rb.position - sphereCenter).normalized;

        //Project camera direction onto the sphere's surface
        cameraForward = Vector3.ProjectOnPlane(cameraForward, characterToCenter).normalized;
        cameraRight = Vector3.ProjectOnPlane(cameraRight, characterToCenter).normalized;

        // Get Movement Direction Based on Camera
        Vector3 moveDirection = (cameraRight * moveInputVector.x + cameraForward * moveInputVector.y).normalized;

        if (moveDirection.magnitude > 0.1f)
        {
            // Keep movement aligned to the sphere surface
            moveDirection = Vector3.ProjectOnPlane(moveDirection, characterToCenter).normalized;

            // Correctly Rotate Character to Face Movement
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, characterToCenter);
            mainJoint.targetRotation = Quaternion.Inverse(targetRotation); // Assigning correct ragdoll rotation

            // Apply movement force
            if (rb.velocity.magnitude < maxSpeed)
            {
                rb.AddForce(moveDirection * movementForce);
            }
        }

        if (!isGrounded)
        {
            rb.AddForce(characterToCenter * -gravityForce);
        }

        if (isGrounded && isJumping)
        {
            rb.AddForce(characterToCenter * jumpForce, ForceMode.Impulse);
            isJumping = false;
        }

        animator.SetFloat("movementSpeed", rb.velocity.magnitude * 0.4f);

        foreach (var obj in syncPhysicsObjects)
        {
            obj.UpdateJointFromAnimation();
        }
    }
}
