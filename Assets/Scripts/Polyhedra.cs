using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Polyhedra : MonoBehaviour
{
    public GameObject hightlightSpherePrefab;
    private Mesh compressedMesh;
    private Dictionary<int, GameObject> vertexHighlightSpheres;
    void Start()
    {
        compressedMesh = WavePropagation.Utils.CompressMesh(GetComponent<MeshFilter>().mesh);
        vertexHighlightSpheres = new Dictionary<int, GameObject>();
    }

    public Mesh GetMesh()
    {
        return compressedMesh;
    }

    public void HighlightVertex(int vertex)
    {
        if (vertexHighlightSpheres.ContainsKey(vertex))
            return;
        Vector3 pos = transform.TransformPoint(compressedMesh.vertices[vertex]);

        GameObject sphere = Instantiate(hightlightSpherePrefab, pos, Quaternion.identity);
        sphere.transform.SetParent(transform);
        vertexHighlightSpheres[vertex] = sphere;
    }
    public void DisableHighlightVertex(int vertex)
    {
        if (!vertexHighlightSpheres.ContainsKey(vertex))
            return;
        GameObject sphere = vertexHighlightSpheres[vertex];
        vertexHighlightSpheres.Remove(vertex);
        Destroy(sphere);
    }
}
