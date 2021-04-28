using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public void StartDecayingWaveExperiment()
    {
        sim.SetPropagator(new DecayingWavePropagator(polyhedra.GetCompressedMesh(), (float)sim.speed));
        sim.StartSimulation(0);
    }
    public void StartClassicWaveExperiment()
    {
        var polyVertices = polyhedra.GetCompressedMesh().vertices;
        float polySide = (polyVertices[1] - polyVertices[0]).magnitude;
        float waveTimeToLive = GameObject.Find("TimeToLive").GetComponentInChildren<Slider>().value;

        sim.SetPropagator(new ClassicWaveTetrahedronPropagator(
            polySide, 
            (float)sim.speed,
            waveTimeToLive
        ));
        sim.StartSimulation(0);
    }
}
