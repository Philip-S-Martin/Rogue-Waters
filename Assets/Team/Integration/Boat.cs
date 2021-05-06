using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boat : MonoBehaviour
{
    public Rigidbody rigidBody;
    // Start is called before the first frame update
    void Start()
    {
        rigidBody.centerOfMass = new Vector3(0f, 2f, 18f);
        rigidBody.inertiaTensor = new Vector3(10000000, 30000000, 6000000);
    }

}
