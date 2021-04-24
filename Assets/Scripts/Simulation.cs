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

public class Simulation : MonoBehaviour
{
    public enum GeodesicAlgorithmType
    {
        DijkstraAlgorithm,
        ExactAlgorithm
    }
    public GeodesicAlgorithmType algorithm;
    public double speed;
    public GameObject polyhedraGameObject;
    public uint maxPings;
    private Polyhedra polyhedra;
    private Mesh polyhedraMesh;
    private Geodesic.Mesh geodesicCompressedMesh;
    private SortedSet<Tuple<double, int>> pings;
    private Dictionary<Tuple<int, int>, double> distance;
    private Queue<int> pingQueue;

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
    public void InitSimulationParameters()
    {
        polyhedraMesh = polyhedra.GetCompressedMesh();
        geodesicCompressedMesh = new Geodesic.Mesh(
            polyhedraMesh.vertices.Select(v => new Geodesic.Vertex(null, new Geodesic.Vec3(v.x, v.y, v.z))).ToArray(),
            Enumerable.Range(0, polyhedraMesh.triangles.Length / 3).Select(ind => new Geodesic.Face(
                null,
                polyhedraMesh.triangles[ind * 3],
                polyhedraMesh.triangles[ind * 3 + 1],
                polyhedraMesh.triangles[ind * 3 + 2]
            )).ToArray()
        );
        pings = new SortedSet<Tuple<double, int>>();
        pingQueue = new Queue<int>();
    }

    private void ComputeDistances()
    {
        distance = new Dictionary<Tuple<int, int>, double>();
        if (algorithm == GeodesicAlgorithmType.DijkstraAlgorithm)
        {
            var geodesicAlgorithm = new Geodesic.DijkstraAlgorithm(geodesicCompressedMesh);
            for (int i = 0; i < geodesicCompressedMesh.vertices.Length; ++i)
            {
                geodesicAlgorithm.Propagate(Enumerable.Repeat(1, 1));
                for (int j = 0; j < geodesicCompressedMesh.vertices.Length; ++j)
                {
                    distance[Tuple.Create(i, j)] = geodesicAlgorithm.DistanceTo(j);
                }
            }
        } else if (algorithm == GeodesicAlgorithmType.ExactAlgorithm)
        {
            double[] vertices = new double[polyhedraMesh.vertexCount * 3];
            for (int i = 0; i < polyhedraMesh.vertexCount; ++i)
            {
                vertices[3 * i] = polyhedraMesh.vertices[i].x;
                vertices[3 * i + 1] = polyhedraMesh.vertices[i].y;
                vertices[3 * i + 2] = polyhedraMesh.vertices[i].z;
            }
            uint[] faces = polyhedraMesh.triangles.Select(x => (uint)x).ToArray();
            double[,] matrix = GeodesicAlgorithmIntegration.geodesicExactComputeMatrix(vertices, faces);
            for (int i = 0; i < polyhedraMesh.vertexCount; ++i)
            {
                for (int t = 0; t < polyhedraMesh.vertexCount; ++t)
                    distance[Tuple.Create(i, t)] = matrix[i, t];
            }
        }
        
    }

    public void StartSimulation(bool onlinePinging = false)
    {
        ComputeDistances();
        if (!onlinePinging)
        {
            double curTime = Time.time;
            foreach (int pingedVertex in pingQueue)
                pings.Add(Tuple.Create(curTime, pingedVertex));
            StartCoroutine("PingSimulationCoroutine");
        }
            
        else
            throw new NotImplementedException();
    }

    public void EnqueuePing(int vertex)
    {
        pingQueue.Enqueue(vertex);
    }
    private double Distance(int a, int b)
    {
        return distance[Tuple.Create(a, b)];
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
            var adjacentHalfEdges = polyhedra.GetAdjacentVertices(currentPingedVertex).ToArray();
            foreach (var otherVertex in adjacentHalfEdges)
            {
                pings.Add(Tuple.Create(
                    pingTime + Distance(currentPingedVertex, otherVertex) / speed, 
                    otherVertex
               ));
            }
        }
    }
}
