using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveTypeMenuController : MonoBehaviour
{
    private SimulationController controller;
    public Counter Counter()
    {
        return GetComponentInChildren<Counter>();
    }
    private void CounterIncrement(int vertex)
    {
        Counter().Increment();
    }
    private void OnEnable()
    {
        Mesh polyMeshToSet = GetComponentInChildren<MeshSelector>().GetSelectedMesh();
        GameObject.Find("Polyhedra").GetComponent<Polyhedra>().SetMesh(polyMeshToSet);
        controller = GameObject.Find("App").GetComponent<SimulationController>();
        controller.onSimulationPing.AddListener(CounterIncrement);
    }
    private void OnDisable()
    {
        controller.onSimulationPing.RemoveListener(CounterIncrement);
    }
}
