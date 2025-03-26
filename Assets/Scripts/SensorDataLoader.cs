using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SensorDataLoader : MonoBehaviour
{
    public readonly float updateInterval = 0.15f;
    public string csvFilePath = "Assets/Resources/sensor_data.csv";
    private List<SensorData> sensorDataList = new List<SensorData>();
    private int currentIndex = 0;
    private float lastFileReadTime = 0;
    private readonly float checkInterval = 0.1f; // Check for updates every 100ms

    void Start()
    {
        LoadCSV();
        StartCoroutine(UpdateSensorData());
        StartCoroutine(CheckForFileUpdates());
    }

    void LoadCSV()
    {
        if (!File.Exists(csvFilePath))
        {
            Debug.LogError("CSV file not found!");
            return;
        }

        sensorDataList.Clear();

        try {
            using (FileStream fs = new FileStream(csvFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (StreamReader reader = new StreamReader(fs))
            {
                string line;
                bool isHeader = true;

                while ((line = reader.ReadLine()) != null)
                {
                    if (isHeader) { isHeader = false; continue; } // Skip header row

                    string[] values = line.Split(',');
                    if (values.Length < 10) continue;

                    try {
                        string timestampStr = values[0];  // Timestamp in mm:ss:ms format
                        string[] timeParts = timestampStr.Split(':');
                        int minutes = int.Parse(timeParts[0]);
                        int seconds = int.Parse(timeParts[1]);
                        int milliseconds = int.Parse(timeParts[2]);

                        // Convert mm:ss:ms to total seconds (float)
                        float timestamp = minutes * 60 + seconds + milliseconds / 1000f;

                        float accX = float.Parse(values[1]);
                        float accY = float.Parse(values[2]);
                        float accZ = float.Parse(values[3]);
                        float gyroX = float.Parse(values[4]);
                        float gyroY = float.Parse(values[5]);
                        float gyroZ = float.Parse(values[6]);
                        float magnX = float.Parse(values[7]);
                        float magnY = float.Parse(values[8]);
                        float magnZ = float.Parse(values[9]);

                        sensorDataList.Add(new SensorData(timestamp, accX, accY, accZ, gyroX, gyroY, gyroZ, magnX, magnY, magnZ));
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("Error parsing CSV: " + e.Message);
                    }
                }
            }
        } catch (Exception e) {
            Debug.LogError("Error reading CSV: " + e.Message);
        }

        Debug.Log("CSV Reloaded!");
        lastFileReadTime = Time.time;
    }

    IEnumerator UpdateSensorData()
    {
        while (true)
        {
            if (sensorDataList.Count > 0)
            {
                Debug.Log(sensorDataList[currentIndex].ToString());
                currentIndex = (currentIndex + 1) % sensorDataList.Count;
            }
            yield return new WaitForSeconds(updateInterval); // Update every 150ms
        }
    }

    IEnumerator CheckForFileUpdates()
    {
        while (true)
        {
            float lastModified = (float)(new FileInfo(csvFilePath).LastWriteTimeUtc - new DateTime(1970, 1, 1)).TotalSeconds;

            if (lastModified > lastFileReadTime)
            {
                LoadCSV(); // Reload data if file was modified
            }

            yield return new WaitForSeconds(checkInterval);
        }
    }
}
