using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MotorBoatControls : MonoBehaviour
{
    public Transform Motor;
    public float SteerPower = 100f;
    public float Power = 5f;
    public float MaxSpeed = 10f;
    public float Drag = 0.1f;

    protected Rigidbody RigidBody;
    protected Quaternion StartRotation;

    float thrust = 0;
    float steer = 0;

    public void Awake()
    {
        RigidBody = GetComponent<Rigidbody>();
        StartRotation = Motor.localRotation;
    }
    public void OnThrust(InputValue input)
    {
        thrust = input.Get<float>();
        Debug.Log("thrust");
    }
    public void OnSteer(InputValue input)
    {
        steer = input.Get<float>();
    }
    void FixedUpdate()
    {
        RigidBody.AddForceAtPosition(steer * thrust * transform.right * SteerPower, Motor.position);
        var forward = Vector3.Scale(new Vector3(1, 0, 1), transform.forward);
        ApplyForceToReachVelocity(RigidBody, forward * thrust * MaxSpeed, Power);
    }
    public static void ApplyForceToReachVelocity(Rigidbody rigidbody, Vector3 velocity, float force = 1, ForceMode mode = ForceMode.Force)
    {
        if (force == 0 || velocity.magnitude == 0)
            return;

        velocity = velocity + velocity.normalized * 0.2f * rigidbody.drag;

        force = Mathf.Clamp(force, -rigidbody.mass / Time.fixedDeltaTime, rigidbody.mass / Time.fixedDeltaTime);

        if (rigidbody.velocity.magnitude == 0)
        {
            rigidbody.AddForce(velocity * force, mode);
        }
        else
        {
            var velocityProjectedToTarget = (velocity.normalized * Vector3.Dot(velocity, rigidbody.velocity) / velocity.magnitude);
            rigidbody.AddForce((velocity - velocityProjectedToTarget) * force, mode);
        }
    }
}
