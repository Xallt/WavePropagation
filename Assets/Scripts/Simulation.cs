using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor.Media;

[System.Serializable]
public class PingEvent : UnityEvent<int>
{
}

public interface IPropagator
{
    IEnumerable<(float, int)> Propagate(int vertex);
}

public class DecayingWavePropagator : IPropagator
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
    public IEnumerable<(float, int)> Propagate(int vertex)
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

public class ClassicWaveTetrahedronPropagator : IPropagator
{
    private float tetrahedronSide, waveSpeed, waveTimeToLive;
    private List<float> pingTimes;
    private void InitPingTimes()
    {
        pingTimes = new List<float>();
        HashSet<MediaRational> fracs = new HashSet<MediaRational>();
        float maxAllowedDistance = waveTimeToLive * waveTimeToLive;

        for (int level = 0; ; ++level)
        {
            for (int i = 0; i <= level; ++i)
            {
                int j = level - i;
                int ys3 = i, x = 1 - i % 2 + j; // ys3 is y / sqrt(3)
                float yf = ys3 * (float)Math.Sqrt(3), xf = (float)x;
                float distance2 = yf * yf + xf * xf;
                if (distance2 > maxAllowedDistance)
                {
                    continue;
                }
                MediaRational rel = new MediaRational(x, ys3);
                if (fracs.Contains(rel))
                {
                    continue;
                }
                fracs.Add(rel);
                pingTimes.Add(Mathf.Sqrt(distance2) / waveSpeed);
            }
            if (level > maxAllowedDistance)
                break;
        }
    }
    public ClassicWaveTetrahedronPropagator(float tetrahedronSide, float waveSpeed, float waveTimeToLive)
    {
        this.tetrahedronSide = tetrahedronSide;
        this.waveSpeed = waveSpeed;
        this.waveTimeToLive = waveTimeToLive;
        InitPingTimes();
    }
    public IEnumerable<(float, int)> Propagate(int vertex)
    {
        for (int i = 0; i < 4; ++i)
        {
            if (i == vertex)
                continue;
            foreach (float f in pingTimes)
                yield return (f, i);
        }
    }
}

public abstract class Simulation
{
    public PingEvent onPing;
    public float speed, time;
    public Simulation(float speed, float time)
    {
        onPing = new PingEvent();
        this.speed = speed;
        this.time = time;
    }
    public abstract IEnumerator StartSimulation();
}


public class MultiplePingSimulation: Simulation
{
    private SortedSet<Tuple<double, int>> pings;
    private Queue<int> pingQueue;
    private int startVertex;
    private IPropagator propagator;
    private float finishTime;
    public MultiplePingSimulation(float speed, float time, Mesh polyhedraMesh, IPropagator propagator, int startVertex = 0): base(speed, time)
    {
        this.propagator = propagator;
        this.startVertex = startVertex;
        pingQueue = new Queue<int>();
        pings = new SortedSet<Tuple<double, int>>();
    }

    public override IEnumerator StartSimulation()
    {
        pings.Clear();
        pingQueue.Clear();

        pings.Add(Tuple.Create((double)Time.time, startVertex));
        finishTime = Time.time + time;

        return PingSimulationCoroutine();
    }
    private IEnumerator PingSimulationCoroutine()
    {
        while (true)
        {
            if (Time.time > finishTime)
                break;
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
            onPing.Invoke(currentPingedVertex);
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

public class SinglePingSimulation : Simulation
{
    private SortedSet<Tuple<double, int>> pings;
    private Queue<int> pingQueue;
    private int startVertex;
    private IPropagator propagator;
    private float eps;
    private float finishTime;
    public SinglePingSimulation(float speed, float time, Mesh polyhedraMesh, IPropagator propagator, float eps = 0, int startVertex = 0) : base(speed, time)
    {
        this.propagator = propagator;
        this.startVertex = startVertex;
        this.eps = eps;
        pingQueue = new Queue<int>();
        pings = new SortedSet<Tuple<double, int>>();
    }

    public override IEnumerator StartSimulation()
    {
        pings.Clear();
        pingQueue.Clear();

        pings.Add(Tuple.Create((double)Time.time, startVertex));
        finishTime = Time.time + time;

        return PingSimulationCoroutine();
    }
    private IEnumerator PingSimulationCoroutine()
    {
        while (true)
        {
            if (Time.time > finishTime)
                break;
            int currentPingedVertex = -1;
            double pingTime = -1;
            
            if (pings.Count > 0)
            {
                if (pings.Min.Item1 > Time.time)
                    yield return new WaitForSeconds((float)pings.Min.Item1 - Time.time);
                (pingTime, currentPingedVertex) = pings.Min;
                List<Tuple<double, int>> toDelete = new List<Tuple<double, int>>();
                foreach (var p in pings)
                {
                    if (p.Item1 > pingTime + eps)
                        break;
                    if (p.Item2 == currentPingedVertex)
                        toDelete.Add(p);
                }
                foreach (var p in toDelete)
                    pings.Remove(p);
            }
            else
                break;
            onPing.Invoke(currentPingedVertex);
            foreach (var (time, otherVertex) in propagator.Propagate(currentPingedVertex))
            {
                if (pingTime + time <= finishTime)
                    pings.Add(Tuple.Create(
                        pingTime + time,
                        otherVertex
                   ));
            }
        }
    }
}
