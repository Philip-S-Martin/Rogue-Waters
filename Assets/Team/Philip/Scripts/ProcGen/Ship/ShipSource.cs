using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipSource : MeshObject
{
    class Rib
    {
        public Vector3 deck;
        public Vector3 wale;
        public Vector3 bilge;
        public Vector3 bottom;
    }
    ShipData data;

    List<Rib> ribs;

    public ShipSource(ShipData data)
    {
        this.data = data;
        mesh.MeshMaterial = data.material;
    }
    public void Generate()
    {
        GenerateRibs();
        MeshDeck();
        MeshHull();
        mesh.RecalculateAllNormals();
    }
    public void GenerateHull()
    {
        float beam = data.beamRatio;
        float length = data.length;

    }
    public void GenerateNextSection()
    {

    }
    
    public void GenerateRibs()
    {
        // GIVEN
        float length = data.length;
        float halfBeam = data.beamRatio * length / 2f;
        float draft = length * data.fullDraftRatio;

        // Bow attributes
        float bowAngle = data.bow.topAngle * 0.5f * Mathf.PI;
        float bowLength = data.midship.location * length; // The length of the bow
        float bowBeam = data.bow.widthRatio * halfBeam;
        float bowSin = Mathf.Sin(bowAngle); // The sin of the first angle to be sampled for the bow
        float bowSheerHeight = data.bow.sheerRatio * draft;
        float bowSheerAngle = data.bow.sheerAngle * 0.5f * Mathf.PI;
        float bowSheerSin = Mathf.Sin(bowSheerAngle); ;

        // Stern attributes
        float sternAngle = data.stern.topAngle * 0.5f * Mathf.PI;
        float sternLength = length - bowLength; // The length of the stern
        float sternBeam = data.stern.widthRatio * halfBeam;
        float sternSin = Mathf.Sin(sternAngle);
        float sternSheerHeight = draft * data.stern.sheerRatio;
        float sternSheerAngle = data.stern.sheerAngle * 0.5f * Mathf.PI;
        float sternSheerSin = Mathf.Sin(sternSheerAngle); ;

        // CALCULATED
        // bow attributes
        float bowSampleProfile = 1f - Mathf.Cos(bowAngle);
        float bowSampleMult = 1 - bowBeam / halfBeam;
        float bowSheerSampleProfile = 1f - Mathf.Cos(bowSheerAngle);

        float midSampleProfile = 1 - Mathf.Cos(sternAngle);
        float midSampleMult = 1 - sternBeam / halfBeam;
        float sternSheerSampleProfile = 1f - Mathf.Cos(sternSheerAngle);

        CircleInterpolator bowDeck = new CircleInterpolator(bowAngle, 0f);
        CircleInterpolator bowSheer = new CircleInterpolator(bowSheerAngle, 0f);
        CircleInterpolator bowWales = new CircleInterpolator(1f, 0f);

        CircleInterpolator sternDeck = new CircleInterpolator(sternAngle, 0f);
        CircleInterpolator sternSheer = new CircleInterpolator(sternSheerAngle, 0);
        // SETTING RIB POSITIONS
        ribs = new List<Rib>();
        // incremental variables
        float deckX, deckY, deckZ = 0f;

        // Bow
        while (deckZ < bowLength)
        {
            float keyProp = deckZ / bowLength;
            deckY = draft + bowSheerHeight * (1f-bowSheer.GetX(keyProp));
            deckX = bowBeam + (halfBeam - bowBeam) * bowDeck.GetX(keyProp);
            Rib rib = new Rib();

            rib.deck = new Vector3(deckX, deckY, deckZ); 
            float waleProp = bowWales.GetX(keyProp);
            float waleX = deckX * (data.midship.waleWidthRatio * waleProp + data.bow.waleWidthRatio * (1f - waleProp));
            float waleY = deckY * (data.midship.waleDraftRatio * waleProp + data.bow.waleDraftRatio * (1f - waleProp));
            rib.wale = new Vector3(waleX, waleY, deckZ);
            float bilgeX = deckX * (data.midship.bilgeWidthRatio * waleProp + data.bow.bilgeWidthRatio * (1f - waleProp));
            float bilgeY = deckY * (data.midship.bilgeDraftRatio * waleProp + data.bow.bilgeDraftRatio * (1f - waleProp));
            rib.bilge = new Vector3(bilgeX, bilgeY, deckZ);
            float bottomX = deckX * (data.midship.bottomWidthRatio * waleProp + data.bow.bottomWidthRatio * (1f - waleProp));
            float bottomY = deckY * (data.midship.bottomDraftRatio * waleProp + data.bow.bottomDraftRatio * (1f - waleProp));
            rib.bottom = new Vector3(bottomX, bottomY, deckZ);
            ribs.Add(rib);
            deckZ++;
        }
        while (deckZ <= length)
        {
            float keyProp = (1f - (deckZ - bowLength) / sternLength);
            deckY = draft + sternSheerHeight * (1f - sternSheer.GetX(keyProp));
            deckX = sternBeam + (halfBeam - sternBeam) * sternDeck.GetX(keyProp);
            Rib rib = new Rib();
            rib.deck = new Vector3(deckX, deckY, deckZ);
            float waleProp = bowWales.GetX(keyProp);
            float waleX = deckX * (data.midship.waleWidthRatio * waleProp + data.stern.waleWidthRatio * (1f - waleProp));
            float waleY = deckY * (data.midship.waleDraftRatio * waleProp + data.stern.waleDraftRatio * (1f - waleProp));
            rib.wale = new Vector3(waleX, waleY, deckZ);
            float bilgeX = deckX * (data.midship.bilgeWidthRatio * waleProp + data.stern.bilgeWidthRatio * (1f - waleProp));
            float bilgeY = deckY * (data.midship.bilgeDraftRatio * waleProp + data.stern.bilgeDraftRatio * (1f - waleProp));
            rib.bilge = new Vector3(bilgeX, bilgeY, deckZ);
            float bottomX = deckX * (data.midship.bottomWidthRatio * waleProp + data.stern.bottomWidthRatio * (1f - waleProp));
            float bottomY = deckY * (data.midship.bottomDraftRatio * waleProp + data.stern.bottomDraftRatio * (1f - waleProp));
            rib.bottom = new Vector3(bottomX, bottomY, deckZ);
            ribs.Add(rib);
            deckZ++;
        }
    }

    ushort deckID = 0;
    ushort hullID = 0;
    bool deckMeshMade = false;
    bool hullMeshMade = false;
    void MeshDeck()
    {
        Vector3 leftScale = new Vector3(-1f, 1f, 1f);
        // Are the bow and stern tris or quads?
        ushort bowTri = 0, sternTri = 0;
        if (ribs[0].deck.x == 0f) bowTri = 1;
        if (ribs[ribs.Count - 1].deck.x == 0f) sternTri = 1;

        // Set the vertex and tri counts
        int verts = (ribs.Count + 1) * 2 - (bowTri + sternTri);
        int tris = (ribs.Count - 1) * 6 - 3 * (bowTri + sternTri);

        // clear preexisting mesh if it exists
        if (deckMeshMade) mesh.DeleteSubMesh((ushort)deckID);
        // reserve new mesh
        deckID = mesh.CreateSubMesh((ushort)(verts), (ushort)(tris));
        deckMeshMade = true;

        // index, vertex, and triangle iterators
        ushort i = 0;
        ushort vert = 0;
        ushort tri = 0;

        if (bowTri == 1)
        {
            mesh.SetSubVert(deckID, vert++, ribs[0].deck, Vector2.zero, Vector2.zero);
            mesh.SetSubTri(deckID, tri++, 0, 2, 1);
            i++;
        }
        while (i < ribs.Count - 1 - sternTri)
        {
            mesh.SetSubTri(deckID, tri++, (ushort)(vert), (ushort)(vert + 3), (ushort)(vert + 2));
            mesh.SetSubTri(deckID, tri++, (ushort)(vert), (ushort)(vert + 1), (ushort)(vert + 3));
            mesh.SetSubVert(deckID, vert++, ribs[i].deck, Vector2.zero, Vector2.zero);
            mesh.SetSubVert(deckID, vert++, Vector3.Scale(ribs[i].deck, leftScale), Vector2.zero, Vector2.zero);
            
            i++;
        }
        if (sternTri == 1)
        {
            mesh.SetSubTri(deckID, tri++, (ushort)(vert), (ushort)(vert + 1), (ushort)(vert + 2));
            mesh.SetSubVert(deckID, vert++, ribs[i].deck, Vector2.zero, Vector2.zero);
            mesh.SetSubVert(deckID, vert++, Vector3.Scale(ribs[i].deck, leftScale), Vector2.zero, Vector2.zero);
            i++;
            mesh.SetSubVert(deckID, vert++, ribs[i].deck, Vector2.zero, Vector2.zero);
        }
        else
        {
            mesh.SetSubVert(deckID, vert++, ribs[i].deck, Vector2.zero, Vector2.zero);
            mesh.SetSubVert(deckID, vert++, Vector3.Scale(ribs[i].deck, leftScale), Vector2.zero, Vector2.zero);
        }
    }
    void MeshHull()
    {
        int bowTris = 0, sternTris = 0;
        if (ribs[0].deck.x != 0f) bowTris += 1;
        if (ribs[0].wale.x != 0f) bowTris += 2;
        if (ribs[0].bilge.x != 0f) bowTris += 2;
        if (ribs[0].bottom.x != 0f) bowTris += 1;
        if (ribs[ribs.Count - 1].deck.x != 0f) sternTris += 1;
        if (ribs[ribs.Count - 1].wale.x != 0f) sternTris += 2;
        if (ribs[ribs.Count - 1].bilge.x != 0f) sternTris += 2;
        if (ribs[ribs.Count - 1].bottom.x != 0f) sternTris += 1;


        Vector3 leftScale = new Vector3(-1f, 1f, 1f);
        Vector2 plank = new Vector2(3f, 0f);
        // Set the vertex and tri counts
        int verts = ribs.Count * 12 + bowTris * 3 + sternTris * 3;
        int tris = (ribs.Count - 1) * 36 + bowTris * 3 + sternTris * 3;

        // clear preexisting mesh if it exists
        if (hullMeshMade) mesh.DeleteSubMesh((ushort)hullID);
        // reserve new mesh
        hullID = mesh.CreateSubMesh((ushort)(verts), (ushort)(tris));
        hullMeshMade = true;

        // index, vertex, and triangle iterators
        ushort i = 0;
        ushort vert = 0;
        ushort tri = 0;

        mesh.SetSubVert(hullID, vert++, ribs[i].deck, Vector2.zero, plank);
        mesh.SetSubVert(hullID, vert++, ribs[i].wale, Vector2.zero, plank);
        mesh.SetSubVert(hullID, vert++, ribs[i].wale, Vector2.zero, plank);
        mesh.SetSubVert(hullID, vert++, ribs[i].bilge, Vector2.zero, plank);
        mesh.SetSubVert(hullID, vert++, ribs[i].bilge, Vector2.zero, plank);
        mesh.SetSubVert(hullID, vert++, ribs[i].bottom, Vector2.zero, plank);
        mesh.SetSubVert(hullID, vert++, Vector3.Scale(ribs[i].deck, leftScale), Vector2.zero, plank);
        mesh.SetSubVert(hullID, vert++, Vector3.Scale(ribs[i].wale, leftScale), Vector2.zero, plank);
        mesh.SetSubVert(hullID, vert++, Vector3.Scale(ribs[i].wale, leftScale), Vector2.zero, plank);
        mesh.SetSubVert(hullID, vert++, Vector3.Scale(ribs[i].bilge, leftScale), Vector2.zero, plank);
        mesh.SetSubVert(hullID, vert++, Vector3.Scale(ribs[i].bilge, leftScale), Vector2.zero, plank);
        mesh.SetSubVert(hullID, vert++, Vector3.Scale(ribs[i].bottom, leftScale), Vector2.zero, plank);
        i++;
        while (i < ribs.Count)
        {
            // Left Side Verts
            mesh.SetSubVert(hullID, vert++, ribs[i].deck, Vector2.zero, plank);
            mesh.SetSubVert(hullID, vert++, ribs[i].wale, Vector2.zero, plank);
            mesh.SetSubVert(hullID, vert++, ribs[i].wale, Vector2.zero, plank);
            mesh.SetSubVert(hullID, vert++, ribs[i].bilge, Vector2.zero, plank);
            mesh.SetSubVert(hullID, vert++, ribs[i].bilge, Vector2.zero, plank);
            mesh.SetSubVert(hullID, vert++, ribs[i].bottom, Vector2.zero, plank);

            // Left Side Tris
            mesh.SetSubTri(hullID, tri++, (ushort)(vert - 1), (ushort)(vert - 14), (ushort)(vert - 2));
            mesh.SetSubTri(hullID, tri++, (ushort)(vert - 1), (ushort)(vert - 13), (ushort)(vert - 14));
            mesh.SetSubTri(hullID, tri++, (ushort)(vert - 3), (ushort)(vert - 16), (ushort)(vert - 4));
            mesh.SetSubTri(hullID, tri++, (ushort)(vert - 3), (ushort)(vert - 15), (ushort)(vert - 16));
            mesh.SetSubTri(hullID, tri++, (ushort)(vert - 5), (ushort)(vert - 18), (ushort)(vert - 6));
            mesh.SetSubTri(hullID, tri++, (ushort)(vert - 5), (ushort)(vert - 17), (ushort)(vert - 18));

            // Right Side Verts
            mesh.SetSubVert(hullID, vert++, Vector3.Scale(ribs[i].deck, leftScale), Vector2.zero, plank);
            mesh.SetSubVert(hullID, vert++, Vector3.Scale(ribs[i].wale, leftScale), Vector2.zero, plank);
            mesh.SetSubVert(hullID, vert++, Vector3.Scale(ribs[i].wale, leftScale), Vector2.zero, plank);
            mesh.SetSubVert(hullID, vert++, Vector3.Scale(ribs[i].bilge, leftScale), Vector2.zero, plank);
            mesh.SetSubVert(hullID, vert++, Vector3.Scale(ribs[i].bilge, leftScale), Vector2.zero, plank);
            mesh.SetSubVert(hullID, vert++, Vector3.Scale(ribs[i].bottom, leftScale), Vector2.zero, plank);

            // Right Side Tris
            mesh.SetSubTri(hullID, tri++, (ushort)(vert - 1), (ushort)(vert - 2), (ushort)(vert - 14));
            mesh.SetSubTri(hullID, tri++, (ushort)(vert - 1), (ushort)(vert - 14), (ushort)(vert - 13));
            mesh.SetSubTri(hullID, tri++, (ushort)(vert - 3), (ushort)(vert - 4), (ushort)(vert - 16));
            mesh.SetSubTri(hullID, tri++, (ushort)(vert - 3), (ushort)(vert - 16), (ushort)(vert - 15));
            mesh.SetSubTri(hullID, tri++, (ushort)(vert - 5), (ushort)(vert - 6), (ushort)(vert - 18));
            mesh.SetSubTri(hullID, tri++, (ushort)(vert - 5), (ushort)(vert - 18), (ushort)(vert - 17));

            i++;
        }

        if(ribs[0].deck.x != 0f)
        {
            mesh.SetSubVert(hullID, vert++, ribs[0].deck, Vector2.zero, plank);
            mesh.SetSubVert(hullID, vert++, Vector3.Scale(ribs[0].deck, leftScale), Vector2.zero, plank);
            if (ribs[0].wale.x == 0f)
            { 
                mesh.SetSubVert(hullID, vert++, ribs[0].wale, Vector2.zero, plank);
                mesh.SetSubTri(hullID, tri++, (ushort)(vert - 1), (ushort)(vert - 2), (ushort)(vert - 3));
            }
        }
        if (ribs[0].wale.x != 0f)
        {
            if (ribs[0].deck.x == 0f)
            {
                mesh.SetSubVert(hullID, vert++, ribs[0].deck, Vector2.zero, plank);
                mesh.SetSubTri(hullID, tri++, (ushort)(vert - 1), (ushort)(vert), (ushort)(vert + 1));
            }
            else
            {
                mesh.SetSubTri(hullID, tri++, (ushort)(vert - 2), (ushort)(vert + 1), (ushort)(vert - 1));
                mesh.SetSubTri(hullID, tri++, (ushort)(vert - 2), (ushort)(vert), (ushort)(vert + 1));
            }
            mesh.SetSubVert(hullID, vert++, ribs[0].wale, Vector2.zero, plank);
            mesh.SetSubVert(hullID, vert++, Vector3.Scale(ribs[0].wale, leftScale), Vector2.zero, plank);
            if (ribs[0].bilge.x == 0f)
            {
                mesh.SetSubVert(hullID, vert++, ribs[0].bilge, Vector2.zero, plank);
                mesh.SetSubTri(hullID, tri++, (ushort)(vert - 1), (ushort)(vert - 2), (ushort)(vert - 3));
            }
        }
        if (ribs[0].bilge.x != 0f)
        {
            if (ribs[0].wale.x == 0f)
            {
                mesh.SetSubVert(hullID, vert++, ribs[0].wale, Vector2.zero, plank);
                mesh.SetSubTri(hullID, tri++, (ushort)(vert - 1), (ushort)(vert), (ushort)(vert + 1));
            }
            else
            {
                mesh.SetSubTri(hullID, tri++, (ushort)(vert - 2), (ushort)(vert + 1), (ushort)(vert - 1));
                mesh.SetSubTri(hullID, tri++, (ushort)(vert - 2), (ushort)(vert), (ushort)(vert + 1));
            }
            mesh.SetSubVert(hullID, vert++, ribs[0].bilge, Vector2.zero, plank);
            mesh.SetSubVert(hullID, vert++, Vector3.Scale(ribs[0].bilge, leftScale), Vector2.zero, plank);
            if (ribs[0].bottom.x == 0f)
            {
                mesh.SetSubVert(hullID, vert++, ribs[0].bottom, Vector2.zero, plank);
                mesh.SetSubTri(hullID, tri++, (ushort)(vert - 1), (ushort)(vert - 2), (ushort)(vert - 3));
            }
        }
        if (ribs[0].bottom.x != 0f)
        {
            if (ribs[0].bilge.x == 0f)
            {
                mesh.SetSubVert(hullID, vert++, ribs[0].bilge, Vector2.zero, plank);
                mesh.SetSubTri(hullID, tri++, (ushort)(vert - 1), (ushort)(vert), (ushort)(vert + 1));
            }
            else
            {
                mesh.SetSubTri(hullID, tri++, (ushort)(vert - 2), (ushort)(vert + 1), (ushort)(vert - 1));
                mesh.SetSubTri(hullID, tri++, (ushort)(vert - 2), (ushort)(vert), (ushort)(vert + 1));
            }
            mesh.SetSubVert(hullID, vert++, ribs[0].bottom, Vector2.zero, plank);
            mesh.SetSubVert(hullID, vert++, Vector3.Scale(ribs[0].bottom, leftScale), Vector2.zero, plank);
        }
        // STERN
        int sternRib = ribs.Count - 1;
        if (ribs[sternRib].deck.x != 0f)
        {
            mesh.SetSubVert(hullID, vert++, Vector3.Scale(ribs[sternRib].deck, leftScale), Vector2.zero, plank);
            mesh.SetSubVert(hullID, vert++, ribs[sternRib].deck, Vector2.zero, plank);
            if (ribs[sternRib].wale.x == 0f)
            {
                mesh.SetSubVert(hullID, vert++, ribs[0].wale, Vector2.zero, plank);
                mesh.SetSubTri(hullID, tri++, (ushort)(vert - 1), (ushort)(vert - 2), (ushort)(vert - 3));
            }
        }
        if (ribs[sternRib].wale.x != 0f)
        {
            if (ribs[sternRib].deck.x == 0f)
            {
                mesh.SetSubVert(hullID, vert++, ribs[sternRib].deck, Vector2.zero, plank);
                mesh.SetSubTri(hullID, tri++, (ushort)(vert - 1), (ushort)(vert), (ushort)(vert + 1));
            }
            else
            {
                mesh.SetSubTri(hullID, tri++, (ushort)(vert - 2), (ushort)(vert + 1), (ushort)(vert - 1));
                mesh.SetSubTri(hullID, tri++, (ushort)(vert - 2), (ushort)(vert), (ushort)(vert + 1));
            }
            mesh.SetSubVert(hullID, vert++, Vector3.Scale(ribs[sternRib].wale, leftScale), Vector2.zero, plank);
            mesh.SetSubVert(hullID, vert++, ribs[sternRib].wale, Vector2.zero, plank);
            if (ribs[sternRib].bilge.x == 0f)
            {
                mesh.SetSubVert(hullID, vert++, ribs[sternRib].bilge, Vector2.zero, plank);
                mesh.SetSubTri(hullID, tri++, (ushort)(vert - 1), (ushort)(vert - 2), (ushort)(vert - 3));
            }
        }
        if (ribs[sternRib].bilge.x != 0f)
        {
            if (ribs[sternRib].wale.x == 0f)
            {
                mesh.SetSubVert(hullID, vert++, ribs[sternRib].wale, Vector2.zero, plank);
                mesh.SetSubTri(hullID, tri++, (ushort)(vert - 1), (ushort)(vert), (ushort)(vert + 1));
            }
            else
            {
                mesh.SetSubTri(hullID, tri++, (ushort)(vert - 2), (ushort)(vert + 1), (ushort)(vert - 1));
                mesh.SetSubTri(hullID, tri++, (ushort)(vert - 2), (ushort)(vert), (ushort)(vert + 1));
            }
            mesh.SetSubVert(hullID, vert++, Vector3.Scale(ribs[sternRib].bilge, leftScale), Vector2.zero, plank);
            mesh.SetSubVert(hullID, vert++, ribs[sternRib].bilge, Vector2.zero, plank);
            if (ribs[sternRib].bottom.x == 0f)
            {
                mesh.SetSubVert(hullID, vert++, ribs[sternRib].bottom, Vector2.zero, plank);
                mesh.SetSubTri(hullID, tri++, (ushort)(vert - 1), (ushort)(vert - 2), (ushort)(vert - 3));
            }
        }
        if (ribs[sternRib].bottom.x != 0f)
        {
            if (ribs[sternRib].bilge.x == 0f)
            {
                mesh.SetSubVert(hullID, vert++, ribs[sternRib].bilge, Vector2.zero, plank);
                mesh.SetSubTri(hullID, tri++, (ushort)(vert - 1), (ushort)(vert), (ushort)(vert + 1));
            }
            else
            {
                mesh.SetSubTri(hullID, tri++, (ushort)(vert - 2), (ushort)(vert + 1), (ushort)(vert - 1));
                mesh.SetSubTri(hullID, tri++, (ushort)(vert - 2), (ushort)(vert), (ushort)(vert + 1));
            }
            mesh.SetSubVert(hullID, vert++, Vector3.Scale(ribs[sternRib].bottom, leftScale), Vector2.zero, plank);
            mesh.SetSubVert(hullID, vert++, ribs[sternRib].bottom, Vector2.zero, plank);
        }

        //mesh.SetSubTri(id, tri++, (ushort)(vert - 1), (ushort)(vert - 3), (ushort)(vert - 2));
    }
}
