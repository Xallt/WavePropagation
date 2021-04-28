using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveTypeMenuController : MonoBehaviour
{
    private void OnPingCounterIncrement(int vertex)
    {
        GetComponentInChildren<Counter>().Increment();
    }
    private void OnEnable()
    {
        Mesh polyMeshToSet = GetComponentInChildren<MeshSelector>().GetSelectedMesh();
        GameObject.Find("Polyhedra").GetComponent<Polyhedra>().SetMesh(polyMeshToSet);

        GameObject.Find("App").GetComponent<Simulation>().pingEvent.AddListener(OnPingCounterIncrement);
    }
    private void OnDisable()
    {
        GameObject.Find("App").GetComponent<Simulation>().pingEvent.RemoveListener(OnPingCounterIncrement);
    }
}
