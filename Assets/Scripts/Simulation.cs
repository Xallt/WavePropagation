using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class PingEvent: UnityEvent<int>
{
}

public class Simulation : MonoBehaviour
{
    public enum GeodesicAlgorithmType
    {
        DijkstraAlgorithm
    }
    public GeodesicAlgorithmType algorithm;
    public float speed;
    public GameObject polyhedraGameObject;
    public PingEvent pingEvent;
    private Polyhedra polyhedra;
    private Geodesic.Mesh geodesicCompressedMesh;
    private SortedSet<Tuple<float, int>> pings;
    private Dictionary<Tuple<int, int>, float> distance;
    private Queue<int> pingQueue;
    
    void Start()
    {
        polyhedra = polyhedraGameObject.GetComponent<Polyhedra>();
        Mesh polyhedraMesh = polyhedra.GetMesh();
        geodesicCompressedMesh = new Geodesic.Mesh(
            polyhedraMesh.vertices.Select(v => new Geodesic.Vertex(null, new Geodesic.Vec3(v.x, v.y, v.z))).ToArray(),
            Enumerable.Range(0, polyhedraMesh.triangles.Length / 3).Select(ind => new Geodesic.Face(
                null,
                polyhedraMesh.triangles[ind * 3],
                polyhedraMesh.triangles[ind * 3 + 1],
                polyhedraMesh.triangles[ind * 3 + 2]
            )).ToArray()
        );
        pings = new SortedSet<Tuple<float, int>>();
        pingQueue = new Queue<int>();
    }

    private void ComputeDistances()
    {
        distance = new Dictionary<Tuple<int, int>, float>();
        if (algorithm == GeodesicAlgorithmType.DijkstraAlgorithm)
        {
            var geodesicAlgorithm = new Geodesic.DijkstraAlgorithm(geodesicCompressedMesh);
            for (int i = 0; i < geodesicCompressedMesh.vertices.Length; ++i)
            {
                geodesicAlgorithm.Propagate(Enumerable.Repeat(1, 1));
                for (int j = 0; j < geodesicCompressedMesh.vertices.Length; ++j)
                {
                    distance[Tuple.Create(i, j)] = (float)geodesicAlgorithm.DistanceTo(j);
                }
            }
        }
        
    }

    public void StartSimulation()
    {
        ComputeDistances();
        StartCoroutine("PingSimulationCoroutine");
    }

    public void EnqueuePing(int vertex)
    {
        pingQueue.Enqueue(vertex);
    }
    private float Distance(int a, int b)
    {
        return distance[Tuple.Create(a, b)]; // Insert Geodesic logic
    }
    private IEnumerator PingSimulationCoroutine()
    {
        while (true)
        {
            int currentPingedVertex = -1;
            float pingTime = -1;
            if (pings.Count > 0 && pings.Min.Item1 < Time.time)
            {
                (pingTime, currentPingedVertex) = pings.Min;
                pings.Remove(pings.Min);
            }                
            else if (pingQueue.Count > 0)
            {
                currentPingedVertex = pingQueue.Dequeue();
                pingTime = Time.time;
            }
            else if (pings.Count > 0)
            {
                yield return new WaitForSeconds(pings.Min.Item1 - Time.time);
                continue;
            } else
                yield return null;
            Debug.Log(currentPingedVertex);
            pingEvent.Invoke(currentPingedVertex);
            foreach (var halfEdge in geodesicCompressedMesh.GetVertexById(currentPingedVertex).AdjacentHalfEdges())
            {
                pings.Add(Tuple.Create(
                    pingTime + Distance(currentPingedVertex, halfEdge.b.id) / speed, 
                    halfEdge.b.id
               ));
            }
            yield return null;
        }
    }
}
