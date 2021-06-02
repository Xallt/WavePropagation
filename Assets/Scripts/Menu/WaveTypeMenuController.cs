using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveTypeMenuController : MonoBehaviour
{
    private SimulationController controller;
    private Slider timeSlider;
    private Slider speedSlider;
    public Counter Counter()
    {
        return GetComponentInChildren<Counter>();
    }
    private void CounterIncrement(int vertex, float time)
    {
        Counter().Increment();
    }
    private void SetTime(float value)
    {
        controller.SetTime(value);
    }
    private void SetSpeed(float value)
    {
        controller.SetSpeed(value);
    }
    private void Awake()
    {
        controller = GameObject.Find("App").GetComponent<SimulationController>();
        timeSlider = transform.Find("Time").transform.Find("Slider").GetComponent<Slider>();
        SetTime(timeSlider.value);
        speedSlider = transform.Find("Speed").transform.Find("Slider").GetComponent<Slider>();
        SetSpeed(speedSlider.value);
    }
    private void OnEnable()
    {
        Mesh polyMeshToSet = GetComponentInChildren<MeshSelector>().GetSelectedMesh();
        GameObject.Find("Polyhedra").GetComponent<Polyhedra>().SetMesh(polyMeshToSet);
        
        controller.onSimulationPing.AddListener(CounterIncrement);
        timeSlider.onValueChanged.AddListener(SetTime);
        speedSlider.onValueChanged.AddListener(SetSpeed);
    }
    private void OnDisable()
    {
        controller.onSimulationPing.RemoveListener(CounterIncrement);
        timeSlider.onValueChanged.RemoveListener(SetTime);
        speedSlider.onValueChanged.RemoveListener(SetSpeed);
    }
}
