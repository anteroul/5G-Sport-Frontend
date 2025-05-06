using UnityEngine;

public class HeartRateTest : MonoBehaviour
{
    public HeartRateBarController heartRateBarController; // Drag & Drop your HeartRateBarController
    [Range(40, 200)] public float testBPM = 80f; // Adjustable BPM in Inspector

    void Update()
    {
        // Update the heart rate bar in real-time
        if (heartRateBarController != null)
        {
            heartRateBarController.SetHeartRate(testBPM);
        }
    }
}
