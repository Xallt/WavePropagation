using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Experiment : MonoBehaviour
{
    public Polyhedra polyhedra;
    // Start is called before the first frame update
    public void StartExperiment()
    {
        Debug.Log("Experiment!");
        var sim = GetComponent<Simulation>();
        sim.StartSimulation();
    }
}
