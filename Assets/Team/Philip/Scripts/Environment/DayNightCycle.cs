using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public float timeSpeed = 0.005f;
    public Vector3 dawnDirection = new Vector3(-.2f,0f,-.6f);
    public Vector3 sunApex = new Vector3(0,1,0);
    public float startTime = 0.25f;
    float currentTime;
    Quaternion sixHourRotation;
    // Start is called before the first frame update
    void Start()
    {
        sixHourRotation = Quaternion.FromToRotation(-dawnDirection, sunApex);
        currentTime = startTime;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        currentTime += Time.fixedDeltaTime * timeSpeed;
        if (currentTime > 2.0f)
            currentTime += Time.fixedDeltaTime * timeSpeed * (1f - Mathf.Abs(3f - currentTime));

        currentTime %= 4f;
        gameObject.transform.forward = Quaternion.SlerpUnclamped(Quaternion.identity, sixHourRotation, currentTime) * dawnDirection;
    }
}
