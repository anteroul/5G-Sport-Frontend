using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SensorDataLoader : MonoBehaviour
{
    public TextAsset csvFile; // Assign the CSV file in the Unity Inspector
    private List<SensorData> sensorDataList = new List<SensorData>();
    private int currentIndex = 0;

    void Start()
    {
        LoadCSV();
        StartCoroutine(UpdateSensorData());
    }

    void LoadCSV()
    {
        if (csvFile == null)
        {
            Debug.LogError("CSV file not assigned!");
            return;
        }

        StringReader reader = new StringReader(csvFile.text);
        string line;
        bool isHeader = true;

        while ((line = reader.ReadLine()) != null)
        {
            if (isHeader) { isHeader = false; continue; } // Skip header row

            string[] values = line.Split(',');
            if (values.Length < 10) continue;

            try
            {
                float timestamp = string.IsNullOrWhiteSpace(values[0]) ? -1f : float.Parse(values[0]);
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
                Debug.LogError("Error parsing CSV line: " + line + "\n" + e.Message);
            }
        }
    }

    IEnumerator UpdateSensorData()
    {
        while (currentIndex < sensorDataList.Count)
        {
            Debug.Log(sensorDataList[currentIndex].ToString());
            currentIndex++;
            yield return new WaitForSeconds(0.1f); // 100ms delay
        }
    }
}
