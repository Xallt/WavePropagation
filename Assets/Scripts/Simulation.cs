using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class PingEvent : UnityEvent<int>
{
}

public interface Propagator
{
    IEnumerable<(float, int)> Propagate(int vertex);
}

public class DecayingWavePropagator : Propagator
{
    private Mesh _mesh;
    private HashSet<int>[] _adjacencyMap;
    private double[,] distance;
    private float waveSpeed;
    private void ComputeDistances()
    {
        double[] vertices = new double[_mesh.vertexCount * 3];
        for (int i = 0; i < _mesh.vertexCount; ++i)
        {
            vertices[3 * i] = _mesh.vertices[i].x;
            vertices[3 * i + 1] = _mesh.vertices[i].y;
            vertices[3 * i + 2] = _mesh.vertices[i].z;
        }
        uint[] faces = _mesh.triangles.Select(x => (uint)x).ToArray();
        double[,] matrix = GeodesicAlgorithmIntegration.geodesicExactComputeMatrix(vertices, faces);
        distance = matrix;
    }
    public DecayingWavePropagator(Mesh mesh, float waveSpeed)
    {
        _mesh = mesh;
        _adjacencyMap = WavePropagation.Utils.AdjacencyMap(mesh);
        this.waveSpeed = waveSpeed;
        ComputeDistances();
    }
    IEnumerable<(float, int)> Propagator.Propagate(int vertex)
    {
        foreach (var otherVertex in _adjacencyMap[vertex])
        {
            yield return (
                (float)distance[vertex, otherVertex] / waveSpeed,
                otherVertex
           );
        }
    }
}

public class Simulation: MonoBehaviour
{
    public double speed;
    public GameObject polyhedraGameObject;
    public uint maxPings;
    private Polyhedra polyhedra;
    private Mesh polyhedraMesh;
    private SortedSet<Tuple<double, int>> pings;
    private Queue<int> pingQueue;
    private Propagator propagator;

    public PingEvent pingEvent;
    public UnityEvent onSimulationEnded;
    private void Awake()
    {
        polyhedra = polyhedraGameObject.GetComponent<Polyhedra>();
    }
    public void OnEnable()
    {
        polyhedra.onMeshSet.AddListener(InitSimulationParameters);
    }
    public void SetPropagator(Propagator propagator)
    {
        this.propagator = propagator;
    }
    public void InitSimulationParameters()
    {
        polyhedraMesh = polyhedra.GetCompressedMesh();
        pings = new SortedSet<Tuple<double, int>>();
        pingQueue = new Queue<int>();
    }

    public void StartSimulation(int startingVertex)
    {
        pings.Add(Tuple.Create((double)Time.time, startingVertex));
        StartCoroutine("PingSimulationCoroutine");
    }

    public void EnqueuePing(int vertex)
    {
        pingQueue.Enqueue(vertex);
    }
    private void EndSimulation()
    {
        onSimulationEnded.Invoke();
    }
    private IEnumerator PingSimulationCoroutine()
    {
        while (true)
        {
            if (pings.Count > maxPings)
            {
                EndSimulation();
                break;
            }
            int currentPingedVertex = -1;
            double pingTime = -1;
            if (pings.Count > 0)
            {
                if (pings.Min.Item1 > Time.time)
                    yield return new WaitForSeconds((float)pings.Min.Item1 - Time.time);
                (pingTime, currentPingedVertex) = pings.Min;
                pings.Remove(pings.Min);
            }
            else
                break;
            pingEvent.Invoke(currentPingedVertex);
            foreach (var (time, otherVertex) in propagator.Propagate(currentPingedVertex))
            {
                pings.Add(Tuple.Create(
                    pingTime + time, 
                    otherVertex
               ));
            }
        }
    }
}
