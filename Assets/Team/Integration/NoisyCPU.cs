using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public static class NoisyCPU
{
    // Start is called before the first frame update
    // BELOW HERE ARE REIMPLEMENTATION OF NOISE FUNCTIONS
    // These are identical to the "NoisyNodes" implementations,
    // The only difference being that these run on the CPU
    public static float4 mod(float4 x, float4 y)
    {
        return x - y * math.floor(x / y);
    }

    public static float3 mod(float3 x, float3 y)
    {
        return x - y * math.floor(x / y);
    }

    public static float2 mod289(float2 x)
    {
        return x - math.floor(x / 289.0f) * 289.0f;
    }

    public static float3 mod289(float3 x)
    {
        return x - math.floor(x / 289.0f) * 289.0f;
    }

    public static float4 mod289(float4 x)
    {
        return x - math.floor(x / 289.0f) * 289.0f;
    }

    public static float4 permute(float4 x)
    {
        return mod289(((x * 34.0f) + 1.0f) * x);
    }

    public static float3 permute(float3 x)
    {
        return mod289((x * 34.0f + 1.0f) * x);
    }

    public static float4 taylorInvSqrt(float4 r)
    {
        return (float4)1.79284291400159 - r * 0.85373472095314f;
    }

    public static float3 taylorInvSqrt(float3 r)
    {
        return 1.79284291400159f - 0.85373472095314f * r;
    }

    public static float3 fade(float3 t)
    {
        return t * t * t * (t * (t * 6.0f - 15.0f) + 10.0f);
    }

    public static float2 fade(float2 t)
    {
        return t * t * t * (t * (t * 6.0f - 15.0f) + 10.0f);
    }


    public static float rand3dTo1d(float3 value, float3 dotDir)
    {
        //make value smaller to avoid artefacts
        float3 smallValue = math.sin(value);
        //get scalar value from 3d vector
        float random = math.dot(smallValue, dotDir);
        //make value more random by making it bigger and then taking the factional part
        random = math.frac(math.sin(random) * 143758.5453f);
        return random;
    }

    public static float rand2dTo1d(float2 value, float2 dotDir)
    {
        float2 smallValue = math.sin(value);
        float random = math.dot(smallValue, dotDir);
        random = math.frac(math.sin(random) * 143758.5453f);
        return random;
    }

    public static float rand1dTo1d(float value, float mutator = 0.546f)
    {
        float random = math.frac(math.sin(value + mutator) * 143758.5453f);
        return random;
    }

    //to 2d functions

    public static float2 rand3dTo2d(float3 value)
    {
        return new float2(
            rand3dTo1d(value, new float3(12.989f, 78.233f, 37.719f)),
            rand3dTo1d(value, new float3(39.346f, 11.135f, 83.155f))
        );
    }

    public static float2 rand2dTo2d(float2 value)
    {
        return new float2(
            rand2dTo1d(value, new float2(12.989f, 78.233f)),
            rand2dTo1d(value, new float2(39.346f, 11.135f))
        );
    }

    public static float2 rand1dTo2d(float value)
    {
        return new float2(
            rand2dTo1d(value, 3.9812f),
            rand2dTo1d(value, 7.1536f)
        );
    }

    //to 3d functions

    public static float3 rand3dTo3d(float3 value)
    {
        return new float3(
            rand3dTo1d(value, new float3(12.989f, 78.233f, 37.719f)),
            rand3dTo1d(value, new float3(39.346f, 11.135f, 83.155f)),
            rand3dTo1d(value, new float3(73.156f, 52.235f, 09.151f))
        );
    }

    public static float3 rand2dTo3d(float2 value)
    {
        return new float3(
            rand2dTo1d(value, new float2(12.989f, 78.233f)),
            rand2dTo1d(value, new float2(39.346f, 11.135f)),
            rand2dTo1d(value, new float2(73.156f, 52.235f))
        );
    }

    public static float3 rand1dTo3d(float value)
    {
        return new float3(
            rand1dTo1d(value, 3.9812f),
            rand1dTo1d(value, 7.1536f),
            rand1dTo1d(value, 5.7241f)
        );
    }
    public static float cnoise(float2 P)
    {
        float4 Pi = math.floor(P.xyxy) + math.float4(0.0f, 0.0f, 1.0f, 1.0f);
        float4 Pf = math.frac(P.xyxy) - math.float4(0.0f, 0.0f, 1.0f, 1.0f);
        Pi = mod289(Pi); // To avoid truncation effects in permutation
        float4 ix = Pi.xzxz;
        float4 iy = Pi.yyww;
        float4 fx = Pf.xzxz;
        float4 fy = Pf.yyww;

        float4 i = permute(permute(ix) + iy);

        float4 gx = math.frac(i / 41.0f) * 2.0f - 1.0f;
        float4 gy = math.abs(gx) - 0.5f;
        float4 tx = math.floor(gx + 0.5f);
        gx = gx - tx;

        float2 g00 = new float2(gx.x, gy.x);
        float2 g10 = new float2(gx.y, gy.y);
        float2 g01 = new float2(gx.z, gy.z);
        float2 g11 = new float2(gx.w, gy.w);

        float4 norm = taylorInvSqrt(new float4(math.dot(g00, g00), math.dot(g01, g01), math.dot(g10, g10), math.dot(g11, g11)));
        g00 *= norm.x;
        g01 *= norm.y;
        g10 *= norm.z;
        g11 *= norm.w;

        float n00 = math.dot(g00, new float2(fx.x, fy.x));
        float n10 = math.dot(g10, new float2(fx.y, fy.y));
        float n01 = math.dot(g01, new float2(fx.z, fy.z));
        float n11 = math.dot(g11, new float2(fx.w, fy.w));

        float2 fade_xy = fade(Pf.xy);
        float2 n_x = math.lerp(new float2(n00, n01), new float2(n10, n11), fade_xy.x);
        float n_xy = math.lerp(n_x.x, n_x.y, fade_xy.y);
        return ((2.3f * n_xy) + 1f) / 2f;
    }
}
