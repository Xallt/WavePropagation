using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimulationController : MonoBehaviour
{
    public PingEvent onSimulationPing;
    public float speed;
    public float time;
    private Polyhedra polyhedra;
    private Simulation sim;
    public void Awake()
    {
        polyhedra = GameObject.Find("Polyhedra").GetComponent<Polyhedra>();
        onSimulationPing = new PingEvent();
    }
    private void OnEnable()
    {
        onSimulationPing.AddListener(polyhedra.GetComponent<VertexHighlighter>().HighlightVertex);
    }
    private void PrepareSimulation()
    {
        sim.onPing.AddListener(onSimulationPing.Invoke);
    }

    public void StartDecayingWaveExperiment()
    {
        sim = new MultiplePingSimulation(
            speed, time, polyhedra.GetCompressedMesh(),
            new DecayingWavePropagator(polyhedra.GetCompressedMesh(), speed)
        );
        PrepareSimulation();
        StartCoroutine(sim.StartSimulation());
    }
    public void StartClassicWaveExperiment()
    {
        var polyVertices = polyhedra.GetCompressedMesh().vertices;
        float polySide = (polyVertices[1] - polyVertices[0]).magnitude;
        float waveTimeToLive = GameObject.Find("TimeToLive").GetComponentInChildren<Slider>().value;

        sim = new SinglePingSimulation(
            speed, time, polyhedra.GetCompressedMesh(),
            new ClassicWaveTetrahedronPropagator(
                polySide,
                speed,
                waveTimeToLive
            )
        );
        PrepareSimulation();
        StartCoroutine(sim.StartSimulation());
    }
}
