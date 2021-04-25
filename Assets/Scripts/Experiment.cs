using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Experiment : MonoBehaviour
{
    public Polyhedra polyhedra;
    private Simulation sim;
    public void Awake()
    {
        sim = GetComponent<Simulation>();
    }
    // Start is called before the first frame update
    private void Start()
    {
        sim.pingEvent.AddListener(polyhedra.GetComponent<VertexHighlighter>().HighlightVertex);
    }

    public void StartExperiment()
    {
        sim.SetPropagator(new DecayingWavePropagator(polyhedra.GetCompressedMesh(), (float)sim.speed));
        sim.StartSimulation(0);
    }
}
