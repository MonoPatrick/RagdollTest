using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField]
    private Rigidbody rb;

    [SerializeField]
    private ConfigurableJoint mainJoint;

    [SerializeField]
    Animator animator;

    //input
    private Vector2 moveInputVector = Vector2.zero;
    private bool isJumping = false;

    //controller settings
    private float maxSpeed = 3f;

    //states
    private bool isGrounded = false;

    private bool isRagdoll = false;

    //raycasts
    private RaycastHit[] raycastHits = new RaycastHit[10];

    //syncing physic objects
    SyncPhysicsObject[] syncPhysicsObjects;

    void Awake()
    {
        syncPhysicsObjects = GetComponentsInChildren<SyncPhysicsObject>();
    }

    // Update is called once per frame
    void Update()
    {
        moveInputVector.x = Input.GetAxis("Horizontal");
        moveInputVector.y = Input.GetAxis("Vertical");

        if(Input.GetKey(KeyCode.Space))
        {
            isJumping = true;
        }

    }

    private void FixedUpdate()
    {
        //assume not grounded
        isGrounded = false;

        //check if grounded
        int numberOfHits = Physics.SphereCastNonAlloc(rb.position, 0.1f, transform.up * -1, raycastHits, 0.5f);

        for (int i = 0; i < numberOfHits; i++)
        {

            //if hit self just continue and ignore it (continue just skips an iteration)
            if (raycastHits[i].transform.root == transform)
            {
                continue;
            }

            isGrounded = true;
            break;
        }

        //if in air add force downwards to counteract floaty feel unity gives
        if(!isGrounded)
        {
            rb.AddForce(Vector3.down * 10);
        }

        Vector3 localVelocityVsForward = transform.forward * Vector3.Dot(transform.forward, rb.velocity);

        float localForwardVelocity = localVelocityVsForward.magnitude;

        float inputMagnatude = moveInputVector.magnitude;

        if(inputMagnatude != 0)
        {
            Quaternion desiredDirection = Quaternion.LookRotation(new Vector3(moveInputVector.x, 0, moveInputVector.y * -1), transform.up);

            mainJoint.targetRotation = Quaternion.RotateTowards(mainJoint.targetRotation, desiredDirection, Time.fixedDeltaTime * 300);

           

            if(localForwardVelocity < maxSpeed)
            {
                rb.AddForce(transform.forward * inputMagnatude * 30);
            }
        }

        if(isGrounded && isJumping)
        {
            rb.AddForce(Vector3.up*10, ForceMode.Impulse);

            isJumping = false;
        }

        animator.SetFloat("movementSpeed", localForwardVelocity* 0.4f);//this controls how fast the legs move

        //update joints based on animation
        for (int i = 0; i < syncPhysicsObjects.Length; i++)
        {
            syncPhysicsObjects[i].UpdateJointFromAnimation();
        }
    }
}
