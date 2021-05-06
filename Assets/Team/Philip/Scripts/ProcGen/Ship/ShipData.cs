using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShipData
{
    [System.Serializable]
    public struct BowData
    {
        [Header("Deck")]
        [Range(0.0f, 1.0f)]
        public float widthRatio;
        [Range(0.0f, 1.0f)]
        public float topAngle;
        [Range(0.0f, 0.25f)]
        public float sheerRatio;
        [Range(0.0f, 1.0f)]
        public float sheerAngle;
        [Header("Wale")]
        [Range(0.0f, 1.0f)]
        public float waleDraftRatio;
        [Range(0.0f, 1.25f)]
        public float waleWidthRatio;
        [Header("Bilge")]
        [Range(0.0f, 1.0f)]
        public float bilgeDraftRatio;
        [Range(0.0f, 1.0f)]
        public float bilgeWidthRatio;
        [Header("Bottom")]
        [Range(0.0f, 1.0f)]
        public float bottomDraftRatio;
        [Range(0.0f, 1.0f)]
        public float bottomWidthRatio;
    }
    [System.Serializable]
    public struct MidshipData
    {
        [Header("Deck")]
        [Range(0.0f, 1.0f)]
        public float location;
        [Header("Wale")]
        [Range(0.0f, 1.0f)]
        public float waleDraftRatio;
        [Range(0.0f, 1.25f)]
        public float waleWidthRatio;
        [Header("Bilge")]
        [Range(0.0f, 1.0f)]
        public float bilgeDraftRatio;
        [Range(0.0f, 1.0f)]
        public float bilgeWidthRatio;
        [Header("Bottom")]
        [Range(0.0f, 1.0f)]
        public float bottomDraftRatio;
        [Range(0.0f, 1.0f)]
        public float bottomWidthRatio;
    }
    [System.Serializable]
    public struct SternData
    {
        [Header("Deck")]
        [Range(0.0f, 1.0f)]
        public float widthRatio;
        [Range(0.0f, 1.0f)]
        public float topAngle;
        [Range(0.0f, 0.25f)]
        public float sheerRatio;
        [Range(0.0f, 1.0f)]
        public float sheerAngle;
        [Header("Wale")]
        [Range(0.0f, 1.0f)]
        public float waleDraftRatio;
        [Range(0.0f, 1.25f)]
        public float waleWidthRatio;
        [Header("Bilge")]
        [Range(0.0f, 1.0f)]
        public float bilgeDraftRatio;
        [Range(0.0f, 1.0f)]
        public float bilgeWidthRatio;
        [Header("Bottom")]
        [Range(0.0f, 1.0f)]
        public float bottomDraftRatio;
        [Range(0.0f, 1.0f)]
        public float bottomWidthRatio;
    }

    // General Ship Properties
    [Header("General Properties")]
    [Range(5, 120)]
    public int length = 40;
    [Range(0.01f, 0.5f)]
    public float beamRatio = 0.25f;
    [Range(0.1f, 0.5f)]
    public float fullDraftRatio = 0.30f;
    [Header("Section Properties")]
    // Deck Properties
    public BowData bow = new BowData()
    {
        widthRatio = 0.0f,
        topAngle = 1f,
        sheerRatio = 0.1f,
        sheerAngle = 0.25f,
        waleDraftRatio = 0.5f,
        waleWidthRatio = 1.1f,
        bilgeDraftRatio = 0.3f,
        bilgeWidthRatio = 0.7f,
        bottomDraftRatio = 0.0f,
        bottomWidthRatio = 0.8f
    };
    public MidshipData midship = new MidshipData()
    {
        location = 0.3f,
        waleDraftRatio = 0.5f,
        waleWidthRatio = 1.1f,
        bilgeDraftRatio = 0.3f,
        bilgeWidthRatio = 0.7f,
        bottomDraftRatio = 0.0f,
        bottomWidthRatio = 0.8f
    };
    public SternData stern = new SternData()
    {
        widthRatio = 0.7f,
        topAngle = 1f,
        sheerRatio = 0.1f,
        sheerAngle = 0.25f,
        waleDraftRatio = 0.5f,
        waleWidthRatio = 1.1f,
        bilgeDraftRatio = 0.3f,
        bilgeWidthRatio = 0.7f,
        bottomDraftRatio = 0.0f,
        bottomWidthRatio = 0.8f
    };


    public Material material;

}
