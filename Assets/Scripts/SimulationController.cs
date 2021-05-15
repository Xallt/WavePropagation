using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class SimulationController : MonoBehaviour
{
    public PingEvent onSimulationPing = new PingEvent();
    public UnityEvent onSimulationStarted = new UnityEvent();
    public UnityEvent onSimulationEnded = new UnityEvent();
    [HideInInspector]
    public float speed;
    [HideInInspector]
    public float time;
    private Polyhedra polyhedra;
    private Simulation sim;
    public void Awake()
    {
        polyhedra = GameObject.Find("Polyhedra").GetComponent<Polyhedra>();
    }
    private void OnEnable()
    {
        onSimulationPing.AddListener(polyhedra.GetComponent<VertexHighlighter>().HighlightVertex);
    }
    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }
    public void SetTime(float time)
    {
        this.time = time;
    }
    private void PrepareSimulation()
    {
        sim.onPing.AddListener(onSimulationPing.Invoke);
    }
    private IEnumerator CurrentSimulationCoroutine()
    {
        yield return sim.StartSimulation();
        onSimulationEnded.Invoke();
    }

    public void StartDecayingWaveExperiment()
    {
        sim = new MultiplePingSimulation(
            speed, time, polyhedra.GetCompressedMesh(),
            new DecayingWavePropagator(polyhedra.GetCompressedMesh(), speed)
        );
        PrepareSimulation();
        StartCoroutine("CurrentSimulationCoroutine");
    }
    public void StopCurrentSimulation()
    {
        StopCoroutine("CurrentSimulationCoroutine");
        onSimulationEnded.Invoke();
    }
    public void StartClassicWaveExperiment()
    {
        var polyVertices = polyhedra.GetCompressedMesh().vertices;
        float polySide = (polyVertices[1] - polyVertices[0]).magnitude;

        sim = new SinglePingSimulation(
            speed, time, polyhedra.GetCompressedMesh(),
            new ClassicWaveTetrahedronPropagator(
                polySide,
                speed,
                time
            )
        );
        PrepareSimulation();
        StartCoroutine("CurrentSimulationCoroutine");
        onSimulationStarted.Invoke();
    }
}
