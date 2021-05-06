using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ShipGen : MonoBehaviour
{
    
    public ShipData data;
    List<ShipSource> sources;
    MeshObjectMap<ShipSource> mappedSources;
    // Start is called before the first frame update
    void Start()
    {
        Generate();
    }

    [ContextMenu("Refresh Boat")]
    void RefreshButton()
    {
        Debug.Log("Refreshing Boat Mesh");
        Generate();
    }

    void Generate()
    {
        if (mappedSources != null) mappedSources.Clear();
        if (sources != null) sources.Clear();
        // Prepare data repos
        sources = new List<ShipSource>();
        mappedSources = new MeshObjectMap<ShipSource>();

        // Add data
        sources.Add(new ShipSource(data));
        
        // Generate Object and Mesh
        foreach (ShipSource source in sources)
        {
            mappedSources.NewObject(source, new Vector3(0f, 0f, 0f));
            source.Generate();
        }

        // Apply meshes to objects
        if (mappedSources != null)
            mappedSources.UpdateAllMeshes();
    }

    private void OnDestroy()
    {
        if(mappedSources != null)
            mappedSources.Clear();
    }
}
