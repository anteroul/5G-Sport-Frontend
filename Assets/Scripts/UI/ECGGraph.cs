using UnityEngine;
using System.Collections.Generic;

public class ECGGraph : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public int maxPoints = 512;
    public float xSpacing = 0.02f;
    private List<Vector3> points = new List<Vector3>();

    void Start()
    {
        lineRenderer.positionCount = 0;
        lineRenderer.useWorldSpace = false; // for 2D positioning
    }

    void Update()
    {
        float ecgValue = ECGWaveform(Time.time); // Simulated ECG

        if (points.Count >= maxPoints)
            points.RemoveAt(0);

        points.Add(new Vector3(points.Count * xSpacing, ecgValue, 0));

        // Shift x to scroll the graph left
        for (int i = 0; i < points.Count; i++)
            points[i] = new Vector3(i * xSpacing, points[i].y, 0);

        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }

    float ECGWaveform(float t)
    {
        float value = 0f;

        // Simulate QRS spike
        float modTime = t % 1.0f;
        if (modTime < 0.02f) value = -4.0f;         // Q wave
        else if (modTime < 0.04f) value = 40.0f;     // R peak
        else if (modTime < 0.06f) value = -8.0f;    // S dip
        else if (modTime < 0.12f) value = 8.0f;     // T wave
        else value = Mathf.PerlinNoise(t * 5, 0f) * 2.0f; // Baseline noise

        return value;
    }
}
