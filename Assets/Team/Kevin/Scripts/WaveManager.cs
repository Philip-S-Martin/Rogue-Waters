using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class WaveManager : MonoBehaviour
{
    WaveData waveData;
    public Material waterMat;
    WaterSource waterSource; 
    MeshObjectMap<WaterSource> mappedSources;
    private void Awake()
    {
        waveData = new WaveData(waterMat);
        waterSource = new WaterSource();
        mappedSources = new MeshObjectMap<WaterSource>();

        mappedSources.NewObject(waterSource, Vector3.zero);
        waterSource.GenerateRange(Vector3.zero, 1024f, 64, waterMat);
        mappedSources.UpdateAllMeshes();
        // Create wave data based on water material
        GameObject baseobject = mappedSources.GetMapped(waterSource);
        for(int i = -3; i < 4; i++)
        {
            for (int j = -3; j < 4; j++)
            {
                if (!(i == 0 && j == 0))
                    GameObject.Instantiate(baseobject, new Vector3(1024f * i, 0f, 1024f*j),Quaternion.identity);
            }
        }
        
    }
    // Data holding nested class. Copies and carries data from a water material
    private class WaveData
    {
        public float O1ScaleX, O1ScaleY, O1Speed, O1Height, O2ScaleX, O2ScaleY, O2Speed, O2Height;
        public WaveData(Material mat)
        {
            O1ScaleX = mat.GetFloat("Octave1_ScaleX");
            O1ScaleY = mat.GetFloat("Octave1_ScaleY");
            O1Speed = mat.GetFloat("Octave1_Speed");
            O1Height = mat.GetFloat("Octave1_Height");

            O2ScaleX = mat.GetFloat("Octave2_ScaleX");
            O2ScaleY = mat.GetFloat("Octave2_ScaleY");
            O2Speed = mat.GetFloat("Octave2_Speed");
            O2Height = mat.GetFloat("Octave2_Height");
        }
    }
    // GetWaveHeight - identical to the top half of WaterGraph, this is just an implementation for CPU
    public float GetWaveHeight(Vector3 pos)
    {
        float2 scaleVec1 = new float2(waveData.O1ScaleX, waveData.O1ScaleY);
        float2 scaleVec2 = new float2(waveData.O2ScaleX, waveData.O2ScaleY);
        float2 pos2d = new float2(pos.x, pos.z);
        float2 scaledPos1 = new float2(waveData.O1ScaleX * pos2d.x, waveData.O1ScaleY * pos2d.y);
        float2 scaledPos2 = new float2(waveData.O2ScaleX * pos2d.x, waveData.O2ScaleY * pos2d.y);

        float2 normScale1 = math.normalize(scaleVec1);
        float2 normScale2 = math.normalize(scaleVec2);

        float time = Time.time;
        float o1Time = time * waveData.O1Speed;
        float2 samplePos1 = scaledPos1 / normScale1 + o1Time;
        float o2Time = time * waveData.O2Speed;
        float2 samplePos2 = scaledPos2 / normScale2 + o2Time;

        float sampleHeight1 = NoisyCPU.cnoise(samplePos1) * waveData.O1Height;
        float sampleHeight2 = NoisyCPU.cnoise(samplePos2) * waveData.O2Height;
        float sampleHeight = sampleHeight1 + sampleHeight2;

        return sampleHeight;
    }
}
