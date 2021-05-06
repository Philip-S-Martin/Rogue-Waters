using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WaterLighting : MonoBehaviour
{
    public Material waterMat;
    void FixedUpdate()
    {
        waterMat.SetColor("sky_color", RenderSettings.ambientLight);
    }
}
