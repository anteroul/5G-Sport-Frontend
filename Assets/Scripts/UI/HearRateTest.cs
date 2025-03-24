using UnityEngine;

public class HeartRateTest : MonoBehaviour
{
    public HeartRateBarController heartRateBarController; // Drag & Drop your HeartRateBarController
    [Range(40, 200)] public float testBPM = 80f; // Adjustable BPM in Inspector
    [Range(0f, 1f)] public float testHeartRateValue = 0.75f; // Adjustable fullness value

    void Update()
    {
        // Update the heart rate bar in real-time
        heartRateBarController.SetHeartRate(testHeartRateValue, testBPM);
    }
}

