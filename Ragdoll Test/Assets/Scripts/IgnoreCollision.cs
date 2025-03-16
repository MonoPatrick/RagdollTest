using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreCollision : MonoBehaviour
{
    [SerializeField]
    Collider thisCollider;

    [SerializeField]
    Collider[] CollidersToIgnore;

    // Start is called before the first frame update
    void Start()
    {
        
        //looping through array to ignore colliders such as the ball collider on the bottom so that the arms and legs etc move properly and dont iteract with them

        foreach (Collider OtherCollider in CollidersToIgnore)
        {
            Physics.IgnoreCollision(thisCollider, OtherCollider, true);
        }

    }

 
}
