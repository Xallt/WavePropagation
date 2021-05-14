using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.Tween;

public class VertexHighlighter : MonoBehaviour
{
    public GameObject hightlightSpherePrefab;
    public float fadeOutDuration = 1.0f;
    public float relativeInitialScale;
    private Polyhedra polyhedra;
    private SimulationController simulationController;
    private Mesh polyhedraCompressedMesh;
    private Dictionary<int, GameObject> vertexHighlightSpheres;
    private float scaleDeclineSpeed;
    private float initialSphereScale;
    private void Awake()
    {
        polyhedra = GetComponent<Polyhedra>();
        vertexHighlightSpheres = new Dictionary<int, GameObject>();
        simulationController = GameObject.Find("App").GetComponent<SimulationController>();
    }
    private void OnEnable()
    {
        polyhedra.onMeshSet.AddListener(InitializeHighlighters);
    }
    void InitializeHighlighters()
    {
        foreach (var sphere in vertexHighlightSpheres.Values)
        {
            Destroy(sphere);
        }
        vertexHighlightSpheres.Clear();
        polyhedraCompressedMesh = polyhedra.GetCompressedMesh();
        float compressedMeshSize = WavePropagation.Utils.MaxDimension(polyhedra.GetMesh());

        initialSphereScale = compressedMeshSize * relativeInitialScale;

        for (int i = 0; i < polyhedraCompressedMesh.vertices.Length; ++i)
        {
            GameObject sphere = Instantiate(hightlightSpherePrefab);
            sphere.transform.SetParent(transform);
            sphere.transform.position = polyhedraCompressedMesh.vertices[i];
            sphere.transform.localScale = new Vector3(0, 0, 0);
            vertexHighlightSpheres.Add(i, sphere);
        }
        scaleDeclineSpeed = initialSphereScale / fadeOutDuration;
    }
    public void HighlightVertex(int vertex)
    {
        vertexHighlightSpheres[vertex].transform.localScale = Vector3.one * initialSphereScale;
    }
    public void Update()
    {
        foreach (var sphere in vertexHighlightSpheres.Values)
        {
            float currentScale = sphere.transform.localScale.x;
            sphere.transform.localScale = Vector3.one * Mathf.Max(0, currentScale - Time.deltaTime * scaleDeclineSpeed);
        }
    }
}
