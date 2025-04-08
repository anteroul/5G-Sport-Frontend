using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class SensorDataManager : MonoBehaviour
{
    public List<Player> Players; // Assign in Inspector or dynamically
    private string jsonPath;

    void Start()
    {
        jsonPath = Path.Combine(Application.dataPath, "Resources/simulated_hockey_sensor_data_stream.json");
        InvokeRepeating(nameof(UpdatePlayersFromJSON), 1f, 5f); // check every 5 seconds
    }

    void UpdatePlayersFromJSON()
    {
        if (!File.Exists(jsonPath)) return;

        string json = File.ReadAllText(jsonPath);
        JObject root = JObject.Parse(json);
        var playersJson = root["players"];

        foreach (var playerProperty in playersJson.Children<JProperty>())
        {
            string playerKey = playerProperty.Name;
            JObject timestamps = (JObject)playerProperty.Value;

            // Get latest timestamp
            string latestTimestamp = null;
            foreach (var ts in timestamps.Properties())
            {
                if (latestTimestamp == null || string.Compare(ts.Name, latestTimestamp) > 0)
                    latestTimestamp = ts.Name;
            }

            if (latestTimestamp != null)
            {
                JObject latestData = (JObject)timestamps[latestTimestamp];
                var hr = latestData["HR"]["average"].Value<float>();
                var ecg = latestData["ECG"]["Samples"][0].Value<float>();
                var accX = latestData["IMU9"]["ArrayAcc"][0]["x"].Value<float>();
                var accY = latestData["IMU9"]["ArrayAcc"][0]["y"].Value<float>();
                var accZ = latestData["IMU9"]["ArrayAcc"][0]["z"].Value<float>();
                var accMag = Mathf.Sqrt(accX * accX + accY * accY + accZ * accZ);

                int playerId = int.Parse(playerKey.Split('_')[1]) - 1;
                if (playerId >= 0 && playerId < Players.Count)
                {
                    Players[playerId].UpdatePlayer(Mathf.RoundToInt(hr), ecg, accMag);
                }
            }
        }
    }
}
