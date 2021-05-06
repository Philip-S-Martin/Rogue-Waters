using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CircleInterpolator
{
    float fromAngle;
    float toAngle;
    float fromSin;
    float fromCos;
    float domain;
    float range;
    public CircleInterpolator(float unscaledFromAngle, float unscaledToAngle)
    {
        //float scaleRatio = widthScale / heightScale;
        //float baseRatio = (Mathf.Sin(unscaledFromAngle) - Mathf.Sin(unscaledToAngle)) / (Mathf.Cos(unscaledFromAngle) - Mathf.Cos(unscaledToAngle));
        //float multiple = baseRatio / scaleRatio;
        fromAngle = unscaledFromAngle; //Mathf.Atan(multiple * Mathf.Tan(unscaledFromAngle));
        toAngle = unscaledToAngle; //Mathf.Atan(multiple * Mathf.Tan(unscaledToAngle));
        fromSin = Mathf.Sin(fromAngle);
        fromCos = Mathf.Cos(fromAngle);
        range = Mathf.Sin(fromAngle) - Mathf.Sin(toAngle);
        domain = Mathf.Cos(toAngle) - Mathf.Cos(fromAngle);
    }
    public float GetX(float y)
    {
        return (Mathf.Cos(Mathf.Asin(fromSin - y * range)) - fromCos) / domain;
    }
}

