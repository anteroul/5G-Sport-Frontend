using UnityEngine;
using System.Collections.Generic;

public class SensorDataReader : MonoBehaviour
{
    public readonly int PlayerCnt = 6;
    public List<Player> Players;
    private Root root;

    void Awake()
    {
        for (int i = 0; i < PlayerCnt / 2; i++)
        {
            Players.Add(new Player(i, "John Doe", false));
        }
        for (int i = PlayerCnt / 2; i < PlayerCnt; i++)
        {
            Players.Add(new Player(i, "John Doe", true));
        }

        ReadSensorData();
    }

    void Update()
    {
        ReadSensorData();
    }

    void ReadSensorData()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("sensor_data.json");
        if (jsonFile == null)
        {
            Debug.LogError("JSON file not found.");
            return;
        }

        root = JsonUtility.FromJson<Root>(jsonFile.text);

        for (int i = 0; i < root.sensors.Length; i++)
        {
            Sensor sensor = root.sensors[i];

            float hr = sensor.HR.average;
            int[] ecg = sensor.ECG.Samples != null && sensor.ECG.Samples.Length > 0 ? sensor.ECG.Samples : null;

            Vector3Array acc = sensor.IMU9.ArrayAcc[0];
            float accMag = Mathf.Sqrt(acc.x * acc.x + acc.y * acc.y + acc.z * acc.z);

            if (i < Players.Count)
            {
                Players[i].UpdatePlayer(Mathf.RoundToInt(hr), ecg, accMag);
                Debug.Log($"Updated Player {i + 1} | HR: {hr}, ECG: {ecg}, AccMag: {accMag}");
            }
        }
    }
}
