using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using WavePropagation;

public class Polyhedra : MonoBehaviour
{
    public UnityEvent onMeshSet;
    public float boxStretch;
    private Mesh compressedMesh;
    private HashSet<int>[] vertexToAdjacent;
    private void SetLocalTransform() {
        Vector3 center = Utils.Center(GetComponent<MeshFilter>().mesh);
        transform.localPosition = center;
        
        float scale = boxStretch / Utils.MaxDimension(GetComponent<MeshFilter>().mesh);
        transform.localScale = new Vector3(scale, scale, scale);
    }
    private void UpdateCompressedMesh()
    {
        Mesh mesh = new Mesh();
        var vertices = (Vector3[])GetMesh().vertices.Clone();
        for (int i = 0; i < vertices.Length; ++i)
            vertices[i] = transform.TransformPoint(vertices[i]);
        mesh.vertices = vertices;
        mesh.triangles = GetComponent<MeshFilter>().mesh.triangles;
        compressedMesh = Utils.CompressMesh(mesh);
        InitAdjacency();
    }
    public void SetMesh(Mesh mesh)
    {
        GetComponent<MeshFilter>().mesh = mesh;
        SetLocalTransform();
        UpdateCompressedMesh();
        onMeshSet.Invoke();
    }

    private void Start()
    {
        SetMesh(GameObject.Find("MeshSelector").GetComponent<MeshSelector>().GetSelectedMesh());
    }
    private void InitAdjacency()
    {
        vertexToAdjacent = Enumerable.Range(0, compressedMesh.vertexCount).Select(_ => new HashSet<int>()).ToArray();
        for (int i = 0; i < compressedMesh.triangles.Length / 3; i++)
        {
            for (int j = i * 3; j < i * 3 + 3; ++j)
            {
                for (int k = j + 1; k < i * 3 + 3; ++k)
                {
                    vertexToAdjacent[compressedMesh.triangles[j]].Add(compressedMesh.triangles[k]);
                    vertexToAdjacent[compressedMesh.triangles[k]].Add(compressedMesh.triangles[j]);
                }
            }
        }
    }
    public Mesh GetCompressedMesh()
    {
        return compressedMesh;
    }
    public Mesh GetMesh()
    {
        return GetComponent<MeshFilter>().mesh;
    }

    public IEnumerable<int> GetAdjacentVertices(int vertex)
    {
        return vertexToAdjacent[vertex];
    }
}
