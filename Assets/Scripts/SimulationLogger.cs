using System.IO;
using System;
using System.Globalization;
using UnityEngine;
using Sirenix.OdinInspector;

public class SimulationLogger : MonoBehaviour
{
    [FolderPath]
    public string pathToLogFolder;

    private SimulationController controller;
    private StreamWriter writer;
    private int pingCounter;
    private float startTime;
    private void Awake()
    {
        controller = GameObject.Find("App").GetComponent<SimulationController>();
    }
    private void OpenLogFile()
    {
        // Create Log folder if it doesn't exist yet
        Directory.CreateDirectory(pathToLogFolder);
        DateTime now = DateTime.Now;

        string filename = now.ToString("dd/MM/yyyy") + "-" + WavePropagation.Utils.GetHashString(now.ToString("s"), 6) + ".txt";
        string filepath = Path.Combine(pathToLogFolder, filename);
        writer = new StreamWriter(filepath);
        startTime = -1;
    }
    private void LogCounterToTime(int vertex, float time)
    {
        if (startTime == -1)
            startTime = time;
        pingCounter += 1;
        NumberFormatInfo timeFormat = new NumberFormatInfo();
        timeFormat.NumberDecimalSeparator = ".";
        writer.WriteLine(String.Format("{0},{1}", (time - startTime).ToString(timeFormat), pingCounter));
    }
    private void CloseLogFile()
    {
        writer.Close();
    }
    private void Start()
    {
        controller.onSimulationStarted.AddListener(OpenLogFile);
        controller.onSimulationPing.AddListener(LogCounterToTime);
        controller.onSimulationEnded.AddListener(CloseLogFile);
    }
}
