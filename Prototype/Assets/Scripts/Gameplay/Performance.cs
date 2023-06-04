using UnityEngine.Profiling;
using UnityEngine;
using System.IO;
using System.Text;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class Performance : MonoBehaviour
{

    private string filePath;
    private StringBuilder stringBuilder;

    public string nameOfSim = "Boids";

    private void Start()
    {
        stringBuilder = new StringBuilder();
        string quality = QualitySettings.names[QualitySettings.GetQualityLevel()];
        filePath = Application.persistentDataPath + "/DESKTOP_"+ nameOfSim + "_" + quality + "_ProfilerData.csv";
        stringBuilder.AppendLine("Seconds since start up, Average FPS, Current FPS, Lowest FPS, Highest FPS, Total Allocated Memory (MB), GPU Allocated Memory (MB)");
    }

    private float totalFps = 0;
    private float countFps = 0;

    private float highestFps = 0;
    private float lowestFps = Mathf.Infinity;

    private float lastFpsCheck = 0;

    private float second = 0;
    private bool done = false;

    private void Update()
    {
        float fps = 1.0f / Time.deltaTime;

        totalFps += fps;
        countFps++;

        highestFps = Mathf.Max(highestFps, fps);
        lowestFps = Mathf.Min(lowestFps, fps);

        if (!done)
            if (Time.time - lastFpsCheck >= 1)
            {
                second++;

                LogProfilerData();
                totalFps = 0;
                countFps = 0;

                highestFps = 0;
                lowestFps = Mathf.Infinity;

                lastFpsCheck = Time.time;
            }

        if (!done && second == 61)
        {
            done = true;
            Save();
        }
    }

    private void LogProfilerData()
    {
        float fps = 1.0f / Time.deltaTime;
        float average = totalFps / countFps;

        float gpuMemory = Profiler.GetAllocatedMemoryForGraphicsDriver() / 1024f / 1024f;
        float allocatedMemory = Profiler.GetTotalAllocatedMemoryLong() / 1024f / 1024f;

        Debug.Log(average.ToString() + "FPS");

        stringBuilder.AppendLine(second + "," + average + "," + fps + "," + lowestFps + "," + highestFps + "," + allocatedMemory + "," + gpuMemory);
    }

    private void Save()
    {
        File.WriteAllText(filePath, stringBuilder.ToString());
        Debug.Log("Profiler Data Saved to: " + filePath);
    }
}