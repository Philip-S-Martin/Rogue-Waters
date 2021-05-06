using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Rigidbody cameraBody;
    public GameObject followObject;
    Vector3 relativePosition;
    Quaternion relativeRotation;
    Vector3 baseforward;
    Vector3 maskY;
    float distance;
    private void Awake()
    {
        maskY = new Vector3(1f, 0f, 1f);
        relativePosition = transform.position - followObject.transform.position;
        relativeRotation = Quaternion.FromToRotation(followObject.transform.forward, relativePosition);
        baseforward = Vector3.Scale(followObject.transform.forward, maskY);
        distance = relativePosition.magnitude;
    }
    void FixedUpdate()
    {
        Vector3 forward = Vector3.Scale(followObject.transform.forward, maskY);
        float angle = Vector3.SignedAngle(baseforward, forward, Vector3.up);
        transform.position = followObject.transform.position + relativePosition;
        transform.RotateAround(followObject.transform.position, Vector3.up, angle);
        transform.LookAt(transform.position + Vector3.Scale(followObject.transform.forward, maskY));
    }
}
