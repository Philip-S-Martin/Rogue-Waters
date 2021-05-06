using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MeshObject
{
    protected ScalableMesh mesh;
    public MeshObject()
    {
        mesh = new ScalableMesh();
    }
    public void UpdateMeshes(GameObject gameObject) => mesh.PopulateGameObject(gameObject);
}
