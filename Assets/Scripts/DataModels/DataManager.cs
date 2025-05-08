using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// DataManager handles both fake and real JSON packets,
/// toggles emission, modularly calculates metrics, and updates the StatsPanel.
/// </summary>
public class DataManager : MonoBehaviour
{
    [Header("UI & Control")]
    public StatsPanelController statsPanel;

    [Tooltip("Enable to start emitting data (fake or real)")]
    public bool isEmitting = false;

    [Tooltip("Use generated fake data instead of real incoming data")]
    public bool useFakeData = true;

    [Header("Fake Data Settings")]
    public float fakeTickInterval = 3f;
    public int fakePlayerId = 12;
    public string fakePlayerName = "Test Player";
    public string fakeTeamName = "Team A";

    // Cumulative state for metrics and timing
    private float totalDistance = 0f;
    private float totalEnergy = 0f;
    private double lastLat;
    private double lastLon;
    private long lastTimestamp = 0;

    void Start()
    {
        // Initialize last known position
        lastLat = 60.223951;
        lastLon = 24.945632;

        // Start fake loop if configured
        StartCoroutine(FakeDataLoop());
    }

    /// <summary>
    /// Call this to process real JSON data from sensors or DB.
    /// Will run only when isEmitting and useFakeData is false.
    /// </summary>
    public void ProcessIncomingJson(string json)
    {
        if (!isEmitting || useFakeData) return;
        var packet = JsonUtility.FromJson<SensorPacket>(json);
        HandlePacket(packet);
    }

    /// <summary>
    /// Coroutine for fake data emission.
    /// Waits until isEmitting is true before each tick.
    /// </summary>
    private IEnumerator FakeDataLoop()
    {
        System.Random rnd = new System.Random();
        float currentHR = UnityEngine.Random.Range(90f, 110f);
        float currentSpeed = UnityEngine.Random.Range(0f, 8f);

        while (true)
        {
            // Wait for emission flag
            yield return new WaitUntil(() => isEmitting && useFakeData);

            // Wait interval
            yield return new WaitForSeconds(fakeTickInterval);

            // Generate timestamp
            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            // Smooth updates
            currentHR = Mathf.Clamp(currentHR + UnityEngine.Random.Range(-5f, 5f), 50f, 200f);
            currentSpeed = Mathf.Clamp(currentSpeed + UnityEngine.Random.Range(-2f, 2f), 0f, 35f);

            // Fake GNSS movement
            lastLat += UnityEngine.Random.Range(-0.00001f, 0.00001f);
            lastLon += UnityEngine.Random.Range(-0.00001f, 0.00001f);

            // Build fake packet
            var packet = new SensorPacket
            {
                playerId = fakePlayerId,
                sensorSetId = 1,
                sensors = new[] { new SensorData {
                    sensorId = 1,
                    HR = new HRData {
                        rrData         = new[] { (int)(60000f / currentHR) },
                        Movesense_series = "174630000192",
                        Pico_ID        = "self.picoW_id",
                        Timestamp_UTC  = now,
                        average        = currentHR
                    },
                    ECG = new ECGData {
                        Samples        = GenerateECGSamples(rnd),
                        Movesense_series = "174630000192",
                        Pico_ID        = "self.picoW_id",
                        Timestamp_UTC  = now,
                        Timestamp_ms   = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() % 100000
                    },
                    IMU9 = new IMU9Data {
                        Timestamp_UTC  = now,
                        Movesense_series = "174630000192",
                        Pico_ID        = "self.picoW_id",
                        Timestamp_ms   = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() % 100000,
                        ArrayAcc       = GenerateAccSamples(rnd),
                        ArrayGyro      = GenerateGyroSamples(rnd),
                        ArrayMagn      = GenerateMagnSamples(rnd)
                    },
                    GNSS = new GNSSData {
                        GNSS_sensor_ID = "174630000192",
                        Date           = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        Latitude       = lastLat,
                        Longitude      = lastLon
                    }
                } }
            };

            // Process generated packet
            HandlePacket(packet);
        }
    }

    /// <summary>
    /// Parses packet, calculates metrics modularly, updates UI.
    /// </summary>
    private void HandlePacket(SensorPacket packet)
    {
        var s = packet.sensors[0];
        float hr = s.HR.average;
        double lat = s.GNSS.Latitude;
        double lon = s.GNSS.Longitude;
        long now = s.HR.Timestamp_UTC;

        // Calculate time delta
        float delta = lastTimestamp > 0 ? (now - lastTimestamp) : fakeTickInterval;
        lastTimestamp = now;

        // Metrics
        float distThis = CalculateDistance(lat, lon);
        totalDistance += distThis;
        float spd = CalculateSpeed(distThis, delta);
        float energy = CalculateEnergy(hr, delta);
        totalEnergy += energy;
        int fatigue = CalculateFatigue(hr);

        // UI update
        statsPanel.UpdatePlayerInfo(packet.playerId,
            useFakeData ? fakePlayerName : packet.playerId.ToString(),
            useFakeData ? fakeTeamName : "");
        statsPanel.UpdateHeartRate(hr);
        statsPanel.UpdateEnergy(Mathf.RoundToInt(totalEnergy));
        statsPanel.UpdateEndurance(fatigue);
        statsPanel.UpdateSpeed(spd);
        statsPanel.UpdateDistance(totalDistance);
    }

    private float CalculateDistance(double newLat, double newLon)
    {
        const double R = 6371000; // meters
        double dLat = Mathf.Deg2Rad * ((float)newLat - (float)lastLat);
        double dLon = Mathf.Deg2Rad * ((float)newLon - (float)lastLon);
        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                          + Math.Cos(Mathf.Deg2Rad * (float)lastLat)
                          * Math.Cos(Mathf.Deg2Rad * (float)newLat)
                          * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        lastLat = newLat;
        lastLon = newLon;
        return (float)(R * c);
    }

    private float CalculateSpeed(float distanceMeters, float deltaSeconds)
    {
        if (deltaSeconds <= 0) return 0f;
        // Convert m/sec to km/h
        float mps = distanceMeters / deltaSeconds;
        return mps * 3.6f;
    }

    private float CalculateEnergy(float heartRate, float deltaSeconds)
    {
        // 0.1 kcal per bpm per minute
        float kcalPerMin = 0.1f * heartRate;
        return kcalPerMin * (deltaSeconds / 60f);
    }

    private int CalculateFatigue(float heartRate)
    {
        float norm = Mathf.InverseLerp(50f, 200f, heartRate);
        return Mathf.Clamp(Mathf.RoundToInt(norm * 7f), 0, 7);
    }

    // Helpers
    private int[] GenerateECGSamples(System.Random rnd)
    {
        var arr = new int[16]; for (int i = 0; i < 16; i++) arr[i] = rnd.Next(-50000, 50000); return arr;
    }
    private AccelSample[] GenerateAccSamples(System.Random rnd)
    {
        var a = new AccelSample[4]; for (int i = 0; i < 4; i++) a[i] = new AccelSample { x = (float)(rnd.NextDouble() * 4 - 2), y = (float)(rnd.NextDouble() * 4 - 2), z = (float)(rnd.NextDouble() * 4 - 2) }; return a;
    }
    private GyroSample[] GenerateGyroSamples(System.Random rnd)
    {
        var g = new GyroSample[4]; for (int i = 0; i < 4; i++) g[i] = new GyroSample { x = (float)(rnd.NextDouble() * 20 - 10), y = (float)(rnd.NextDouble() * 20 - 10), z = (float)(rnd.NextDouble() * 20 - 10) }; return g;
    }
    private MagnSample[] GenerateMagnSamples(System.Random rnd)
    {
        var m = new MagnSample[4]; for (int i = 0; i < 4; i++) m[i] = new MagnSample { x = (float)(rnd.NextDouble() * 100 - 50), y = (float)(rnd.NextDouble() * 100 - 50), z = (float)(rnd.NextDouble() * 100 - 50) }; return m;
    }
}