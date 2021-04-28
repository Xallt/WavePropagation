using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveTypeMenuController : MonoBehaviour
{
    private Simulation sim;
    private void Awake()
    {
        sim = GameObject.Find("App").GetComponent<Simulation>();
    }
    private void OnPingCounterIncrement(int vertex)
    {
        GetComponentInChildren<Counter>().Increment();
    }
    private void OnEnable()
    {
        Mesh polyMeshToSet = GetComponentInChildren<MeshSelector>().GetSelectedMesh();
        GameObject.Find("Polyhedra").GetComponent<Polyhedra>().SetMesh(polyMeshToSet);

        sim.pingEvent.AddListener(OnPingCounterIncrement);
    }
    private void OnDisable()
    {
        sim.pingEvent.RemoveListener(OnPingCounterIncrement);
    }
}
