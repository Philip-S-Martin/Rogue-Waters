using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalableMesh
{
    // This class will organize an addressable of static meshes (submeshes) into a large mesh
    // data buffer (verts, tris, uvs, etc), then it can pass regions of this buffer to gameObjects
    public struct SubMesh
    {
        public ushort vertBase, triBase;
        public ushort vertCount, triCount;
        public SubMesh(ushort vertBase, ushort triBase, ushort vertCount, ushort triCount)
        {
            this.vertBase = vertBase;
            this.triBase = triBase;
            this.vertCount = vertCount;
            this.triCount = triCount;
        }
        public SubMesh ResizeBuffers(ushort newVertCount, ushort newTriCount)
        {
            this.vertCount = newVertCount;
            this.triCount = newTriCount;
            return this;
        }
    }
    public class SubDiv
    {
        public ushort SubMeshBaseIdx { get; private set; }
        public List<Vector3> Verts { get; private set; }
        public List<Vector3> Normals { get; private set; }
        public List<Vector2> TextureUVs { get; private set; }
        public List<Vector2> SubmaterialUVs { get; private set; }
        public List<ushort> Tris { get; private set; }
        public List<SubMesh> SubMeshes { get; private set; }
        public List<ushort> FragmentedSubs { get; private set; }
        private ushort lastVert, lastTri;
        public SubDiv(ushort SubMeshBaseIdx)
        {
            this.SubMeshBaseIdx = SubMeshBaseIdx;
            Verts = new List<Vector3>();
            Normals = new List<Vector3>();
            TextureUVs = new List<Vector2>();
            SubmaterialUVs = new List<Vector2>();
            Tris = new List<ushort>();
            SubMeshes = new List<SubMesh>();
            FragmentedSubs = new List<ushort>();
            lastVert = 0;
            lastTri = 0;
        }
        public void SetSubVert(ushort subMeshId, ushort i, Vector3 vert, Vector2 texUV, Vector2 submatUV)
        {
            Verts[SubMeshes[subMeshId - SubMeshBaseIdx].vertBase + i] = vert;
            TextureUVs[SubMeshes[subMeshId - SubMeshBaseIdx].vertBase + i] = texUV;
            SubmaterialUVs[SubMeshes[subMeshId - SubMeshBaseIdx].vertBase + i] = submatUV;
        }
        public void SetSubTri(ushort subMeshId, ushort i, ushort a, ushort b, ushort c)
        {
            SubMesh subMesh = SubMeshes[subMeshId - SubMeshBaseIdx];
            int triOffset = subMesh.triBase + i * 3;
            Tris[triOffset] = (ushort)(subMesh.vertBase + a);
            Tris[triOffset + 1] = (ushort)(subMesh.vertBase + b);
            Tris[triOffset + 2] = (ushort)(subMesh.vertBase + c);
        }
        public bool TryCreateSubMesh(ushort vertsToReserve, ushort trisToReserve, out ushort index)
        {
            if (TryFindBestFragment(vertsToReserve, trisToReserve, out ushort fragmentID))
            {
                CreateInFragment(fragmentID, vertsToReserve, trisToReserve);
                FragmentedSubs.Remove(fragmentID);
                index = (ushort)(fragmentID + SubMeshBaseIdx);
                return true;
            }
            else
            {
                if (SubMeshes.Count >= 1 && (vertsToReserve + lastVert > 65536 || trisToReserve + lastTri > 65536))
                {
                    index = 0;
                    return false;
                }
                else
                {
                    index = CreateFreshSubMesh(vertsToReserve, trisToReserve);
                    return true;
                }
            }
        }
        private bool TryFindBestFragment(ushort vertsToReserve, ushort trisToReserve, out ushort fragmentID)
        {
            fragmentID = 0;
            SubMesh bestFragment = new SubMesh(0, 0, 65535, 65535);
            for (int i = 0; i < FragmentedSubs.Count; i++)
            {
                SubMesh fragment = SubMeshes[FragmentedSubs[i]];
                if (fragment.vertCount >= vertsToReserve && fragment.triCount >= trisToReserve)
                {
                    if (fragment.vertCount + fragment.triCount < bestFragment.vertCount + bestFragment.triCount)
                    {
                        fragmentID = FragmentedSubs[i];
                        bestFragment = fragment;
                        if (bestFragment.vertCount == vertsToReserve && bestFragment.triCount == vertsToReserve)
                            return true;
                    }
                }
            }
            if (bestFragment.vertCount == 65535 && bestFragment.triCount == 65535)
                return false;
            else return true;
        }
        public bool TryFindReplacement(SubMesh toReplace, out ushort replacementIdx, out SubMesh replacement)
        {
            replacementIdx = 0;
            replacement = new SubMesh();
            for (int i = (ushort)(SubMeshes.Count - 1); i >= 0; i--)
            {
                if (!FragmentedSubs.Contains((ushort)i))
                {
                    replacement = SubMeshes[i];
                    if (replacement.vertCount <= toReplace.vertCount && replacement.triCount <= toReplace.triCount)
                    {
                        replacementIdx = (ushort)(i + SubMeshBaseIdx);
                        return true;
                    }
                }
            }
            return false;
        }
        public SubMesh GetSubMesh(ushort idx)
        {
            return SubMeshes[idx - SubMeshBaseIdx];
        }
        public void SetSubMesh(ushort idx, SubMesh subMesh)
        {
            SubMeshes[idx - SubMeshBaseIdx] = subMesh;
        }
        public ushort CreateFreshSubMesh(ushort vertsToReserve, ushort trisToReserve)
        {
            // create custom List<> implementation with NativeArray that supports growth without assignment!
            SubMesh subMesh = new SubMesh((ushort)Verts.Count, (ushort)Tris.Count, vertsToReserve, trisToReserve);
            lastVert += vertsToReserve;
            lastTri += trisToReserve;
            SubMeshes.Add(subMesh);
            for (int i = 0; i < vertsToReserve; i++)
            {
                Verts.Add(Vector3.zero); // this is multiple assignment! BAD!
                Normals.Add(Vector3.up);
                TextureUVs.Add(Vector2.zero);
                SubmaterialUVs.Add(Vector2.zero);
            }
            for (int i = 0; i < trisToReserve; i++)
            {
                Tris.Add(0); // more evil multiple assignment!
            }
            return (ushort)(SubMeshes.Count + SubMeshBaseIdx - 1);
        }
        private void CreateInFragment(ushort subMeshID, ushort vertsToReserve, ushort trisToReserve)
        {
            SubMesh subMesh = SubMeshes[subMeshID].ResizeBuffers(vertsToReserve, trisToReserve);
            FragmentedSubs.Remove(subMeshID);
            SubMeshes[subMeshID] = subMesh;
        }
        public void DeleteSubMesh(ushort subMeshId)
        {
            subMeshId -= SubMeshBaseIdx;
            SubMesh subMesh = SubMeshes[subMeshId];
            FragmentedSubs.Add(subMeshId);
            if (subMeshId < SubMeshes.Count - 1)
            {
                SubMesh next = SubMeshes[subMeshId + 1];
                SubMeshes[subMeshId] = subMesh.ResizeBuffers((ushort)(next.vertBase - subMesh.vertBase), (ushort)(next.triBase - subMesh.triBase));
            }
            for (int i = subMesh.vertBase; i < subMesh.vertBase + subMesh.vertCount; i++)
                Verts[i] = Vector3.zero;
            for (int i = subMesh.triBase; i < subMesh.triBase + subMesh.triCount; i++)
                Tris[i] = subMesh.vertBase;
        }
        public void RecalculateNormals()
        {
            for (int i = 0; i < Normals.Count; i++)
                Normals[i] = Vector3.zero;
            for (int i = 0; i < Tris.Count; i += 3)
            {
                Vector3 norm = Vector3.Cross(Verts[Tris[i + 1]] - Verts[Tris[i]], Verts[Tris[i + 2]] - Verts[Tris[i]]);
                Normals[Tris[i]] += norm;
                Normals[Tris[i + 1]] += norm;
                Normals[Tris[i + 2]] += norm;
            }
            for (int i = 0; i < Normals.Count; i++)
                Normals[i] = Normals[i].normalized;
        }
    }

    private List<SubDiv> subDivs;
    private ushort selectedSubMeshIdx;
    private Material meshMaterial;
    public Material MeshMaterial { get => meshMaterial; set => meshMaterial = value; }
    private SubDiv selectedSubDiv;
    public ScalableMesh()
    {
        subDivs = new List<SubDiv>();
        subDivs.Add(new SubDiv(0));
        MeshMaterial = Resources.Load("Materials/Plain/White") as Material;
    }
    public void SetSubVert(ushort subMeshId, ushort i, Vector3 vert, Vector2 texUV, Vector2 submatUV) => GetSubDiv(subMeshId).SetSubVert(subMeshId, i, vert, texUV, submatUV);
    public void SetSubTri(ushort subMeshId, ushort i, ushort a, ushort b, ushort c) => GetSubDiv(subMeshId).SetSubTri(subMeshId, i, a, b, c);
    public ushort CreateSubMesh(ushort reserveVerts, ushort reserveTris)
    {
        foreach (SubDiv subDiv in subDivs)
        {
            if (subDiv.TryCreateSubMesh(reserveVerts, reserveTris, out selectedSubMeshIdx))
            {
                selectedSubDiv = new SubDiv((ushort)(subDivs[subDivs.Count - 1].SubMeshBaseIdx + subDivs[subDivs.Count - 1].SubMeshes.Count));
                return selectedSubMeshIdx;
            }
        }
        if (subDivs.Count == 0)
            selectedSubDiv = new SubDiv((ushort)(0)); 
        else selectedSubDiv = new SubDiv((ushort)(subDivs[subDivs.Count - 1].SubMeshBaseIdx + subDivs[subDivs.Count - 1].SubMeshes.Count));
        subDivs.Add(selectedSubDiv);
        selectedSubDiv.TryCreateSubMesh(reserveVerts, reserveTris, out selectedSubMeshIdx);
        return selectedSubMeshIdx;
    }
    public ushort MoveFromScalableMesh(ScalableMesh fromMesh, ushort fromIdx)
    {
        SubDiv fromDiv = fromMesh.GetSubDiv(fromIdx);
        SubMesh fromSubMesh = fromDiv.GetSubMesh(fromIdx);
        ushort toIdx = CreateSubMesh(fromSubMesh.vertCount, fromSubMesh.triCount);
        SubDiv toDiv = GetSubDiv(toIdx);
        SubMesh toSubMesh = toDiv.GetSubMesh(toIdx);
        for (int i = 0; i < fromSubMesh.vertCount; i++)
        {
            toDiv.Verts[toSubMesh.vertBase + i] = fromDiv.Verts[fromSubMesh.vertBase + i];
            toDiv.Normals[toSubMesh.vertBase + i] = fromDiv.Normals[fromSubMesh.vertBase + i];
            toDiv.TextureUVs[toSubMesh.vertBase + i] = fromDiv.TextureUVs[fromSubMesh.vertBase + i];
            toDiv.SubmaterialUVs[toSubMesh.vertBase + i] = fromDiv.SubmaterialUVs[fromSubMesh.vertBase + i];
        }
        int baseDiff = toSubMesh.vertBase - fromSubMesh.vertBase;
        for (int i = 0; i < fromSubMesh.triCount; i++)
        {
            toDiv.Tris[toSubMesh.triBase + i] = (ushort)(fromDiv.Tris[fromSubMesh.triBase + i] + baseDiff);
        }
        fromDiv.DeleteSubMesh(fromIdx);
        return toIdx;
    }
    public void DeleteSubMesh(ushort subMeshID) => GetSubDiv(subMeshID).DeleteSubMesh(subMeshID);
    public bool DeleteSubMeshSwapTrick(ushort subMeshID, out ushort swappedID)
    {
        swappedID = 0;
        SubDiv deleteFromDiv = GetSubDiv(subMeshID);
        SubMesh deleteSubMesh = deleteFromDiv.GetSubMesh(subMeshID);
        for (int i = subDivs.Count - 1; i >= 0; i--)
        {
            if (subDivs[i].TryFindReplacement(deleteSubMesh, out swappedID, out SubMesh replacement))
            {
                deleteFromDiv.SetSubMesh(subMeshID, deleteSubMesh.ResizeBuffers(replacement.vertCount, replacement.triCount));
                for (int j = 0; j < replacement.vertCount; j++)
                {
                    deleteFromDiv.Verts[deleteSubMesh.vertBase + j] = subDivs[i].Verts[replacement.vertBase + j];
                    deleteFromDiv.Normals[deleteSubMesh.vertBase + j] = subDivs[i].Normals[replacement.vertBase + j];
                    deleteFromDiv.TextureUVs[deleteSubMesh.vertBase + j] = subDivs[i].TextureUVs[replacement.vertBase + j];
                    deleteFromDiv.SubmaterialUVs[deleteSubMesh.vertBase + j] = subDivs[i].SubmaterialUVs[replacement.vertBase + j];
                }
                int baseDiff = deleteSubMesh.vertBase - replacement.vertBase;
                for (int j = 0; j < replacement.triCount; j++)
                {
                    deleteFromDiv.Tris[deleteSubMesh.triBase + j] = (ushort)(subDivs[i].Tris[replacement.triBase + j] + baseDiff);
                }
                DeleteSubMesh(swappedID);
                return true;
            }
        }
        DeleteSubMesh(subMeshID);
        return false;
    }
    public SubDiv GetSubDiv(ushort subMeshID)
    {
        foreach (SubDiv subDiv in subDivs)
        {
            if (subMeshID >= subDiv.SubMeshBaseIdx && subMeshID < subDiv.SubMeshBaseIdx + subDiv.SubMeshes.Count)
                return subDiv;
        }
        throw new System.Exception();
    }
    public void RecalculateAllNormals()
    {
        foreach (SubDiv subDiv in subDivs)
        {
            subDiv.RecalculateNormals();
        }
    }
    public void CullExtraSubDivs()
    {
        for (int i = subDivs.Count - 1; i >= 0; i--)
        {
            if (subDivs[i].SubMeshes.Count == subDivs[i].FragmentedSubs.Count)
                subDivs.Remove(subDivs[i]);
        }
    }
    public void PopulateGameObject(GameObject gameObject, bool recalculateNormals = false)
    {
        CullExtraSubDivs();
        GameObject[] gameObjects = new GameObject[subDivs.Count];
        if (subDivs.Count > 0) gameObjects[0] = gameObject;

        int child = gameObject.transform.childCount;
        while (child > subDivs.Count)
        {
            GameObject childObject = gameObject.transform.GetChild(child).gameObject;
            if (!childObject.TryGetComponent<MeshFilter>(out MeshFilter mf))
            {
                if (mf.mesh != null) Object.Destroy(mf.mesh);
            }
            Object.Destroy(childObject);
            child--;
        }


        int i = 1;
        while (i < child + 1)
        {
            gameObjects[i] = gameObject.transform.GetChild(i - 1).gameObject;
            i++;
        }
        while (i < gameObjects.Length)
        {
            GameObject newObject = new GameObject("ProceduralChild_" + i);
            newObject.transform.parent = gameObject.transform;
            newObject.transform.localPosition = Vector3.zero;
            gameObjects[i] = newObject;
            i++;
        }
        i = 0;
        while (i < gameObjects.Length)
        {

            AssignSubDivToGameObject(gameObjects[i], subDivs[i], recalculateNormals);
            i++;
        }
    }

    private void AssignSubDivToGameObject(GameObject gameObject, SubDiv subDiv, bool recalculateNormals = false)
    {
        MeshFilter mf;
        if (!gameObject.TryGetComponent<MeshFilter>(out mf))
        {
            mf = gameObject.AddComponent<MeshFilter>();
        }
        if (!gameObject.TryGetComponent<MeshRenderer>(out MeshRenderer mr))
        {
            gameObject.AddComponent<MeshRenderer>().material = MeshMaterial;
        }
        else
        {
            if (mr.sharedMaterial != meshMaterial) mr.material = meshMaterial;
        }
        if (recalculateNormals)
        {
            subDiv.RecalculateNormals();
        }
        if (mf.sharedMesh == null) mf.sharedMesh = new Mesh();
        mf.sharedMesh.SetVertices(subDiv.Verts);
        mf.sharedMesh.SetTriangles(subDiv.Tris, 0);
        mf.sharedMesh.SetNormals(subDiv.Normals);
        mf.sharedMesh.SetUVs(0, subDiv.TextureUVs);
        mf.sharedMesh.SetUVs(1, subDiv.SubmaterialUVs);
    }
}
