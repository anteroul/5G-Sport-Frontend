using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using TMPro; // Required for TextMeshPro

// Serializable classes matching the expected JSON schema
[Serializable]
public class SensorPacket
{
    public int playerId;
    public int sensorSetId;
    public SensorData[] sensors;
}

[Serializable]
public class SensorData
{
    public int sensorId;
    public HRData HR;
    public ECGData ECG;
    public IMU9Data IMU9;
    public GNSSData GNSS;
}

[Serializable]
public class HRData
{
    public int[] rrData;
    public string Movesense_series;
    public string Pico_ID;
    public long Timestamp_UTC;
    public float average;
}

[Serializable]
public class ECGData
{
    public int[] Samples;
    public string Movesense_series;
    public string Pico_ID;
    public long Timestamp_UTC;
    public long Timestamp_ms;
}

[Serializable]
public class AccelSample { public float x, y, z; }
[Serializable]
public class GyroSample { public float x, y, z; }
[Serializable]
public class MagnSample { public float x, y, z; }

[Serializable]
public class IMU9Data
{
    public long Timestamp_UTC;
    public string Movesense_series;
    public string Pico_ID;
    public long Timestamp_ms;
    public AccelSample[] ArrayAcc;
    public GyroSample[] ArrayGyro;
    public MagnSample[] ArrayMagn;
}

[Serializable]
public class GNSSData
{
    public string GNSS_sensor_ID;
    public string Date;
    public double Latitude;
    public double Longitude;
}

/// <summary>
/// Generates realistic fake sensor JSON packets with smooth value changes,
/// then applies them to the StatsPanelController, gated by external control.
/// </summary>
public class FakeDataController : MonoBehaviour
{
    [Header("UI Reference")]
    public StatsPanelController statsPanel;

    [Header("Control Flags")]
    public bool isEmitting = false;   // Controlled externally

    [Header("Simulation Settings")]
    public int playerId = 12;
    public int sensorSetId = 1;
    public float tickInterval = 3f;

    [Header("Internal State")]
    private float currentHR;
    private float currentSpeed;
    private float totalDistance;
    private float totalEnergy;
    private double lastLat = 60.223951;
    private double lastLon = 24.945632;

    [Header("Player Card UI")]
    public TMP_Text playerNameTxt;
    public TMP_Text playerIdTxt;
    public string playerName = "xxx xxx";
    public string playerTeam; 

    void Start()
    {
        playerNameTxt.text = playerName;
        playerIdTxt.text = playerId.ToString();

        // Initialize smooth state
        currentHR = UnityEngine.Random.Range(90f, 110f);
        currentSpeed = UnityEngine.Random.Range(5f, 10f);
        totalDistance = 0f;
        totalEnergy = 0f;
        StartCoroutine(EmitFakeData());
    }

    IEnumerator EmitFakeData()
    {
        while (true)
        {
            // Wait until external logic enables emission
            yield return new WaitUntil(() => isEmitting);

            // Smooth updates
            currentHR = Mathf.Clamp(currentHR + UnityEngine.Random.Range(-5f, 5f), 50f, 200f);
            currentSpeed = Mathf.Clamp(currentSpeed + UnityEngine.Random.Range(-2f, 2f), 0f, 35f);

            // Compute new position slightly
            lastLat += UnityEngine.Random.Range(-0.00001f, 0.00001f);
            lastLon += UnityEngine.Random.Range(-0.00001f, 0.00001f);

            // Distance & energy accumulation
            float distThisTick = currentSpeed * (tickInterval / 3600f) * 1000f;
            totalDistance += distThisTick;
            float kcalThisTick = (0.1f * currentHR) * (tickInterval / 60f);
            totalEnergy += kcalThisTick;

            // Build packet
            var packet = new SensorPacket
            {
                playerId = playerId,
                sensorSetId = sensorSetId,
                sensors = new[] {
                    new SensorData {
                        sensorId = 1,
                        HR = new HRData { rrData = new[] { (int)(60000f/currentHR) }, Movesense_series = "174630000192", Pico_ID = "self.picoW_id", Timestamp_UTC = DateTimeOffset.UtcNow.ToUnixTimeSeconds(), average = currentHR },
                        ECG = new ECGData { Samples = GenerateECGSamples(), Movesense_series = "174630000192", Pico_ID = "self.picoW_id", Timestamp_UTC = DateTimeOffset.UtcNow.ToUnixTimeSeconds(), Timestamp_ms = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() % 100000 },
                        IMU9 = new IMU9Data { Timestamp_UTC = DateTimeOffset.UtcNow.ToUnixTimeSeconds(), Movesense_series = "174630000192", Pico_ID = "self.picoW_id", Timestamp_ms = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() % 100000, ArrayAcc = GenerateAccSamples(), ArrayGyro = GenerateGyroSamples(), ArrayMagn = GenerateMagnSamples() },
                        GNSS = new GNSSData { GNSS_sensor_ID = "174630000192", Date = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"), Latitude = lastLat, Longitude = lastLon }
                    }
                }
            };

            // Serialize & apply
            string json = JsonUtility.ToJson(packet);
            ApplyToStatsPanel(json);

            yield return new WaitForSeconds(tickInterval);
        }
    }

    void ApplyToStatsPanel(string json)
    {
        var pkt = JsonUtility.FromJson<SensorPacket>(json);
        var s = pkt.sensors[0];
        statsPanel.UpdatePlayerInfo(pkt.playerId, playerName , playerTeam);
        statsPanel.UpdateHeartRate(s.HR.average);
        statsPanel.UpdateEnergy(Mathf.RoundToInt(totalEnergy));
        statsPanel.UpdateEndurance(Mathf.Clamp(Mathf.FloorToInt((s.HR.average - 50f) / 150f * 7f), 0, 7));
        statsPanel.UpdateSpeed(currentSpeed);
        statsPanel.UpdateDistance(totalDistance);
    }

    // Helpers
    int[] GenerateECGSamples() { var arr = new int[16]; for (int i = 0; i < 16; i++) arr[i] = UnityEngine.Random.Range(-50000, 50000); return arr; }
    AccelSample[] GenerateAccSamples() { var a = new AccelSample[4]; for (int i = 0; i < 4; i++) a[i] = new AccelSample { x = UnityEngine.Random.Range(-2f, 2f), y = UnityEngine.Random.Range(-2f, 2f), z = UnityEngine.Random.Range(-2f, 2f) }; return a; }
    GyroSample[] GenerateGyroSamples() { var g = new GyroSample[4]; for (int i = 0; i < 4; i++) g[i] = new GyroSample { x = UnityEngine.Random.Range(-10f, 10f), y = UnityEngine.Random.Range(-10f, 10f), z = UnityEngine.Random.Range(-10f, 10f) }; return g; }
    MagnSample[] GenerateMagnSamples() { var m = new MagnSample[4]; for (int i = 0; i < 4; i++) m[i] = new MagnSample { x = UnityEngine.Random.Range(-50f, 50f), y = UnityEngine.Random.Range(-50f, 50f), z = UnityEngine.Random.Range(-50f, 50f) }; return m; }
}