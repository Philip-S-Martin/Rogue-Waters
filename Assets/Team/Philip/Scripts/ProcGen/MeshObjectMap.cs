using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshObjectMap<T> where T : MeshObject
{
    Dictionary<T, GameObject> meshToGame;
    Dictionary<GameObject, T> gameToMesh;
    public MeshObjectMap()
    {
        meshToGame = new Dictionary<T, GameObject>();
        gameToMesh = new Dictionary<GameObject, T>();
    }
    public bool NewObject(T meshObject, Vector3 position)
    {
        if (meshToGame.ContainsKey(meshObject))
            return false;
        GameObject gameObject = new GameObject("ProceduralObject");
        gameObject.transform.position = position;
        meshToGame.Add(meshObject, gameObject);
        gameToMesh.Add(gameObject, meshObject);
        meshObject.UpdateMeshes(gameObject);
        return true;
    }
    public void Remove(T meshObject)
    {
        Remove(GetMapped(meshObject), meshObject);
    }
    public void Remove(GameObject gameObject)
    {
        Remove(gameObject, GetMapped(gameObject));
    }
    private void Remove(GameObject gameObject, T meshObject)
    {
        meshToGame.Remove(meshObject);
        gameToMesh.Remove(gameObject);
        Object.Destroy(gameObject);
    }
    public GameObject GetMapped(T meshObject)
    {
        return meshToGame[meshObject];
    }
    public T GetMapped(GameObject gameObject)
    {
        return gameToMesh[gameObject];
    }
    public void UpdateMesh(GameObject gameObject)
    {
        UpdateMesh(gameObject, GetMapped(gameObject));
    }
    public void UpdateMesh(T meshObject)
    {
        UpdateMesh(GetMapped(meshObject), meshObject);
    }
    private void UpdateMesh(GameObject gameObject, T meshObject)
    {
        meshObject.UpdateMeshes(gameObject);
    }
    public void UpdateAllMeshes()
    {
        foreach(KeyValuePair<T, GameObject> kvp in meshToGame)
        {
            kvp.Key.UpdateMeshes(kvp.Value);
        }
    }
    public void Clear()
    {
        foreach (KeyValuePair<GameObject, T> pair in gameToMesh)
        {
            if (pair.Key) Object.DestroyImmediate(pair.Key);
        }
        gameToMesh.Clear();
        meshToGame.Clear();
    }
}
