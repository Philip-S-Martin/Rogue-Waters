using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSource : MeshObject
{
    public void GenerateRange(Vector3 center, float size, int resolution, Material material, bool skirt = false)
    {
        mesh.MeshMaterial = material;
        int vertDimension, squareDimension;
        if (skirt)
            squareDimension = resolution + 2;
        else
            squareDimension = resolution;
        vertDimension = squareDimension + 1; ;

        ushort id = mesh.CreateSubMesh((ushort)(vertDimension * vertDimension), (ushort)(squareDimension * squareDimension * 6));

        Debug.Log($"SquareDim = {squareDimension}, VertDim = {vertDimension}");

        ushort vert = 0;
        ushort skirtborder = (ushort)(vertDimension - 1);
        float minCoord = -size / 2f;
        Vector3 vertPos = new Vector3(minCoord, 0f, minCoord);
        for (ushort i = 0; i < vertDimension; i++)
        {
            vertPos.x = minCoord + ((float)i / (float)squareDimension) * size;
            for (ushort j = 0; j < vertDimension; j++)
            {
                vertPos.y = Convert.ToSingle(skirt & (i == 0 | j == 0 | i == skirtborder | j == skirtborder)) * -0.125f;
                vertPos.z = minCoord + ((float)j / (float)squareDimension) * size;
                mesh.SetSubVert(id, vert++, vertPos, Vector2.zero, Vector2.zero);
            }
        }

        ushort tri = 0;
        for (ushort i = 0; i < squareDimension; i++)
        {
            for (ushort j = 0; j < squareDimension; j++)
            {
                ushort j0Vert = (ushort)(i * vertDimension + j);
                ushort j1Vert = (ushort)(j0Vert + vertDimension);
                mesh.SetSubTri(id, tri++, j0Vert, (ushort)(j0Vert + 1), j1Vert);
                mesh.SetSubTri(id, tri++, (ushort)(j0Vert + 1), (ushort)(j1Vert + 1), j1Vert);
            }
        }
    }
}
