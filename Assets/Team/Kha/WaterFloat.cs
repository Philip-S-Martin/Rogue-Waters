using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class WaterFloat : MonoBehaviour
{
    public Material waterMat;

    public float AirDrag = 1;
    public float WaterDrag = 10;
    public Transform[] FloatPoints;
    public bool AttachToSurface;
    protected Rigidbody Rigidbody;
    protected Waves Waves;


    protected float WaterLine;
    protected Vector3[] WaterLinePoints;

    // help vectors
    protected Vector3 centerOffset;
    protected Vector3 smoothVectorRotation;
    protected Vector3 TargetUp;

    public Vector3 Center
    {
        get { return transform.position + centerOffset; }
    }

    private void Awake()
    {
        Waves = FindObjectOfType<Waves>();
        Rigidbody = GetComponent<Rigidbody>();
        Rigidbody.useGravity = true;
        
        WaterLinePoints = new Vector3[FloatPoints.Length];
        for (int i = 0; i < FloatPoints.Length; i++)
        {
            WaterLinePoints[i] = FloatPoints[i].position;
        }

        centerOffset = PhysicsHelper.GetCenter(WaterLinePoints) - transform.position;
        Rigidbody.centerOfMass = new Vector3(0f, 3f, 22f);
        //Rigidbody.ResetCenterOfMass();
        //Rigidbody.ResetInertiaTensor();
        Rigidbody.inertiaTensor = new Vector3(10000000f, 40000000f, 50000000f);
    }

    private void FixedUpdate()
    {
        //default water surface
        var newWaterLine = 0f;
        var gravity = Physics.gravity / FloatPoints.Length;
        var floatForce = -gravity*2f;
        //transform.position = new Vector3(transform.position.x, GetWaterHeight(transform.position), transform.position.z);
        
        for (int i = 0; i < FloatPoints.Length; i++)
        {
            WaterLinePoints[i] = FloatPoints[i].position;
            //WaterLinePoints[i].y = GetWaterHeight(FloatPoints[i].position);
            newWaterLine += WaterLinePoints[i].y / FloatPoints.Length;
            float depth = WaterLinePoints[i].y - FloatPoints[i].position.y;
            if (depth > 0)
            {
                Rigidbody.AddForceAtPosition(floatForce * math.lerp(0f,1f,math.min(depth,1f)), FloatPoints[i].position, ForceMode.Acceleration);
            }
            
            //Rigidbody.AddForceAtPosition(gravity, FloatPoints[i].position);
        }
    }
    


    

    



    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.green;
    //    if (FloatPoints == null)
    //        return;

    //    for (int i = 0; i < FloatPoints.Length; i++)
    //    {
    //        if (FloatPoints[i] == null)
    //            continue;

    //        if (Waves != null)
    //        {

    //            //draw cube
    //            Gizmos.color = Color.red;
    //            Gizmos.DrawCube(WaterLinePoints[i], Vector3.one * 0.3f);
    //        }

    //        //draw sphere
    //        Gizmos.color = Color.green;
    //        Gizmos.DrawSphere(FloatPoints[i].position, 0.1f);

    //    }

    //    //draw center
    //    if (Application.isPlaying)
    //    {
    //        Gizmos.color = Color.red;
    //        Gizmos.DrawCube(new Vector3(Center.x, WaterLine, Center.z), Vector3.one * 1f);
    //        //Gizmos.DrawRay(new Vector3(Center.x, WaterLine, Center.z), TargetUp * 1f);
    //    }
    //}
}
